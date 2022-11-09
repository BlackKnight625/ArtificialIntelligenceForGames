using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    Rigidbody agentRigidbody;

    public int turnDirection; //0 forward; 1 right; 2 left; 3 back
    [SerializeField] float agentSpeed = 2f;

    [SerializeField] float agentMaxSpeed = 4f;

    [SerializeField] GameObject goal;

    public GameObject checkpoint1;

    public GameObject checkpoint2;

    void Awake()
    {
        agentRigidbody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        Vector3 dirToGo = Vector3.zero;
        if (turnDirection == 0 || Input.GetKey(KeyCode.W))
        {
            dirToGo = transform.forward * agentSpeed;

        }
        if (turnDirection == 1 || Input.GetKey(KeyCode.D))
        {
            dirToGo = transform.right * agentSpeed;

        }
        if (turnDirection == 2 || Input.GetKey(KeyCode.A))
        {
            dirToGo = -transform.right * agentSpeed;

        }
        if(turnDirection == 3 || Input.GetKey(KeyCode.S))
        {
            dirToGo = -transform.forward * agentSpeed;
        }

        agentRigidbody.velocity = dirToGo * agentMaxSpeed;
        transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
    }


    public void SetDirection(int dir)
    {
        turnDirection = dir;
    }

    public void SetStartPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }


    public void SetGoalStartPosition(Vector3 pos)
    {
        goal.transform.localPosition = pos;
    }

    public Vector3 GetGoalPosition()
    {
        return goal.transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            turnDirection = 4;
        }
    }


}
