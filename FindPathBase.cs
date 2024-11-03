

using System.Collections.Generic;
using UnityEngine;

public abstract class FindPathBase//定义一个基类
{
    //开启列表
    protected List<PathNode> openList = new List<PathNode>();

    public List<PathNode> findPathList = new List<PathNode>();

    //关闭列表
    protected List<PathNode> closeList = new List<PathNode>();


    public PathNode[,] MapNodes
    {
        get { return MapManager.GetInstance().mapNodes; }
    }


    public abstract  List<PathNode> FindPath(Vector2 startPos, Vector2 endPos);

    protected bool IsMapExternal(int x, int y)
        //是否在地图之外
    {
        return MapManager.GetInstance().IsMapExternal(x, y);
    }
}
