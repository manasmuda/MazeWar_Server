public class PathNode
{
    public int i;
    public int j;
    public int fValue;
    public int gValue;
    public PathNode prevNode;

    public PathNode(int i,int j,int gValue, int fValue,PathNode prevNode)
    {
        this.i = i;
        this.j = j;
        this.fValue = fValue;
        this.gValue = gValue;
        this.prevNode = prevNode;
    }

    
}

