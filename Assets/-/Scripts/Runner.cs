using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class Runner : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject charObj;

    [SerializeField] CinemachineSmoothPath path;
    public float targetPos = 0;
    public float distanceIncrement = 1f;
    public float posCheckDistance = 2f;
    public float checkIntervalSec = 0.3f;

    public bool isAiControlled = true; //AI by default

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        charObj = GetComponentInChildren<Animator>().gameObject;
    }
    private void Start()
    {
        agent.updateRotation = false; Debug.Log("Disabled rotation on runner navmesh agent");

        if (isAiControlled == false) return;

        InitializeAi();
    }
    private void FixedUpdate()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            charObj.transform.rotation = Quaternion.Lerp( charObj.transform.rotation, Quaternion.LookRotation(agent.velocity), 1f);
            //Debug.Log(Quaternion.LookRotation(agent.velocity));
        }
    }

    public void MoveToPoint(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    void InitializeAi()
    {
        agent.SetDestination(path.EvaluatePosition(0));
        StartCoroutine( MoveAiCoroutine() );
    }
    void MoveAi()
    {
        if ( (path.EvaluatePosition(targetPos) - transform.position).sqrMagnitude < posCheckDistance * posCheckDistance )
        {
            targetPos += (distanceIncrement / path.PathLength) * path.MaxPos;
            if (targetPos >= path.MaxPos) targetPos = 0;

            agent.SetDestination(path.EvaluatePosition(targetPos));
        }
    }
    IEnumerator MoveAiCoroutine()
    {
        bool temp = true;
        while (temp)
        {
            MoveAi();
            yield return new WaitForSeconds(checkIntervalSec);
        }
    }
}
