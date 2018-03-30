using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RunningTeyze
{
    public  class AStarNode
    {
        public int g;
        public int h;
        public Vector2Int pos;
        public Vector2Int parent;
    }

    public enum ASTAR_DIRECTION
    {
        LEFT = 0x01,
        RIGHT = 0x02,
        TOP = 0x04,
        BOTTOM = 0x08
    }

    public class AStar 
    {
        Tilemap m_map;
        AStarNode[] m_neighbours = new AStarNode[4];
        
        Dictionary<Vector2Int, AStarNode> m_open;
        
        Dictionary<Vector2Int, AStarNode> m_closed;
        List<Vector2> m_path;

        public AStar(Tilemap map)
        {

            this.m_map = map;
            m_open = new Dictionary<Vector2Int, AStarNode>();
            m_closed = new Dictionary<Vector2Int, AStarNode>();
            m_path = new List<Vector2>();
        }

        public void Begin()
        {
            m_open.Clear();
            m_closed.Clear();
            m_path.Clear();
        }

        public Vector2[] FindPath(Vector2 startF, Vector2 targetF)
        {

            Vector3Int start3 = m_map.WorldToCell(startF);
            Vector3Int target3 = m_map.WorldToCell(targetF);

            Vector2Int start = new Vector2Int(start3.x, start3.y);
            Vector2Int target = new Vector2Int(target3.x, target3.y);

            AStarNode n = new AStarNode();
            m_open.Clear();
            m_closed.Clear();
            m_path.Clear();
            n.pos = start;
            m_open.Add(start, n);
            

            for(int q= 0; q<100;q++)
            {
                int F = 100000000;
                int minF = F;
                foreach (KeyValuePair<Vector2Int,AStarNode> k in m_open)
                {
                    AStarNode o = k.Value;
                    F = o.g + o.h;
                    if (F < minF)
                    {
                        minF = F;
                        n = o;
                    }
                }
                m_open.Remove(n.pos);
                if(!m_closed.ContainsKey(n.pos)) 
                m_closed.Add(n.pos,n);

                if (n.pos == target)
                    return makePath(start, target);

                m_neighbours[0] = getNeighbour(n.pos, ASTAR_DIRECTION.TOP, target);
                m_neighbours[1] = getNeighbour(n.pos, ASTAR_DIRECTION.RIGHT, target);
                m_neighbours[2] = getNeighbour(n.pos, ASTAR_DIRECTION.LEFT, target);
                m_neighbours[3] = getNeighbour(n.pos, ASTAR_DIRECTION.BOTTOM, target);

                AStarNode minNode = null;
                for (int i = 0; i < 4; i++)
                {
                    if (m_neighbours[i] == null) continue;

                    if (minNode == null) minNode = m_neighbours[i];
                    F = m_neighbours[i].g + m_neighbours[i].h;
                    if (F < minF)
                    {
                        minF = F;
                        minNode = m_neighbours[i];
                        minNode.parent = n.pos;
                    }
                }

            }

            return null;
        }

        Vector2[] makePath(Vector2Int start, Vector2Int target)
        {
            AStarNode n = m_closed[target];
            Vector3Int pos3 = new Vector3Int(n.pos.x, n.pos.y, 0);
            m_path.Add(m_map.GetCellCenterWorld(pos3));
            while(n.pos != start)
            {
                pos3 = new Vector3Int(n.parent.x, n.parent.y, 0);
                m_path.Add(m_map.GetCellCenterWorld(pos3));
                n = m_closed[n.parent];
            }
            return m_path.ToArray();
        }

      
        AStarNode getNeighbour(Vector2Int start, ASTAR_DIRECTION dir, Vector2Int target)
        {
            Vector2Int offset = Vector2Int.zero;
            if ((dir & ASTAR_DIRECTION.RIGHT) == ASTAR_DIRECTION.RIGHT) offset.x = 1;
            else if ((dir & ASTAR_DIRECTION.LEFT) == ASTAR_DIRECTION.LEFT) offset.x = -1;

            if ((dir & ASTAR_DIRECTION.TOP) == ASTAR_DIRECTION.TOP) offset.y = 1;
            else if ((dir & ASTAR_DIRECTION.BOTTOM) == ASTAR_DIRECTION.BOTTOM) offset.y = -1;

            Vector2Int key = start + offset;
            AStarNode node = null;
            if (m_closed.ContainsKey(key)) return node;
            if (m_map.HasTile(new Vector3Int(key.x, key.y, 0))) return node;
            if (m_open.ContainsKey(key)) node = m_open[key];
            else
            {
                node = new AStarNode();
                node.pos = key;
                node.parent = start;
                m_open[key] = node;
            }

            node.g = (int)(offset.magnitude * 10);
            node.h = Mathf.Abs((target.x - key.x)) + Mathf.Abs((target.y - key.y));
            node.h *= 10;

            return node;
        }
    }
}
