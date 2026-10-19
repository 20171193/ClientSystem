using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using ClientSystem.BTCore;
using Status = ClientSystem.BTCore.BTNode.Status;

public class BTAgent : MonoBehaviour
{
    [Header("코어 스텟"), Space(5)]
    [SerializeField] float moveSpeed;

    [Header("컴포넌트"), Space(5)]
    [SerializeField] CharacterController controller;
    [Header("센서"), SerializeField] DetectionSensor sensor;

    private BTNode btRoot = null;

    [Header("AI 설정"), Space(5)]
    [Header("순찰"), Space(3)]
    [Header("웨이 포인트"), SerializeField] List<Transform> patrolWaypoints = new List<Transform>();
    private int curPatrolIndex = 0;
    private int PatrolIndex{ 
        get { return curPatrolIndex; } 
        set 
        {
            if (value >= patrolWaypoints.Count)
                curPatrolIndex = 0;
            else
                curPatrolIndex = value;
        }
    }

    [Header("탐지/추격"), Space(3)]
    [Header("재추격 유예시간"), SerializeField] float traceDelayTime = 0f;
    public bool IsDetectPlayer {get { return playerTr != null; } }

    Transform playerTr = null;
    Collider[] dectectResults;

    Vector3 moveDir;
    Vector3 destination;
    
    private void Awake()
    {
        // BT빌드 후 루트 할당
        btRoot = new BTBuilder()
            .Selector()
                .Sequence()
                    .Condition(() => IsDetectPlayer)
                    .Action(() => TracePlayer())
                .End()
                .Action(() => Patrol())
            .End()
            .Build();
    }

    private void OnEnable()
    {
        sensor.OnDetected += OnDetectedPlayer;
        sensor.OnLost += OnLostPlayer;
    }

    private void OnDisable()
    {
        sensor.OnDetected -= OnDetectedPlayer;
        sensor.OnLost -= OnLostPlayer;
    }

    private void Update()
    {
        if(btRoot != null)
            btRoot.Tick();
    }

    private void Move()
    {
        moveDir = (destination - transform.position).normalized;
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    // 플레이어 추격
    private Status TracePlayer()
    {
        if (playerTr == null) return Status.Failure;
        
        destination = playerTr.position;
        Move();

        return Status.Running;
    }

    private void OnDetectedPlayer(Collider col)
    {
        //TODO : 재할당 방지 딜레이, 최적화 
        playerTr = col.transform;
    }
    private void OnLostPlayer(Collider col)
    {
        playerTr = null;
        //TODO : Detect - Lost 간 유예시간 추가
    }


    // 순찰(배회)
    private Status Patrol()
    {
        float sqm = (patrolWaypoints[PatrolIndex].position - transform.position).sqrMagnitude;
        if(sqm < 0.02) PatrolIndex++;

        destination = patrolWaypoints[PatrolIndex].position;
        Move();

        return Status.Running;
    }
}
