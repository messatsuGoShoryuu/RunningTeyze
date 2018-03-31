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
        List<Vector2> m_rawPath;

        public AStar(Tilemap map)
        {

            this.m_map = map;
            m_open = new Dictionary<Vector2Int, AStarNode>();
            m_closed = new Dictionary<Vector2Int, AStarNode>();
            m_path = new List<Vector2>();
            m_rawPath = new List<Vector2>();
        }

        public void Begin()
        {
            m_open.Clear();
            m_closed.Clear();
            m_path.Clear();
        }

        public Vector2[] FindPath(Vector2 startF, Vector2 targetF, float jumpDistance)
        {
            int maxUpwardsTilesAllowed = (int)(jumpDistance / m_map.layoutGrid.cellSize.x);

            Vector3Int start3 = m_map.WorldToCell(startF);
            Vector3Int target3 = m_map.WorldToCell(targetF);

            Vector2Int start = new Vector2Int(start3.x, start3.y);
            Vector2Int target = new Vector2Int(target3.x, target3.y);

            AStarNode n = new AStarNode();
            m_open.Clear();
            m_closed.Clear();
            m_path.Clear();

            for (int attempt = 0; attempt < 1; attempt++)
            {
                n.pos = start;
                m_open.Add(start, n);


                for (int q = 0; q < 30; q++)
                {
                    int F = 100000000;
                    int minF = F;
                    foreach (KeyValuePair<Vector2Int, AStarNode> k in m_open)
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
                    if (!m_closed.ContainsKey(n.pos))
                        m_closed.Add(n.pos, n);

                    if (n.pos == target)
                    {
                        Vector2[] result =  makePath(start, target,jumpDistance);
                        if (result == null)
                        {
                            clearNeighbours(m_closed[start]);
                            clearNeighbours(m_closed[target]);
                            break;
                        }
                        else return result;
                    }

                    int penalty = getDirectionPenalty(n, ASTAR_DIRECTION.TOP, maxUpwardsTilesAllowed);

                    m_neighbours[0] = getNeighbour(n.pos, ASTAR_DIRECTION.TOP, target, 0);
                    penalty = getDirectionPenalty(n, ASTAR_DIRECTION.RIGHT, 0);
                    m_neighbours[1] = getNeighbour(n.pos, ASTAR_DIRECTION.RIGHT, target, 0);
                    penalty = getDirectionPenalty(n, ASTAR_DIRECTION.LEFT, 0);
                    m_neighbours[2] = getNeighbour(n.pos, ASTAR_DIRECTION.LEFT, target, 0);
                    m_neighbours[3] = getNeighbour(n.pos, ASTAR_DIRECTION.BOTTOM, target, 0);

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
            }

            return null;
        }
        
        void clearNeighbours(AStarNode n)
        {
            Vector2Int l = Vector2Int.left + n.pos;
            Vector2Int r = Vector2Int.right + n.pos;
            Vector2Int b = Vector2Int.up + n.pos;
            Vector2Int t = Vector2Int.down + n.pos;

            m_open.Clear();
            if(m_closed.ContainsKey(n.pos))
            {
                m_closed.Remove(n.pos);
            }
            if(m_closed.ContainsKey(l))
            {
                m_closed.Remove(l);
            }
            if (m_closed.ContainsKey(r))
            {
                m_closed.Remove(r);;
            }
            if (m_closed.ContainsKey(b))
            {
                m_closed.Remove(b);
            }
            if (m_closed.ContainsKey(t))
            {
                m_closed.Remove(t);
            }
        }
        Vector2[] makePath(Vector2Int start, Vector2Int target, float jumpDistance)
        {
            m_rawPath.Clear();
            m_path.Clear();
            AStarNode n = m_closed[target];
            Vector3Int pos3 = new Vector3Int(n.pos.x, n.pos.y, 0);
            Vector2 realPos = m_map.GetCellCenterWorld(pos3);

            m_rawPath.Add(realPos);

            int layerMask = (1 << LayerMask.NameToLayer("Ground"));
            
            while(n.pos != start)
            {
                pos3 = new Vector3Int(n.parent.x, n.parent.y, 0);
                realPos = m_map.GetCellCenterWorld(pos3);

                m_rawPath.Add(realPos);
                if (m_closed.ContainsKey(n.parent))
                    n = m_closed[n.parent];
                else return null;
            }

            if(!clearPath(jumpDistance)) return null;
            return m_path.ToArray();
        }

        bool clearPath(float jumpDistance)
        {
            int max = m_rawPath.Count - 1;
            for (int i = 0; i <m_rawPath.Count; i++)
            {
                if (i == 0 || i == max)
                    m_path.Add(m_rawPath[max - i]);
                else if (i > 0 && i < max)
                {
                    Vector2 a = m_rawPath[max - i] - m_rawPath[max - (i - 1)];
                    Vector2 b = m_rawPath[max - (i + 1)] - m_rawPath[max - i];

                    if (Mathf.Abs(Vector2.Dot(a, b)) < 0.01f)
                    {
                        m_path.Add(m_rawPath[max - i]);
                        if(m_path.Count > 2)
                        {
                            int j = m_path.Count - 1;
                            Vector2 v1 = m_path[j - 1] - m_path[j - 2];
                            Vector2 v2 = m_path[j] - m_path[j - 1];

                            float dot = Vector2.Dot(v2, Vector2.right);
                            if (Mathf.Approximately(dot, 0.0f))
                            {
                                /*
                                if(v2.y > 0.0f && v2.y > jumpDistance)
                                {
                                    drawDebug(Color.red);
                                    m_rawPath.Clear();
                                    return false;
                                }
                                */
                            }
                            else
                            {
                                int layerMask = (1 << LayerMask.NameToLayer("Ground"));
                                m_path[j] = Physics2D.Raycast(m_path[j], Vector2.down, 5.0f, layerMask).point;
                            }
                        }
                    }
                }
            }
            drawDebug(Color.green);
            return true;
        }

        int getDirectionPenalty(AStarNode n, ASTAR_DIRECTION dir, int maxUpwards)
        {
            int penalty = 0;
            Vector2Int speculative = n.pos + getOffset(dir);
            AStarNode current = n;
            for(int i = 0; i<5;i++)
            {
                
                if (speculative.y - current.parent.y >= 1)
                {
                    int costMultiplier = 0;
                    Vector3Int pos = new Vector3Int(n.parent.x, n.parent.y, 0);
                    while (!m_map.HasTile(pos))
                    {
                        pos.y--;
                        costMultiplier++;
                    }

                    penalty += 8 * costMultiplier;
                }
                
                if (!m_open.ContainsKey(n.parent)) return penalty;
                current = m_open[current.parent];
            }

            return penalty;
        }

        Vector2Int getOffset(ASTAR_DIRECTION dir)
        {
            Vector2Int offset = Vector2Int.zero;
            if ((dir & ASTAR_DIRECTION.RIGHT) == ASTAR_DIRECTION.RIGHT) offset.x = 1;
            else if ((dir & ASTAR_DIRECTION.LEFT) == ASTAR_DIRECTION.LEFT) offset.x = -1;

            if ((dir & ASTAR_DIRECTION.TOP) == ASTAR_DIRECTION.TOP) offset.y = 1;
            else if ((dir & ASTAR_DIRECTION.BOTTOM) == ASTAR_DIRECTION.BOTTOM) offset.y = -1;

            return offset;
        }

        void drawDebug(Color color)
        {
            for (int i = 0; i < m_path.Count; i++)
            {
                if (m_path.Count == 1)
                    Debug.DrawRay(m_path[0], Vector2.up, color,3.0f);
                else if (i < m_path.Count - 1)
                    Debug.DrawLine(m_path[i], m_path[i + 1], color, 3.0f);
            }
        }

      
        AStarNode getNeighbour(Vector2Int start, ASTAR_DIRECTION dir, Vector2Int target, int directionPenalty)
        {
            Vector2Int offset = getOffset(dir);

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

            int yCoef = 10;

            {
                Vector2Int parentPos = node.parent;
                Vector2Int currentPos = node.pos;

                for (int i = 0; i < 3; i++)
                {
                    int diff = currentPos.y - parentPos.y;

                    if (diff >= 1)
                        yCoef += 30 *  i;
                    else break;
                    currentPos = parentPos;
                    if (!m_closed.ContainsKey(parentPos)) break;
                        parentPos = m_closed[parentPos].parent; 
                }
                    
            }
            
            node.g = (int)(offset.magnitude * 10);
            node.h = Mathf.Abs((target.x - key.x)) * 10 + Mathf.Abs((target.y - key.y)) * yCoef + directionPenalty;
            node.h *= 10;

            return node;
        }
    }
}
