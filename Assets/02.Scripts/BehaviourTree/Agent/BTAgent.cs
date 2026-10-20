using System.Collections.Generic;
using UnityEngine;
using ClientSystem.BTCore;
using Status = ClientSystem.BTCore.BTNode.Status;

[RequireComponent(typeof(CharacterController))]
public class BTAgent : MonoBehaviour
{
    [Header("코어 스텟"), Space(5)]
    [SerializeField] 
    private float moveSpeed;

    [Header("컴포넌트"), Space(5)]
    [SerializeField] 
    private CharacterController controller;
    [Header("센서"), SerializeField] 
    private DetectionSensor sensor;

    private BTNode btRoot = null;

    [Header("AI 설정"), Space(5)]
    [Header("순찰"), Space(3)]
    [Header("다음 목적지 이동 전 딜레이"), SerializeField] 
    private float patrolDelayTime = 1.5f;    
    private bool isPatrolArrived = false;
    private float patrolArrivedTime = 0f;

    [Header("웨이 포인트"), SerializeField] 
    private List<Transform> patrolWaypoints = new List<Transform>();
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
    [Header("목표를 놓친 경우 탐지 종료 유예시간(0f:곧장 종료)"), SerializeField] 
    private float traceExitTime = 0f;
    private float lastLostTime = float.NegativeInfinity;   // 놓친 적 없음
    Vector3 lastLostPosition = Vector3.zero;

    // 놓친 뒤 유예 시간 중
    private bool IsLosingPlayer => Time.time - lastLostTime < traceExitTime;

    // '유예 시간 중'은 '탐지 중'으로 간주
    public bool IsDetectPlayer => sensor.IsDetected || IsLosingPlayer;


    Vector3 curForward = Vector3.zero;  
    Vector3 destination = Vector3.zero;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();   

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
        sensor.OnLost += OnLostPlayer;
    }
    private void OnDisable()
    {
        sensor.OnLost -= OnLostPlayer;
    }
    private void Update()
    {
        if(btRoot != null)
            btRoot.Tick();
    }

    private void Move()
    {
        // TODO : 자연스러운 회전? 
        // 회전 
        curForward = (destination - transform.position).normalized;
        transform.forward = curForward;
        
        // 이동
        controller.Move(curForward * moveSpeed * Time.deltaTime);
    }

    // 플레이어 추격
    private Status TracePlayer()
    {
        if (!IsDetectPlayer) return Status.Failure;
        
        // 탐지 중 : 실시간 위치 / 유예 중 : 마지막으로 놓친 위치
        destination = sensor.IsDetected ? sensor.DetectedTarget.position : lastLostPosition;
        Move();

        return Status.Running;
    }

    private void OnLostPlayer(Collider col)
    {
        lastLostTime = Time.time;
        lastLostPosition = col.transform.position;
    }

    // 순찰(배회)
    private Status Patrol()
    {
        if (isPatrolArrived)
        {
            if(Time.time - patrolArrivedTime >= patrolDelayTime)
                isPatrolArrived = false;
            else 
                return Status.Running;
        }

        float sqm = (patrolWaypoints[PatrolIndex].position - transform.position).sqrMagnitude;
        if(sqm < 0.5 && !isPatrolArrived) 
        {
            isPatrolArrived = true;
            patrolArrivedTime = Time.time;
            PatrolIndex++;
        }

        destination = patrolWaypoints[PatrolIndex].position;
        Move();

        return Status.Running;
    }
}
