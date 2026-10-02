// base node
public abstract class BTNode
{ 
    public enum Status { Failure = 0, Success, Running}
    public abstract Status Tick();
}