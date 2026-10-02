// Selector Node
//  : Success/Running 발견 시 바로 반환
public class BTSelector : BTComposite
{
    

    public override Status Tick()
    {
        // 마지막 Running 노드부터 순회
        for (int i = lastRunningIndex; i < children.Count; i++)
        {
            var status = children[i].Tick();

            if (status == Status.Running)
            {
                lastRunningIndex = i;
                return Status.Running;
            }

            if (status == Status.Success)
            {
                lastRunningIndex = 0;
                return Status.Success;
            }
        }
        lastRunningIndex = 0;
        return Status.Failure;
    }
}
