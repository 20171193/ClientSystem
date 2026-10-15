using System;

namespace ClientSystem.BTCore
{
    // Condition 내부에서 상태를 바꾸면 안됨
    public class BTCondition : BTNode
    {
        private readonly Func<bool> func;

        public BTCondition(Func<bool> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func), "Condition노드의 델리게이트가 null입니다.");

            this.func = func;
        }

        public override Status Tick()
        {
            return func.Invoke() ? Status.Success : Status.Failure;
        }
    }
}
