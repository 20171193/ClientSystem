namespace ClientSystem.BTCore
{
    // Sequence Node
    //  : Failure/Running 발견 시 바로 반환
    //  : 순차실행 (AND)
    public class BTSequence : BTComposite
    {
        // 모든 노드 순회
        // TODO : 이어보기
        public override Status Tick()
        {
            foreach(var c in children){
                var status = c.Tick();

                if (status == Status.Running)
                    return Status.Running;
                if (status == Status.Failure)
                    return Status.Failure;
            }
            return Status.Success;
        }
    }
}