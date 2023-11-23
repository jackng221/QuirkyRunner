using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPath : MonoBehaviour
{
    public float pos = 0;
    CinemachineSmoothPath path;

    private void Awake()
    {
        path = GetComponent<CinemachineSmoothPath>();
    }
    private void Update()
    {
        Debug.DrawRay(path.EvaluatePosition(pos), Vector2.up);
        //pos += 1 * Time.deltaTime;
        //if (pos >= path.MaxPos) pos = 0;
    }
}
