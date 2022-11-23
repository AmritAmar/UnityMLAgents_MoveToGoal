using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    // Reinforcement Learning
    // Observation -> Decision -> Action -> Reward -> Observation

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    // Start of Cycle
    public override void OnEpisodeBegin()
    {
        // Reset position of Agent and Goal
        switch (Random.Range(0, 4))
        {
            case 0:
                transform.localPosition = new Vector3(Random.Range(1f, 5f), 1, Random.Range(-5f, 5f));
                targetTransform.localPosition = new Vector3(Random.Range(-5f, -1f), 1, Random.Range(-5f, 5f));
                break;
            case 1:
                transform.localPosition = new Vector3(Random.Range(-5f, -1f), 1, Random.Range(-5f, 5f));
                targetTransform.localPosition = new Vector3(Random.Range(1f, 5f), 1, Random.Range(-5f, 5f));
                break;
            case 2:
                transform.localPosition = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(-5f, -1f));
                targetTransform.localPosition = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(1f, 5f));
                break;
            case 3:
                transform.localPosition = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(1f, 5f));
                targetTransform.localPosition = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(-5f, -1f));
                break;
        }
    }

    // Observation
    public override void CollectObservations(VectorSensor sensor)
    {
        // Position of Agent and Position of Ball
        // Maybe use distance from ball and direction of ball?
        sensor.AddObservation(transform.localPosition); // 3
        sensor.AddObservation(targetTransform.localPosition); // 3
        // Total Observations = 6
    }

    // Action
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 7.5f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    // Check if we got the actions correct by specifying our own decisions and setting action directly
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
    }

    // Rewards
    private void OnTriggerEnter(Collider other)
    {
        // SetReward() vs AddReward()
        if (other.CompareTag("Goal"))
        {
            SetReward(1f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.CompareTag("Wall"))
        {
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }

}
