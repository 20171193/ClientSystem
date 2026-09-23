# Behavior Tree

## 목표 / 범위

- 1차 목표: 런타임 코어 + 간단한 시각화(로그, 빌보드 UI)
- 노드 에디터(GraphView)는 시간 대비 효과를 고려해 **추가 개발 요소**로 남겨둠 (아래 백로그 참고)

## 설계 결정

- [ ] 트리 정의와 실행 상태 분리 여부 (Flyweight — 에이전트 여러 마리가 트리 하나를 공유)
- [ ] Running 노드 재진입 방식: Reactive(매 틱 루트부터 재평가) vs Memory(이어서 진행)
- [ ] 중단(Abort) 처리: 우선순위 높은 조건 충족 시 실행 중인 Running 노드를 끊고 정리(OnAbort/OnExit)
- [ ] 블랙보드 구조: 타입 안전 키 기반으로 결정 (Dictionary<string, object> 지양)
- [ ] 트리 구성 방법: C# Fluent Builder 사용 (SO/직렬화는 에디터 툴 단계에서 검토)
- [ ] Tick 주기: 매 프레임 vs 일정 간격

## 데모 시나리오

경비병 AI 하나로 주요 기능을 모두 보여주는 구성:

```
Selector (Reactive)
├─ Sequence [HP 낮음]     → 도주 → 회복 대기
├─ Sequence [플레이어 인지] → 추격 → (사거리 내) 공격 [Cooldown]
├─ Sequence [마지막 목격 위치 있음] → 이동 → 주변 수색 [Repeater] → 기억 삭제
└─ 순찰 (웨이포인트)
```

- 적 2~3마리가 트리 하나를 공유 (Flyweight 검증)
- HP 깎기 등 디버그 조작으로 Abort 전환을 직접 유발

## 마일스톤

- [ ] 코어 뼈대: `Node`(abstract), `Status` enum, `Composite`(Sequence/Selector), `Decorator`(Inverter/Repeater/Cooldown), `Leaf`(Action/Condition), Fluent Builder
- [ ] 블랙보드: 타입 안전 키 기반 컨텍스트, 에이전트별 실행 상태 분리
- [ ] EditMode 테스트(NUnit): 가짜 Leaf로 Sequence/Selector/Decorator/Abort 동작 검증
- [ ] 데모 씬: 경비병 AI (순찰 → 인지/추격 → 공격 → 수색), NavMesh 이동
- [ ] 시각화: 빌보드 UI(현재 노드/상태 텍스트) + 콘솔 로그
- [ ] WebGL 빌드 & 검증

## 추가 개발 요소 (백로그)

- [ ] 노드 에디터 (GraphView 기반 비주얼 에디터, 저장/로드)
