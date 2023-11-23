using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunnerControl : MonoBehaviour
{
    Runner runner;
    RunnerInput input;

    Camera cam;
    Vector3 pointerPos;


    private void Awake()
    {
        input = new RunnerInput();
    }
    private void Start()
    {
        cam = GameObject.FindObjectsByType<Camera>(FindObjectsSortMode.None)[0];
    }
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        if (runner == null) { return; }

        //Debug.DrawRay(cam.ScreenToWorldPoint(input.Player.Point.ReadValue<Vector2>()), cam.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(cam.ScreenToWorldPoint(input.Player.Point.ReadValue<Vector2>()), cam.transform.forward, out hit))
        {
            pointerPos = hit.point;
            Debug.DrawRay(pointerPos, Vector3.up);
        }

        if (input.Player.Tap.IsPressed())
        {
            runner.MoveToPoint(pointerPos);
        }
    }

    [ContextMenu("Assign Runner")]
    void TestAssignRunner()
    {
        runner = GameObject.FindObjectsByType<Runner>(FindObjectsSortMode.None)[0];
        runner.isAiControlled = false;
        Debug.Log("Runner assigned");
    }
    [ContextMenu("Release Runner")]
    void TestReleaseRunner()
    {
        runner.isAiControlled = true;
        runner = null;
        Debug.Log("Runner Released");
    }
}
