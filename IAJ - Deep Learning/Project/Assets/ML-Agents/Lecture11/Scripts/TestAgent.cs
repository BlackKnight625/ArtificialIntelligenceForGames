using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.SceneManagement;

public class TestAgent : Agent
{

    MovementController movementController;

    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 goalStartPos;



    public override void OnEpisodeBegin()
    {
        ResetAgent();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //Add an existential penalty

        // TODO
        //switch (actionBuffers.DiscreteActions[0])
        //{
        // See the available actions in the movement controller 
        //}

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Here you can add extra observations that you want your agent to know of
        // for instance the agent's and goal's locations(raycasts are automatically perceived)

        //sensor.AddObservation(?);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // When the agent hits either a wall or the goal you should add a reward

        // TODO
        if (collision.gameObject.tag == "Goal")
        {
            AddReward(0f);
            EndEpisode();
        }
        if (collision.gameObject.tag == "Wall")
        {
            AddReward(0f);
            EndEpisode();
        }
    }

    public void ResetAgent()
    {
        movementController = null;
        movementController = GetComponent<MovementController>();
        movementController.SetStartPosition(new Vector3(Random.Range(2.90f, 4.60f), 0.25f, Random.Range(-1.80f, 1.80f)));
        movementController.SetGoalStartPosition(new Vector3(Random.Range(-4.70f, -1.80f), 0.25f, Random.Range(-1.80f, 1.80f)));
    }

}
