
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager 
{
    
    //地图宽高
    private int mapW;
    private int mapH;
    
    //地图所有格子对象
    public PathNode[,] mapNodes;
    
    
    private static MapManager instance;
    public static MapManager GetInstance()
    {
        if (instance == null)
        {
            instance = new MapManager();
        }

        return instance;
    }
    
    
  

    public void InitMap(int w, int h, Vector2[] stops, int stopNum)
        //初始化地图
    {
        mapNodes = new PathNode[w, h];

        mapW = w;
        mapH = h;
        //创建格子
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                mapNodes[i, j] = new PathNode(i, j, ENodeType.Walk);

            }
        }

        //随机阻挡
        if (stopNum == 0)
        {
            for (int i = 0; i < stops.Length; i++)
            {
                int x = (int)stops[i].x;//强制转换
                int y = (int)stops[i].y;
                mapNodes[x, y].type = ENodeType.Stop;
            }
        }
        else
        {
            for (int i = 0; i < stopNum; i++)
            {
                int x = Random.Range(0, w);
                int y = Random.Range(0, h);
                mapNodes[x, y].type = ENodeType.Stop;
            }
        }
    }
    
   
    public bool IsMapExternal(int x, int y)
        //是否在地图之外
    {
        if (x < 0 || x >= mapW || y < 0 || y >= mapH )
        {
            return true;
        }

        return false;
    }

    public PathNode GetPathNode(int x, int y)
    {
        return mapNodes[x, y];
    }
}
