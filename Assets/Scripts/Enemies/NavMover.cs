using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

class NavMover : MonoBehaviour
{
    [SerializeField, Min(0f)] private float navRecalculateThreshold = 0.1f;
    [SerializeField, Min(0f)] private float rejoinMeshMaxDistance = 0.5f;
    [SerializeField, Min(0f)] private float onMeshThreshold = 0.1f;

    private CharacterController controller;
    private NavMeshAgent agent;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        agent = GetComponent<NavMeshAgent>();
        Assert.IsNotNull(agent);
        // Manual movement with NavMeshAgent pathfinding
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    public void LookInDirection(Vector3 direction, float turnSpeed)
    {
        direction.y = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
    }

    private void RecomputeAgentPath(Vector3 displacement)
    {
        agent.nextPosition = transform.position;

        Vector3 target = transform.position + displacement;
        if (Vector3.Distance(target, agent.destination) > navRecalculateThreshold)
            agent.SetDestination(target);
    }

    private Vector3 ComputeAgentVelocity(Vector3 displacement)
    {
        RecomputeAgentPath(displacement);
        Vector3 velocity = agent.desiredVelocity;
        velocity.y = 0f;
        return velocity.normalized;
    }

    public Vector3 MoveController(Vector3 displacement, float moveSpeed, float turnSpeed)
    {
        if (!controller.enabled)
            return Vector3.zero;


        if (!agent.isOnNavMesh)
            return MoveWithoutAI(displacement, displacement, moveSpeed, turnSpeed);

        Vector3 velocity = ComputeAgentVelocity(displacement);

        LookInDirection(velocity.magnitude > 0.001f ? velocity : displacement, turnSpeed);

        Vector3 movement = moveSpeed * Time.deltaTime * velocity;
        controller.Move(movement);

        return movement;
    }

    public Vector3 MoveController(Vector3 displacement, float stoppingDistance, float moveSpeed, float turnSpeed)
    {
        if (!controller.enabled)
            return Vector3.zero;

        Vector3 lookDirection = displacement;
        displacement = (displacement.magnitude - stoppingDistance) * displacement.normalized;

        if (!agent.isOnNavMesh)
            return MoveWithoutAI(displacement, lookDirection, moveSpeed, turnSpeed);

        Vector3 velocity = ComputeAgentVelocity(displacement);

        LookInDirection(lookDirection, turnSpeed);

        Vector3 movement = moveSpeed * Time.deltaTime * velocity;
        controller.Move(movement);

        return movement;
    }

    private Vector3 MoveWithoutAI(Vector3 displacement, Vector3 lookDirection, float moveSpeed, float turnSpeed)
    {
        // Try to rejoin mesh
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, rejoinMeshMaxDistance, NavMesh.AllAreas))
            displacement = hit.position - transform.position;
        
        LookInDirection(lookDirection, turnSpeed);

        Vector3 movement = Mathf.Min(moveSpeed * Time.deltaTime, displacement.magnitude) * displacement.normalized;
        controller.Move(movement);

        return movement;
    }

    public void RestrictMovement(ref Vector3 displacement)
    {
        if (displacement.sqrMagnitude < Mathf.Epsilon) return;

        Vector3 start = transform.position;
        Vector3 end = start + displacement;

        float bestFactor = 0f;
        float low = 0f;
        float high = 1f;

        // Binary search
        int maxIterations = 10;
        for (int i = 0; i < maxIterations; ++i)
        {
            float mid = (low + high) * 0.5f;
            Vector3 test = Vector3.Lerp(start, end, mid);
            if (NavMesh.SamplePosition(test, out NavMeshHit _, onMeshThreshold, NavMesh.AllAreas))
            {
                bestFactor = mid;
                low = mid;
            }
            else
                high = mid;
        }

        Debug.Log(bestFactor);
        displacement *= bestFactor;
    }
}
