using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class RunnerRB : MonoBehaviour
{
    [SerializeField] GameObject disableRotationParent;
    GameObject charObj;
    Rigidbody rb;
    public float accelerationForce;
    Vector3 pathIndexPos;
    Vector3 finalTargetPos;
    Vector3 moveDirection;
    bool doAvoid = false;
    Vector3 avoidOffset;
    RaycastHit hit;

    [SerializeField] CinemachineSmoothPath path;
    public float targetPathIndex = 0;
    public float distanceIncrement = 1f;
    public float posCheckDistance = 2f;
    public float checkIntervalSec = 0.5f;

    public bool isAiControlled = true; //AI by default

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        charObj = GetComponentInChildren<Animator>().gameObject;
        //currentPath = innerPath;
    }
    private void Start()
    {
        disableRotationParent.transform.SetParent(null);
    }
    private void FixedUpdate()
    {
        if (isAiControlled)
            AiUpdateTarget();

        //Add force aimed at finalTargetPos to runner, then rotate runner to face velocity
        moveDirection = (finalTargetPos - transform.position).normalized;
        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        rb.AddForce(moveDirection * accelerationForce);

            //Debug.DrawRay(transform.position, rb.velocity, Color.cyan);        
        if (rb.velocity.magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized);
        disableRotationParent.transform.position = rb.worldCenterOfMass;
    }
    void AiUpdateTarget()
    {
        //Initialize target
        if (finalTargetPos == Vector3.zero )
        {
            targetPathIndex = path.FindClosestPoint(transform.position, 0, -1, 1);
            finalTargetPos = pathIndexPos = path.EvaluatePosition(targetPathIndex);
        }

        //Update target on distance check
        if ( (finalTargetPos - transform.position).sqrMagnitude < posCheckDistance * posCheckDistance)
        {
            if (Vector3.Dot(transform.forward, finalTargetPos - transform.position) >= 0)
            {
                //targetPathPos += distanceIncrement / currentPath.PathLength * currentPath.MaxPos;
                targetPathIndex += (rb.velocity.magnitude / path.PathLength * path.MaxPos);
                    if (targetPathIndex >= path.MaxPos) targetPathIndex -= path.MaxPos;

                finalTargetPos = pathIndexPos = path.EvaluatePosition(targetPathIndex);
            }
        }

        //Adjust avoidOffset over time, then add avoidOffset back to finalTargetPos
        if (doAvoid)
        {
            avoidOffset = Vector3.MoveTowards(avoidOffset, (transform.right * -1f ).normalized * 1.5f, 0.3f * Time.deltaTime);
        }
        else
        {
            avoidOffset = Vector3.MoveTowards(avoidOffset, Vector3.zero, 0.1f * Time.deltaTime);
        }
        Debug.Log(avoidOffset.magnitude);
        finalTargetPos = pathIndexPos + avoidOffset;

        #region experimentalPaths
        //if (currentPath == innerPath)
        //{
        //    //Debug.DrawLine(rb.worldCenterOfMass, rb.worldCenterOfMass + rb.velocity.normalized * 1, Color.cyan);
        //    if (Physics.SphereCast(rb.worldCenterOfMass, 0.5f, rb.velocity.normalized, out hit, 1, LayerMask.GetMask("Runner")))
        //    {
        //        Debug.Log("Switch to outerPath");
        //        currentPath = outerPath;
        //        moveToPos = currentPath.EvaluatePosition(targetPathPos);
        //    }
        //}
        //else if (currentPath == outerPath)
        //{
        //    //Debug.DrawLine(rb.worldCenterOfMass, rb.worldCenterOfMass + Quaternion.Euler(0, 30, 0) * rb.velocity, Color.cyan);
        //    //if (Physics.SphereCast(rb.worldCenterOfMass, 1, Quaternion.Euler(0, 30, 0) * rb.velocity.normalized, out hit, 2,  LayerMask.GetMask("Runner")))
        //    //{
        //    //    return;
        //    //}
        //    //Debug.DrawLine(rb.worldCenterOfMass, rb.worldCenterOfMass + Quaternion.Euler(0, 60, 0) * rb.velocity, Color.cyan);
        //    if (Physics.SphereCast(rb.worldCenterOfMass, 1, Quaternion.Euler(0, 60, 0) * rb.velocity.normalized, out hit, 2, LayerMask.GetMask("Runner")))
        //    {
        //        return;
        //    }
        //    //Debug.DrawLine(rb.worldCenterOfMass, rb.worldCenterOfMass + Quaternion.Euler(0, 90, 0) * rb.velocity, Color.cyan);
        //    if (Physics.SphereCast(rb.worldCenterOfMass, 1, Quaternion.Euler(0, 90, 0) * rb.velocity.normalized, out hit, 2, LayerMask.GetMask("Runner")))
        //    {
        //        return;
        //    }
        //    //Debug.DrawLine(rb.worldCenterOfMass, rb.worldCenterOfMass + Quaternion.Euler(0, 120, 0) * rb.velocity, Color.cyan);
        //    if (Physics.SphereCast(rb.worldCenterOfMass, 1, Quaternion.Euler(0, 120, 0) * rb.velocity.normalized, out hit, 2, LayerMask.GetMask("Runner")))
        //    {
        //        return;
        //    }

        //    Debug.Log("Switch to innerPath");
        //    currentPath = innerPath;
        //    moveToPos = currentPath.EvaluatePosition(targetPathPos);
        //}
        #endregion
    }

    private void OnCollisionStay(Collision collision)  //supposedly after fixedupdate
    {
        if (collision.transform.GetComponentInParent<RunnerRB>() == null) return;

        if (Vector3.Dot(transform.forward, collision.transform.position - transform.position) >= 0.5f)
        {
            doAvoid = true;
            Debug.DrawRay(transform.position, collision.transform.position - transform.position, Color.red);
        }
        else
        {
            doAvoid = false;
            Debug.DrawRay(transform.position, collision.transform.position - transform.position, Color.blue);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.GetComponentInParent<RunnerRB>() == null) return;

        doAvoid = false;
    }
    private void OnDrawGizmosSelected()
    {
        if (rb == null) return;
        Gizmos.DrawWireSphere(finalTargetPos, posCheckDistance);

        if (Vector3.Dot(transform.forward, finalTargetPos - transform.position) >= 0)
            Debug.DrawRay(transform.position, finalTargetPos - transform.position, Color.green);
        else
            Debug.DrawRay(transform.position, finalTargetPos - transform.position, Color.yellow);
    }
}
