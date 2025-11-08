using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class WaypointPatroller : MonoBehaviour
{
    [SerializeField] private float acceptance_threshold = 1f;
    
    public List<Waypoint> waypoints;
    public CharacterController characterController;
    public float moveSpeed = 5f;
    public UnityEvent<Vector3> onMove;

    private bool patrolling = false;
    private int current_waypoint_index = -1;
    private Waypoint current_waypoint = null;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        Assert.IsNotNull(characterController);
    }

    private void Start()
    {
        Assert.IsTrue(waypoints.Count > 0);
        TargetClosestWaypoint();
    }

    private void Update()
    {
        if (!patrolling || !characterController.enabled)
            return;

        Vector3 displacement = current_waypoint.transform.position - transform.position;
        displacement.y = 0f;
        if (displacement.magnitude <= acceptance_threshold)
        {
            TargetNextWaypoint();
            displacement = current_waypoint.transform.position - transform.position;
            displacement.y = 0f;
        }

        transform.LookAt(current_waypoint.transform.position);
        displacement = Mathf.Min(moveSpeed * Time.deltaTime, displacement.magnitude) * displacement.normalized;
        characterController.Move(displacement);
        onMove.Invoke(displacement);
    }

    private void TargetNextWaypoint()
    {
        current_waypoint_index = (current_waypoint_index + 1) % waypoints.Count;
        current_waypoint = waypoints[current_waypoint_index];
    }

    private void TargetClosestWaypoint()
    {
        float minDistance = float.MaxValue;
        for (int i = 0; i < waypoints.Count; i++)
        {
            Waypoint waypoint = waypoints[i];
            float distance = (waypoint.transform.position - transform.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                current_waypoint_index = i;
                current_waypoint = waypoint;
            }
        }
    }

    public void StartPatrol()
    {
        if (!patrolling)
        {
            patrolling = true;
            TargetClosestWaypoint();
        }
    }

    public void StopPatrol()
    {
        patrolling = false;
    }
}
