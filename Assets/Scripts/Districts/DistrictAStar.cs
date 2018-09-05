using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RunningTeyze
{
    public class DistrictAStar : AStar
    {
        private Tilemap m_map;
        private Dictionary<Vector2Int, DistrictNavigationItem> m_nodes;

        public DistrictAStar(Tilemap map)
        {
            m_map = map;
            m_nodes = new Dictionary<Vector2Int, DistrictNavigationItem>();
        }

        public DistrictAStar() : base()
        {

        }

        public void AddDistrictNavigationItem(DistrictNavigationItem item)
        {
            Vector2Int key = GetAStarPos(item.transform.position);
            if (m_nodes.ContainsKey(key)) return;
            m_nodes[key] = item;
        }

        protected override Vector2 GetRealWorldPos(Vector3Int position)
        {
            return m_map.GetCellCenterWorld(position);
        }


        protected override Vector2Int GetAStarPos(Vector2 position)
        {
            Vector3Int mapPos = m_map.WorldToCell(position);
            return new Vector2Int(mapPos.x,mapPos.y);
        }

        protected override bool GetAstarBlock(Vector2Int pos, ASTAR_DIRECTION dir)
        { 
            if(m_nodes.ContainsKey(pos))
            {
                DistrictNavigationItem n = m_nodes[pos];
                switch(dir)
                {
                    case ASTAR_DIRECTION.BOTTOM:
                        return n.bottom == null;
                    case ASTAR_DIRECTION.TOP:
                        return n.top == null;
                    case ASTAR_DIRECTION.LEFT:
                        return n.left == null;
                    case ASTAR_DIRECTION.RIGHT:
                        return n.right == null;
                }
            }

            return false;
        }

        protected override bool IsValid(AStarNode node)
        {
            if (!m_nodes.ContainsKey(node.pos)) return false;

            if (m_nodes[node.pos].isCity) return m_nodes[node.pos].neighborhood.isUnlocked;
            return true;
        }

        protected override AStarNode GetClosestNode(Vector2Int pos, ASTAR_DIRECTION dir)
        {
            
            if (m_nodes.ContainsKey(pos))
            {
                DistrictNavigationItem item = m_nodes[pos];
                Vector3Int v3 = Vector3Int.zero;

                switch (dir)
                {
                    case ASTAR_DIRECTION.BOTTOM:
                            v3 = m_map.WorldToCell(item.bottom.transform.position);
                        break;
                    case ASTAR_DIRECTION.TOP:
                        v3 = m_map.WorldToCell(item.top.transform.position);
                        break;
                    case ASTAR_DIRECTION.LEFT:
                        v3 = m_map.WorldToCell(item.left.transform.position);
                        break;
                    case ASTAR_DIRECTION.RIGHT:
                        v3 = m_map.WorldToCell(item.right.transform.position);
                        break;

                }

                Vector2Int v2 = new Vector2Int(v3.x, v3.y);

                if (m_closed.ContainsKey(v2)) return null;
                if (m_open.ContainsKey(v2)) return m_open[v2];

                AStarNode n = new AStarNode();
                n.pos = v2;                
                
                return n;
            }

            return null;
        }

        protected override int GetXCoef(AStarNode node)
        {
            return 10;
        }


        protected override int GetYCoef(AStarNode node)
        {
            return 10;
        }
    }
}
