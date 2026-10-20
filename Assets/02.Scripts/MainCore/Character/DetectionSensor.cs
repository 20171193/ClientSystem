using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class DetectionSensor : MonoBehaviour
{
    // 2단계 탐지
    // 1. 센서 반경(Trigger) 내 : 센서 활성화 (enableSensor = true)
    // 2. 시야각(내적 범위) 내 : 탐지 이벤트 발생 (OnDetected.Invoke)

    [Header("타겟 레이어 마스크")] public LayerMask targetMask;

    [Space(3),Header("\t\t센서 세팅")]
    [Header("센서 탐지 빈도")]
    public float detectFrequency = 0.5f;
    private float lastDetectedTime = 0f;

    [Header("센서 반지름")] 
    public float radius = 0f;
    [Header("시야각(정면기준)"), Range(0, 360f)] 
    public float fov = 0f;
    private float cosHalfFov = 0f;

    [Space(3), Header("\t\t기즈모 세팅")]
    [Header("테두리 타입")]
    public bool isWireFrame = true;
    [Header("정점 개수"), SerializeField]
    public int vertexCount = 20;
    [Header("색상"), SerializeField, ColorUsage(false, true)]
    public Color gizmoColor = Color.red;

    private SphereCollider sensorCol = null;
    private Collider sensedCol = null;  // 현재 센서에 들어온 콜라이더(탐지중x)

    public UnityAction<Collider> OnDetected;
    public UnityAction<Collider> OnLost;

    private bool enableSensor = false;
    private bool isDetected = false;

    // 현재 탐지 상태 (탐지 대상이 파괴된 경우 미탐지로 간주)
    public bool IsDetected => isDetected && sensedCol != null;
    public Transform DetectedTarget => IsDetected ? sensedCol.transform : null;

    private void Awake()
    {
        Init();
    }

    [ContextMenu("Init")]
    private void Init()
    {
        // cos 시야각 캐싱(내적에 필요)
        cosHalfFov = Mathf.Cos(fov * 0.5f * Mathf.Deg2Rad);

        // 위치 초기화
        transform.localPosition = Vector3.zero;

        // 트리거 세팅
        sensorCol = GetComponent<SphereCollider>();
        sensorCol.center = Vector3.zero;
        sensorCol.radius = radius;
        sensorCol.isTrigger = true;
    }

    private void Update()
    {
        // OnTrigger로 센서가 활성화 되었고, 탐지되지 않은 상태
        if(enableSensor && !isDetected && sensedCol != null)
        {   
            // detectFrequency 간격마다 탐지 체크
            if(Time.time - lastDetectedTime >= detectFrequency)
            {
                lastDetectedTime = Time.time;

                if(CheckDetection())
                {
                    isDetected = true;
                    OnDetected?.Invoke(sensedCol);
                }
                else
                {
                    isDetected = false;
                }   
            }
        }
    }

    // 시야 내 위치한 경우에만 이벤트 발생
    private bool CheckDetection()
    {
        if(sensedCol == null) return false;

        Vector3 fwd = transform.forward.OnlyMask(Extension.Axis.X | Extension.Axis.Z);
        Vector3 toward = (sensedCol.transform.position - transform.position).normalized.OnlyMask(Extension.Axis.X | Extension.Axis.Z);

        if (Vector3.Dot(fwd, toward) >= cosHalfFov)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 센서 활성화 상태가 아니고, 타겟 레이어에 포함된 경우
        if (targetMask.Contains(other.gameObject.layer) && !enableSensor)
        {
            // 센서 활성화
            sensedCol = other;
            enableSensor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ( other == sensedCol)
        {
            // 센서 비활성화
            sensedCol = null;
            enableSensor = false;

            if(isDetected)
            {
                OnLost?.Invoke(other);
                isDetected = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;

        float angleStep = fov / (vertexCount - 1);   // 정점 사이 각도

        Vector3 fwd = transform.forward.OnlyMask(Extension.Axis.X | Extension.Axis.Z).normalized;
        Vector3 origin = transform.position;

        Vector3 prevPoint = origin + Quaternion.Euler(0f, -fov * 0.5f, 0f) * fwd * radius;
        Gizmos.DrawLine(origin, prevPoint);

        for (int i = 1; i < vertexCount; i++)
        {
            float angle = -fov * 0.5f + angleStep * i;
            Vector3 point = origin + Quaternion.Euler(0f, angle, 0f) * fwd * radius;

            if(isWireFrame)
                Gizmos.DrawLine(prevPoint, point);
            else
                Gizmos.DrawLine(origin, point);

            prevPoint = point;
        }

        Gizmos.DrawLine(prevPoint, origin);
    }
}


