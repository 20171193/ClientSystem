
using UnityEngine.Video;

namespace ClientSystem.BTCore
{
    // Selector Node
    //  : Success/Running 발견 시 바로 반환
    //  : 우선순위대로 실행 (OR)
    public class BTSelector : BTComposite
    {
        public override Status Tick()
        {
            // 모든 노드 순회
            // TODO : 이어보기
            foreach (var c in children)
            {
                var status = c.Tick();

                if (status == Status.Running)
                    return Status.Running;
                if (status == Status.Success)
                    return Status.Success;
            }
            return Status.Failure;
        }
    }
}