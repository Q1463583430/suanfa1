/**
 * @author B站：码农小徐
 * @url https://space.bilibili.com/304648178
 * @version 1.0
 */

public enum ENodeType  {
    Walk,
    Stop
}

public class PathNode
{
    
    //坐标
    public int x;
    public int y;
    
    
    //寻路消耗
    public float f;
    //离起点的距离
    public float g;
    //离终点的距离
    public float h;
    //父对象
    public PathNode father;
    //格子类型
    public ENodeType type;
    
    //寻路权重值
    public float cost;

    public bool minNode = false;

    public PathNode(int x, int y, ENodeType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }


    public void Clear()
    {
        f = 0;
        g = 0;
        h = 0;
        father = null;
        minNode = false;
    }

}
