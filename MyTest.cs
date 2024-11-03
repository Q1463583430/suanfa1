
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTest : MonoBehaviour
{

    public Color normalColor, stopColor, pathColor, findColor, startColor, endColor;
    public int mapW = 10;
    public int mapH = 10;
    public float cubeOffset = 0.1f;//格子间距
    public int stopNum = 20;//随机阻挡数量
    public Vector2[] stops;//固定阻挡坐标
    
    private Dictionary<string, GameObject> goDic;
    private Vector2 mousePos = Vector2.one * -1;
    private List<PathNode> pathList;//寻路路径

    private bool setStart = false;

    private int step = 0;

    [SerializeField]
    private bool openStep;

  

    private Dictionary<int, FindPathBase> findPathDic;
    private FindPathBase curFindPath;
    public void SetDijkstraPathFinder()
    {
        // 创建一个Dijkstra对象并赋值给curFindPath  
        curFindPath = new DFS();
    }


    private const string MaterialColorProperty = "_Color";
    
    void Start()
    {
        
        MapManager.GetInstance().InitMap(mapW, mapH, stops, stopNum);
        SetDijkstraPathFinder();
        StartCoroutine(CreateCube());//开始协程

        if (mapW * mapH < 30)
        {
            transform.position = new Vector3(1, 45.5f, 2.2f);
            Camera.main.orthographicSize = 4;
        }
    }

    

    IEnumerator CreateCube()
    {
        goDic = new Dictionary<string, GameObject>();
        var nodes = MapManager.GetInstance().mapNodes;
        //创建格子
        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                if (j%6==0)
                {
                    yield return null;
                }
                
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = new Vector3(i + cubeOffset*i, 0, j + cubeOffset*j);
                var pos = i + "_" + j;
                go.name = pos;
                goDic.Add(go.name, go);

                //创建格子阻挡
                var node = nodes[i, j];
                if (node.type == ENodeType.Stop)
                {
                    go.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, stopColor);
                }
                else
                {
                    go.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, normalColor);
                }
                
                
            }
        }
       
    }

    public void ShowNextStep()
    {
        var list2 = curFindPath.findPathList;//之前已经有了
     
        if (step >= list2.Count)
        {
            var list = pathList;
            for (int i = 0; i < list.Count; i++)
            {
                var node = list[i];
                var goName = node.x + "_" + node.y;
                var pathGo = goDic[goName];
                pathGo.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, pathColor);
            }
            return;
        }
      
        //for (int i = 0; i < list2.Count; i++)
        {
            var node = list2[step];
            var goName = node.x + "_" + node.y;
            var pathGo = goDic[goName];
            pathGo.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, node.minNode ? Color.magenta : findColor);
        }
                        
        
        step++;
        print(step);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//鼠标点击
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 1000))
            {
                var go = raycastHit.collider.gameObject;
                string[] names = go.name.Split('_');
                go.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, startColor);
                int x = int.Parse(names[0]);
                int y = int.Parse(names[1]);
                var pos = new Vector2(x, y);
                if (!setStart)//第一次点击，设置为起点
                {
                    var list2 = curFindPath.findPathList;
                    if (list2.Count > 0)
                    {
                        ClearMouse();
                    }
                    mousePos = pos;
                    setStart = true;
                }
                else //第二次点击，进行寻路
                {
                    print(mousePos + ":" + pos);
                    var list = curFindPath.FindPath(mousePos, pos);
                    step = 0;
                    if (list != null)
                    {
                        if (!openStep)
                        {
                            var list2 = curFindPath.findPathList;//之前已经有了
                            for (int i = 0; i < list2.Count; i++)
                            {
                                var node = list2[i];
                                var goName = node.x + "_" + node.y;
                                var pathGo = goDic[goName];
                                pathGo.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, findColor);
                            }
                            
                            for (int i = 0; i < list.Count; i++)
                            {
                                var node = list[i];
                                var goName = node.x + "_" + node.y;
                                var pathGo = goDic[goName];
                                pathGo.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, pathColor);
                            }
                        }
                        
                        pathList = list;

                        var startGoName = mousePos.x + "_" + mousePos.y;
                        var startGo = goDic[startGoName];
                        startGo.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, startColor);
                        
                        go.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, endColor);
                        
                    }
                    mousePos = Vector2.one * -1;
                    setStart = false;
                }
            }
        }else if (Input.GetMouseButtonDown(1))
        {
            //clear
            ClearMouse();
        }else if (Input.GetMouseButtonDown(2))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 1000))
            {
                var go = raycastHit.collider.gameObject;
                string[] names = go.name.Split('_');
                int x = int.Parse(names[0]);
                int y = int.Parse(names[1]);
                var pos = new Vector2(x, y);
                var pathNode = MapManager.GetInstance().GetPathNode(x, y);
                var cost = pathNode.cost + 1;
                pathNode.cost = cost;
               
            }
        }
        else if (openStep && Input.GetKeyUp(KeyCode.Space))
        {
            ShowNextStep();//按下空格键显示路径
        }
    }

    private void ClearMouse()
    {
        if (setStart) //第一次点击，设置为起点
        {
            var go = goDic[mousePos.x + "_" + mousePos.y];
            go.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, normalColor);
            setStart = false;
        }

        if (pathList != null)
        {
            for (int i = 0; i < pathList.Count; i++)
            {
                var node = pathList[i];
                var goName = node.x + "_" + node.y;
                var pathGo = goDic[goName];//设置颜色
                pathGo.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, normalColor);
            }
            pathList = null;
        }
        
        var list2 = curFindPath.findPathList;
        for (int i = 0; i < list2.Count; i++)
        {
            var node = list2[i];
            var goName = node.x + "_" + node.y;
            var pathGo = goDic[goName];
            pathGo.GetComponent<MeshRenderer>().material.SetColor(MaterialColorProperty, normalColor);
        }
        list2.Clear();
    }
}
