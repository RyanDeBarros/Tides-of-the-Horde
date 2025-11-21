using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

class NavMover : MonoBehaviour
{
    [SerializeField] private float navRecalculateThreshold = 0.1f;

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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
    }

    private void RecomputeAgentPath(Vector3 displacement)
    {
        agent.nextPosition = transform.position;

        Vector3 target = transform.position + displacement;
        if (Vector3.Distance(target, agent.destination) > navRecalculateThreshold)
            agent.SetDestination(target);
    }

    public Vector3 MoveController(Vector3 displacement, float moveSpeed, float turnSpeed)
    {
        RecomputeAgentPath(displacement);

        Vector3 velocity = agent.desiredVelocity;
        velocity.y = 0f;

        LookInDirection(velocity.magnitude > 0.001f ? velocity : displacement, turnSpeed);

        Vector3 movement = moveSpeed * Time.deltaTime * velocity.normalized;
        controller.Move(movement);

        return movement;
    }

    public Vector3 MoveController(Vector3 displacement, float stoppingDistance, float moveSpeed, float turnSpeed)
    {
        RecomputeAgentPath((displacement.magnitude - stoppingDistance) * displacement.normalized);

        Vector3 velocity = agent.desiredVelocity;
        velocity.y = 0f;

        LookInDirection(displacement.normalized, turnSpeed);

        Vector3 movement = moveSpeed * Time.deltaTime * velocity.normalized;
        controller.Move(movement);

        return movement;
    }
}
