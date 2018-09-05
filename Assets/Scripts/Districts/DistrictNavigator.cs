using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RunningTeyze
{

    public class DistrictNavigator : MonoBehaviour
    {
        const string L_T = "Districts_11";
        const string L_B = "Districts_2";
        const string L_R_T = "Districts_16";
        const string L_R_B = "Districts_12";
        const string L_T_B = "Districts_3";
        const string L_R_T_B = "Districts_15";
        const string R_T = "Districts_9";
        const string R_B = "Districts_0";
        const string R_T_B = "Districts_7";
        const string CITY = "Districts_8";
        public Tilemap map;
        public Tilemap neighborhoodMap;


        [SerializeField]
        private DistrictNeighborhood[] m_neighborhoods;
        public static DistrictNeighborhood[] neighborhoods;

        public Transform startingPoint;

        List<DistrictNavigationItem> m_items;
        Queue<DistrictNavigationItem> m_itemQueue;

        Dictionary<Vector3Int,DistrictNavigationItem> m_filledPoints;
        Dictionary<Vector3Int, DistrictNavigationItem> m_filledNeighborhoods;

        private void Awake()
        {
            neighborhoods = m_neighborhoods;
        }
        // Use this for initialization
        void Start()
        {
            m_items = new List<DistrictNavigationItem>();
            m_itemQueue = new Queue<DistrictNavigationItem>();
            m_filledPoints = new Dictionary<Vector3Int, DistrictNavigationItem>();
            m_filledNeighborhoods = new Dictionary<Vector3Int, DistrictNavigationItem>();
            generateMap();
        }


        void checkItem(DistrictNavigationItem parent, DistrictNavigationItem child, ASTAR_DIRECTION direction, bool enqueue = true)
        {
            if (child)
            {
                switch (direction)
                {
                    case ASTAR_DIRECTION.BOTTOM:
                        {
                            parent.bottom = child;
                            if(enqueue) m_itemQueue.Enqueue(child);
                            child.top = parent;
                        }
                        break;
                    case ASTAR_DIRECTION.LEFT:
                        {
                            parent.left = child;
                            if (enqueue) m_itemQueue.Enqueue(child);
                            child.right = parent;
                        }
                        break;
                    case ASTAR_DIRECTION.RIGHT:
                        {
                            parent.right = child;
                            if (enqueue) m_itemQueue.Enqueue(child);
                            child.left = parent;
                        }
                        break;
                    case ASTAR_DIRECTION.TOP:
                        {
                            parent.top = child;
                            if (enqueue) m_itemQueue.Enqueue(child);
                            child.bottom = parent;
                        }
                        break;
                }
            }
        }

        void checkDirection(ASTAR_DIRECTION direction, DistrictNavigationItem item, 
            DistrictNavigationItem directionItem, Vector3Int cellPos)
        {
            DistrictNavigationItem neighborhood = explore(neighborhoodMap, cellPos, direction);
            checkItem(item, neighborhood, direction);

            directionItem = explore(map, cellPos, direction);
            if(neighborhood == null) checkItem(item, directionItem, direction);

            if (directionItem && neighborhood)
            {
                neighborhood.allowedDirections = directionItem.allowedDirections;
                GameObject.Destroy(directionItem.gameObject);
            }
            else if (neighborhood)
            {
                switch (direction)
                {
                    case ASTAR_DIRECTION.BOTTOM:
                    case ASTAR_DIRECTION.TOP:
                        neighborhood.allowedDirections = ASTAR_DIRECTION.BOTTOM | ASTAR_DIRECTION.TOP;
                        break;
                    case ASTAR_DIRECTION.LEFT:
                    case ASTAR_DIRECTION.RIGHT:
                        neighborhood.allowedDirections = ASTAR_DIRECTION.LEFT | ASTAR_DIRECTION.RIGHT;
                        break;
                }
            }
        }

        void generateMap()
        {
            m_items.Clear();
            DistrictNavigationItem item = DistrictNavigationItem.Create(startingPoint.transform.position);
            
            item.canStop = true;
            item.isCity = true;


            Vector3 mapLocal = map.WorldToLocal(item.transform.position);
            Vector3Int mapCell = map.LocalToCell(mapLocal);
            m_filledPoints.Add(mapCell, item);

            item.allowedDirections = (ASTAR_DIRECTION.LEFT | ASTAR_DIRECTION.RIGHT | ASTAR_DIRECTION.TOP);
            m_itemQueue.Enqueue(item);

            while(m_itemQueue.Count > 0)
            {
                item = m_itemQueue.Dequeue();
                m_items.Add(item);

                mapLocal = map.WorldToLocal(item.transform.position);
                mapCell = map.LocalToCell(mapLocal);

                DistrictNavigationItem right = null;
                DistrictNavigationItem left = null;
                DistrictNavigationItem top = null;
                DistrictNavigationItem bottom = null;

                while (right == null)
                {
                    if ((item.allowedDirections & ASTAR_DIRECTION.RIGHT) != ASTAR_DIRECTION.RIGHT) break;
                    if (item.right != null) break;
                    if (!map.HasTile(mapCell)) break;

                    checkDirection(ASTAR_DIRECTION.RIGHT, item, right, mapCell);
                    
                    mapCell.x++;
                }

                mapCell = map.LocalToCell(mapLocal);
                while (left == null)
                {
                    if ((item.allowedDirections & ASTAR_DIRECTION.LEFT) != ASTAR_DIRECTION.LEFT) break;
                    if (item.left != null) break;
                    if (!map.HasTile(mapCell)) break;

                    checkDirection(ASTAR_DIRECTION.LEFT, item, left, mapCell);
                    checkItem(item, left, ASTAR_DIRECTION.LEFT);
                    mapCell.x--;
                }

                mapCell = map.LocalToCell(mapLocal);
                while (top == null)
                {
                    if ((item.allowedDirections & ASTAR_DIRECTION.TOP) != ASTAR_DIRECTION.TOP) break;
                    if (item.top != null) break;
                    if (!map.HasTile(mapCell)) break;

                    checkDirection(ASTAR_DIRECTION.TOP, item, top, mapCell);
                    checkItem(item, top, ASTAR_DIRECTION.TOP);
                    mapCell.y++;
                }

                mapCell = map.LocalToCell(mapLocal);
                while (bottom == null)
                {
                    if ((item.allowedDirections & ASTAR_DIRECTION.BOTTOM) != ASTAR_DIRECTION.BOTTOM) break;
                    if (item.bottom != null) break;
                    if (!map.HasTile(mapCell)) break;

                    checkDirection(ASTAR_DIRECTION.BOTTOM, item, bottom, mapCell);
                    checkItem(item, bottom, ASTAR_DIRECTION.BOTTOM);
                    mapCell.y--;
                }
            }

            EventSystem.Dispatch(new Event_DistrictNavigationInitialized(m_items.ToArray()));
        }

        DistrictNavigationItem explore(Tilemap t, Vector3Int position, ASTAR_DIRECTION direction)
        {
            Vector3Int offset = Vector3Int.zero;
            switch(direction)
            {
                case ASTAR_DIRECTION.LEFT:
                    offset.x = -1;
                    break;
                case ASTAR_DIRECTION.RIGHT:
                    offset.x = 1;
                    break;
                case ASTAR_DIRECTION.BOTTOM:
                    offset.y = -1;
                    break;
                case ASTAR_DIRECTION.TOP:
                    offset.y = 1;
                    break;
            }

            Vector3Int cellPos = position + offset;
            DistrictNavigationItem item = null;
            bool contains = false;
            if(t == map)
                contains = m_filledPoints.ContainsKey(cellPos);
            else contains = m_filledNeighborhoods.ContainsKey(cellPos);

            if (contains)
            {
                if(t == map)
                    return m_filledPoints[cellPos];
                return m_filledNeighborhoods[cellPos];
            }
            if (t.HasTile(cellPos))
            {
                
                TileBase tile = t.GetTile(cellPos);
                Vector3 newPos = t.GetCellCenterWorld(cellPos);

                
                switch(tile.name)
                {
                    case L_T:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "L_T";
                            item.canStop = false;
                            item.allowedDirections = (ASTAR_DIRECTION.LEFT | ASTAR_DIRECTION.TOP);
                        }
                        break;
                    case L_B:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "L_B";
                            item.canStop = false;
                            item.allowedDirections = (ASTAR_DIRECTION.LEFT | ASTAR_DIRECTION.BOTTOM);
                        }
                        break;
                    case L_R_T:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "L_R_T";
                            item.canStop = true;
                            item.allowedDirections = (ASTAR_DIRECTION.LEFT | ASTAR_DIRECTION.RIGHT | ASTAR_DIRECTION.TOP);
                        }
                        break;
                    case L_R_B:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "L_R_B";
                            item.canStop = true;
                            item.allowedDirections = (ASTAR_DIRECTION.LEFT | ASTAR_DIRECTION.RIGHT | ASTAR_DIRECTION.BOTTOM);
                        }
                        break;
                    case L_T_B:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "L_T_B";
                            item.canStop = true;
                            item.allowedDirections = (ASTAR_DIRECTION.LEFT | ASTAR_DIRECTION.TOP | ASTAR_DIRECTION.BOTTOM);
                        }
                        break;
                    case L_R_T_B:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "L_R_T_B";
                            item.canStop = true;
                            item.allowedDirections = (ASTAR_DIRECTION.LEFT | ASTAR_DIRECTION.RIGHT | ASTAR_DIRECTION.TOP |ASTAR_DIRECTION.BOTTOM);
                        }
                        break;
                    case R_T:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "R_T";
                            item.canStop = false;
                            item.allowedDirections = (ASTAR_DIRECTION.RIGHT | ASTAR_DIRECTION.TOP);
                        }
                        break;
                    case R_B:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "R_B";
                            item.canStop = false;
                            item.allowedDirections = (ASTAR_DIRECTION.RIGHT | ASTAR_DIRECTION.BOTTOM);
                        }
                        break;
                    case R_T_B:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "R_T_B";
                            item.canStop = true;
                            item.allowedDirections = (ASTAR_DIRECTION.RIGHT | ASTAR_DIRECTION.TOP | ASTAR_DIRECTION.BOTTOM);
                        }
                        break;
                    case CITY:
                        {
                            item = DistrictNavigationItem.Create(newPos);
                            item.name = "CITY";
                            item.canStop = true;
                            item.isCity = true;
                        }
                        break;
                }
            }

            if (item != null)
            {
                if (t == map)
                    m_filledPoints.Add(cellPos, item);
                else m_filledNeighborhoods.Add(cellPos, item);
            }

            return item;
        }
    }
}
