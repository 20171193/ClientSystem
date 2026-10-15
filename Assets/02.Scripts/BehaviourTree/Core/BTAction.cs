using System;

namespace ClientSystem.BTCore
{
    public class BTAction : BTNode
    {
        private readonly Func<Status> func;

        public BTAction(Func<Status> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func), "Action노드의 델리게이트가 null입니다.");

            this.func = func;
        }

        public override Status Tick()
        {
            return func.Invoke();
        }
    }
}
