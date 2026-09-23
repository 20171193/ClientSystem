using System.Collections.Generic;

public abstract class BTComposite : BTNode
{
    protected int lastRunningIndex = 0;
    public int LastRunningIndex { get { return lastRunningIndex; } }

    protected List<BTNode> children = new List<BTNode>();
}
