using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Runner : MonoBehaviour
{
    RunnerInput input;

    NavMeshAgent agent;
    //Vector3 destinationPos;
    GameObject charObj;

    Camera cam;
    Vector3 pointerPos;

    private void Awake()
    {
        input = new RunnerInput();

        agent = GetComponent<NavMeshAgent>();
        charObj = GetComponentInChildren<Animator>().gameObject;
    }
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        cam = GameObject.FindObjectsByType<Camera>(FindObjectsSortMode.None) [0];
        agent.updateRotation = false; Debug.Log("Disabled rotation on runner navmesh agent");
    }
    private void Update()
    {
        //Debug.DrawRay(cam.ScreenToWorldPoint(input.Player.Point.ReadValue<Vector2>()), cam.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast( cam.ScreenToWorldPoint(input.Player.Point.ReadValue<Vector2>()), cam.transform.forward, out hit ))
        {
            pointerPos = hit.point;
            Debug.DrawRay(pointerPos, Vector3.up);
        }

        if (input.Player.Tap.IsPressed())
        {
            MoveToPoint();
        }
    }
    private void FixedUpdate()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            charObj.transform.rotation = Quaternion.Lerp( charObj.transform.rotation, Quaternion.LookRotation(agent.velocity), 0.5f);
            Debug.Log(Quaternion.LookRotation(agent.velocity));
        }
    }
    public void MoveToPoint()
    {
        agent.SetDestination(pointerPos);
    }
}
