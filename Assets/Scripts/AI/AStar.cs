using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Basic AStar class.

namespace RunningTeyze
{
    public  class AStarNode
    {
        public int g;
        public int h;
        public Vector2Int pos;
        public Vector2Int parent;
    }

    public struct TilemapAstarPoint
    {
        public Vector2 point;
        public bool jump;
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
        public AStar()
        {
            m_open = new Dictionary<Vector2Int, AStarNode>();
            m_closed = new Dictionary<Vector2Int, AStarNode>();
            m_path = new List<Vector2>();

        }

        public static ASTAR_DIRECTION GetOppositeDirection(ASTAR_DIRECTION direction)
        {
            ASTAR_DIRECTION result = 0x00;
            if((direction & ASTAR_DIRECTION.LEFT) == ASTAR_DIRECTION.LEFT)
            {
                result |= ASTAR_DIRECTION.RIGHT;
            }
            if ((direction & ASTAR_DIRECTION.BOTTOM) == ASTAR_DIRECTION.BOTTOM)
            {
                result |= ASTAR_DIRECTION.TOP;
            }
            if ((direction & ASTAR_DIRECTION.TOP) == ASTAR_DIRECTION.TOP)
            {
                result |= ASTAR_DIRECTION.BOTTOM;
            }
            if ((direction & ASTAR_DIRECTION.RIGHT) == ASTAR_DIRECTION.RIGHT)
            {
                result |= ASTAR_DIRECTION.LEFT;
            }

            return result;
        }

        protected virtual Vector2 GetRealWorldPos(Vector3Int position)
        {
            return Vector2.zero;
        }
        

        protected virtual Vector2Int GetAStarPos(Vector2 position)
        {
            return Vector2Int.zero;
        }

        

        protected virtual bool GetAstarBlock(Vector2Int pos, ASTAR_DIRECTION dir)
        {
            return false;
        }

        protected virtual AStarNode GetClosestNode(Vector2Int pos, ASTAR_DIRECTION dir)
        {
            return null;
        }

        protected virtual int GetXCoef(AStarNode node)
        {
            return 10;
        }


        protected virtual int GetYCoef(AStarNode node)
        {
            return 10;
        }

        protected virtual bool IsValid(AStarNode node)
        {
            return true;
        }


        public void Begin()
        {
            m_open.Clear();
            m_closed.Clear();
        }
       

        public Vector2[] GetPath(Vector2 start, Vector2 end)
        {
            if(!findPath(GetAStarPos(start), GetAStarPos(end)))return null;
            return m_path.ToArray();
        }

        protected bool makePath(Vector2Int start, Vector2Int target)
        {
            m_path.Clear();

            AStarNode n = m_closed[target];
            Vector3Int pos3 = new Vector3Int(n.pos.x, n.pos.y, 0);
            Vector2 realPos = GetRealWorldPos(pos3);

            m_path.Add(realPos);

            while (n.pos != start)
            {
                pos3 = new Vector3Int(n.parent.x, n.parent.y, 0);
                realPos = GetRealWorldPos(pos3);

                m_path.Add(realPos);
                if (m_closed.ContainsKey(n.parent))
                    n = m_closed[n.parent];
                else return start == target;
            }
            return true;
        }

        static List<GameObject> s_debugSprites;
        public void DrawDebugOpenClosed()
        {
            if (s_debugSprites == null) s_debugSprites = new List<GameObject>();
            for (int j = 0; j < s_debugSprites.Count; j++)
            {
                s_debugSprites[j].GetComponent<SpriteRenderer>().enabled = false;
            }

            int i= 0;
            foreach(KeyValuePair<Vector2Int, AStarNode> k in m_open)
            {
                Vector3Int v3 = new Vector3Int(k.Value.pos.x, k.Value.pos.y, 0);
                Vector2 pos = GetRealWorldPos(v3);

                SpriteRenderer sr = null;

                if (i < s_debugSprites.Count)
                {
                    sr = s_debugSprites[i++].GetComponent<SpriteRenderer>();
                    sr.enabled = true;
                    sr.transform.position = pos;
                }
                else
                {
                    sr = Utilities.InstantiateFromResources("Prefabs/Debug/SpriteTile", pos).GetComponent<SpriteRenderer>();
                    s_debugSprites.Add(sr.gameObject);
                }

                Color col = Color.green;
                
                col.b = (float)(k.Value.g + k.Value.h) / 100.0f;

                sr.color = col;
            }

            i = 0;
            foreach (KeyValuePair<Vector2Int, AStarNode> k in m_closed)
            {
                Vector3Int v3 = new Vector3Int(k.Value.pos.x, k.Value.pos.y, 0);
                Vector2 pos = GetRealWorldPos(v3);

                SpriteRenderer sr = null;

                if (i < s_debugSprites.Count)
                {
                    sr = s_debugSprites[i++].GetComponent<SpriteRenderer>();
                    sr.enabled = true;
                    sr.transform.position = pos;
                }
                else
                {
                    sr = Utilities.InstantiateFromResources("Prefabs/Debug/SpriteTile", pos).GetComponent<SpriteRenderer>();
                    s_debugSprites.Add(sr.gameObject);
                }

                Color col = Color.red;

                col.b = (float)(k.Value.g + k.Value.h) / 1000.0f;

                sr.color = col;
            }
        }

        protected bool findPath(Vector2Int start, Vector2Int target)
        {
            AStarNode n = new AStarNode();
            m_open.Clear();
            m_closed.Clear(); ;

            for (int attempt = 0; attempt < 1; attempt++)
            {
                n.pos = start;
                m_open.Add(start, n);


                for (int q = 0; q < 120; q++)
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
                        if (makePath(start, target))
                        {
                            return true;
                        }
                        else
                        {
                            clearNeighbours(m_closed[start]);
                            clearNeighbours(m_closed[target]);
                            break;
                        }
                    }

                    m_neighbours[0] = getNeighbour(n.pos, ASTAR_DIRECTION.TOP, target);
                    m_neighbours[1] = getNeighbour(n.pos, ASTAR_DIRECTION.RIGHT, target);
                    m_neighbours[2] = getNeighbour(n.pos, ASTAR_DIRECTION.LEFT, target);
                    m_neighbours[3] = getNeighbour(n.pos, ASTAR_DIRECTION.BOTTOM, target);

                    AStarNode minNode = null;
                    for (int i = 0; i < 4; i++)
                    {
                        if (m_neighbours[i] == null) continue;

                        if (IsValid(m_neighbours[i]))
                        {
                            if (minNode == null) minNode = m_neighbours[i];
                            F = m_neighbours[i].g + m_neighbours[i].h;
                            if (F < minF)
                            {
                                minF = F;
                                minNode = m_neighbours[i];
                                minNode.parent = n.pos;
                            }
                        }
                        else
                        {
                            m_open.Remove(m_neighbours[i].pos);
                            m_closed.Add(m_neighbours[i].pos, m_neighbours[i]);
                        }
                    }
                }
            }

            return false;
        }

        protected void clearNeighbours(AStarNode n)
        {
            Vector2Int l = Vector2Int.left + n.pos;
            Vector2Int r = Vector2Int.right + n.pos;
            Vector2Int b = Vector2Int.up + n.pos;
            Vector2Int t = Vector2Int.down + n.pos;

            m_open.Clear();
            if (m_closed.ContainsKey(n.pos))
            {
                m_closed.Remove(n.pos);
            }
            if (m_closed.ContainsKey(l))
            {
                m_closed.Remove(l);
            }
            if (m_closed.ContainsKey(r))
            {
                m_closed.Remove(r); ;
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


        protected  Vector2Int getOffset(ASTAR_DIRECTION dir)
        {
            Vector2Int offset = Vector2Int.zero;
            if ((dir & ASTAR_DIRECTION.RIGHT) == ASTAR_DIRECTION.RIGHT) offset.x = 1;
            else if ((dir & ASTAR_DIRECTION.LEFT) == ASTAR_DIRECTION.LEFT) offset.x = -1;

            if ((dir & ASTAR_DIRECTION.TOP) == ASTAR_DIRECTION.TOP) offset.y = 1;
            else if ((dir & ASTAR_DIRECTION.BOTTOM) == ASTAR_DIRECTION.BOTTOM) offset.y = -1;

            return offset;
        }

        protected AStarNode getNeighbour(Vector2Int start, ASTAR_DIRECTION dir, Vector2Int target)
        {
            Vector2Int offset = getOffset(dir);

            AStarNode node = null;

            if (GetAstarBlock(start, dir)) return node;
            else
            {
                node = GetClosestNode(start, dir);
                if (node == null) return null;
                if (!m_open.ContainsKey(node.pos))
                {
                    m_open[node.pos] = node;
                    node.parent = start;
                }
                
            }

            int xCoef = 10;
            int yCoef = 10;

            xCoef = GetXCoef(node);
            yCoef = GetYCoef(node);

            node.g = (int)((float)10 * offset.magnitude);
            node.h = Mathf.Abs((target.x - node.pos.x)) * xCoef + Mathf.Abs((target.y - node.pos.y)) * yCoef;
            node.h *= 10;

            return node;
        }

        protected AStarNode[] m_neighbours = new AStarNode[4];
        protected Dictionary<Vector2Int, AStarNode> m_open;
        protected Dictionary<Vector2Int, AStarNode> m_closed;
        protected List<Vector2> m_path;

    }

    public class TilemapAStar : AStar
    {
        Tilemap m_map;

        public Vector2 realStart;

        protected List<TilemapAstarPoint> m_tilemapPath;

        public TilemapAStar(Tilemap map) : base()
        {
            this.m_map = map;
            m_tilemapPath = new List<TilemapAstarPoint>();
        }

        protected override int GetYCoef(AStarNode node)
        {

            Vector2Int parentPos = node.parent;
            Vector2Int currentPos = node.pos;

            int yCoef = 10;

            for (int i = 0; i < 3; i++)
            {
                int diff = currentPos.y - parentPos.y;

                if (diff >= 1)
                    yCoef += 30 * i;
                else break;
                currentPos = parentPos;
                if (!m_closed.ContainsKey(parentPos)) break;
                parentPos = m_closed[parentPos].parent;
            }

            return yCoef;
        }

        public TilemapAstarPoint[] FindPath(Vector2 startF, Vector2 targetF, float jumpDistance)
        {
            int maxUpwardsTilesAllowed = (int)(jumpDistance / m_map.layoutGrid.cellSize.x);

            Vector3Int start3 = m_map.WorldToCell(startF);
            Vector3Int target3 = m_map.WorldToCell(targetF);

            Vector2Int start = new Vector2Int(start3.x, start3.y);
            Vector2Int target = new Vector2Int(target3.x, target3.y);

            AStarNode n = new AStarNode();
            m_open.Clear();
            m_closed.Clear();;

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
                        if (makePath(start, target))
                        {

                            if (!clearPath(jumpDistance)) return null;
                            return m_tilemapPath.ToArray();
                        }
                        else
                        {
                            clearNeighbours(m_closed[start]);
                            clearNeighbours(m_closed[target]);
                            break;
                        }
                    }

                    m_neighbours[0] = getNeighbour(n.pos, ASTAR_DIRECTION.TOP, target);
                    m_neighbours[1] = getNeighbour(n.pos, ASTAR_DIRECTION.RIGHT, target);
                    m_neighbours[2] = getNeighbour(n.pos, ASTAR_DIRECTION.LEFT, target);
                    m_neighbours[3] = getNeighbour(n.pos, ASTAR_DIRECTION.BOTTOM, target);

                    AStarNode minNode = null;
                    for (int i = 0; i < 4; i++)
                    {
                        if (m_neighbours[i] == null) continue;
                        if (IsValid(m_neighbours[i]))
                        {

                            if (minNode == null) minNode = m_neighbours[i];
                            F = m_neighbours[i].g + m_neighbours[i].h;
                            if (F < minF)
                            {
                                minF = F;
                                minNode = m_neighbours[i];
                                minNode.parent = n.pos;
                            }
                        }
                        else
                        {
                            m_open.Remove(m_neighbours[i].pos);
                            m_closed.Add(m_neighbours[i].pos, m_neighbours[i]);
                        }
                    }
                    
                }
            }

            return null;
        }
        
       


       
        bool clearPath(float jumpDistance)
        {
            int max = m_tilemapPath.Count - 1;
            m_path.Clear();
            for (int i = 0; i <m_tilemapPath.Count; i++)
            {
                if (i == 0 || i == max)
                {
                    realStart = m_path[max];
                    int layerMask = (1 << LayerMask.NameToLayer("Ground"));
                    TilemapAstarPoint p = new TilemapAstarPoint();
                    p.point = m_path[max - i];
                    p.point = Physics2D.Raycast(p.point, Vector2.down * 10.0f).point;
                    p.jump = false;
                    m_tilemapPath.Add(p);
                }
                else if (i > 0 && i < max)
                {
                    Vector2 a = m_path[max - i] - m_path[max - (i - 1)];
                    Vector2 b = m_path[max - (i + 1)] - m_path[max - i];

                    if (Mathf.Abs(Vector2.Dot(a, b)) < 0.01f)
                    {
                        TilemapAstarPoint p = new TilemapAstarPoint();
                        p.point = m_path[max - i];
                        
                        
                        if (m_tilemapPath.Count > 1)
                        {
                            int j = m_tilemapPath.Count - 1;
                            Vector2 v1 = m_tilemapPath[j].point - m_tilemapPath[j-1].point;
                            Vector2 v2 = p.point - m_tilemapPath[j].point;

                            float dot = Vector2.Dot(v2, Vector2.right);
                            if (Mathf.Approximately(dot, 0.0f))
                            {
                                p.jump = false;
                            }
                            else
                            {
                                int layerMask = (1 << LayerMask.NameToLayer("Ground"));
                                p.point = Physics2D.Raycast(p.point, Vector2.down, 5.0f, layerMask).point;
                                p.jump = true;
                            }
                        }

                        m_tilemapPath.Add(p);
                    }
                }
            }

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


        public void DrawDebug(Color color)
        {
            for (int i = 0; i < m_path.Count; i++)
            {
                if (m_path.Count == 1)
                    Debug.DrawRay(m_tilemapPath[0].point, Vector2.up, color);
                else if (i < m_path.Count - 1)
                    Debug.DrawLine(m_tilemapPath[i].point, m_tilemapPath[i + 1].point, color);
            }
            
        }

      
        
    }
}
