using System.Collections.Generic;

namespace ClientSystem.BTCore
{
    public abstract class BTComposite : BTNode
    {
        protected List<BTNode> children = new List<BTNode>();

        public void Add(BTNode node)
        {
            children.Add(node);
        }
    }
}