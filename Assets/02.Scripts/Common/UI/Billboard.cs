using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera mainCam;

    protected virtual void Awake()
    {
        mainCam = Camera.main;
    }

    protected virtual void LateUpdate()
    {
        transform.LookAt(mainCam.transform);
    }
}
