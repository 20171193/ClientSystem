using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class DetectionSensor : MonoBehaviour
{
    [Header("타겟 레이어 마스크")] public LayerMask targetMask;
    
    [Header("센서 세팅"), Space(5)]
    [Header("센서 반지름")] public float radius = 0f;
    [Header("시야각(정면기준)"), Range(0, 360f)] public float fov = 0f;
    private float cosHalfFov = 0f;
    
    public UnityAction<Collider> OnDetected;
    public UnityAction<Collider> OnLost;

    private void Awake()
    {
        cosHalfFov = Mathf.Cos(fov * 0.5f * Mathf.Deg2Rad);
    }

    private void CheckDetection(Collider other)
    {
        Vector3 fwd = transform.forward.OnlyMask(Extension.Axis.X | Extension.Axis.Z);
        Vector3 toward = (other.transform.position - transform.position).normalized.OnlyMask(Extension.Axis.X | Extension.Axis.Z);

        if(Vector3.Dot(fwd, toward) >= cosHalfFov)
        {
            OnDetected?.Invoke(other);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetMask.Contains(other.gameObject.layer))
        {
            CheckDetection(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (targetMask.Contains(other.gameObject.layer))
        {
            OnLost?.Invoke(other);
        }
    }
}
