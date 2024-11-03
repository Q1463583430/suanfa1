using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class DFS : FindPathBase // DFS��Ϊ����
{
    // Ѱ·
    public override List<PathNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        int sx = Mathf.FloorToInt(startPos.x);
        int sy = Mathf.FloorToInt(startPos.y);
        int ex = Mathf.FloorToInt(endPos.x);
        int ey = Mathf.FloorToInt(endPos.y);

        // �Ƿ�Խ��
        if (IsMapExternal(sx, sy) || IsMapExternal(ex, ey))
        {
            Debug.LogError("�����յ��ڵ�ͼ��");
            return null;
        }

        // �Ƿ��赲
        var startNode = MapNodes[sx, sy];
        var endNode = MapNodes[ex, ey];
        if (startNode.type == ENodeType.Stop || endNode.type == ENodeType.Stop)
        {
            Debug.LogError("�����յ����赲");
            return null;
        }

        // �����һ������
        openList.Clear();
        findPathList.Clear();
        closeList.Clear();
        startNode.Clear();

        Stack<PathNode> stack = new Stack<PathNode>();
        stack.Push(startNode);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            closeList.Add(currentNode);

            if (currentNode.x == ex && currentNode.y == ey)
            {
                var pathList = ReconstructPath(currentNode);
                return pathList;
            }

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!closeList.Contains(neighbor) && neighbor.type != ENodeType.Stop)
                {
                    stack.Push(neighbor);
                    neighbor.father = currentNode;
                }
            }
        }

        Debug.LogError("û���ҵ�·��");
        return null;
    }

    private List<PathNode> ReconstructPath(PathNode endNode)
    {
        var pathList = new List<PathNode>();
        PathNode currentNode = endNode;
        while (currentNode != null)
        {
            pathList.Add(currentNode);
            currentNode = currentNode.father;
        }
        pathList.Reverse();
        return pathList;
    }

    private IEnumerable<PathNode> GetNeighbors(PathNode node)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int nx = node.x + i;
                int ny = node.y + j;

                if (!IsMapExternal(nx, ny))
                {
                    var neighbor = MapNodes[nx, ny];
                    yield return neighbor;
                }
            }
        }
    }
}
