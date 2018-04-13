using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RunningTeyze
{
    public class AIPath
    {
 
        static Tilemap s_tileMap;
        static AStar s_aStar;
        static AStar astar
        {
            get
            {
                if (s_aStar == null)
                {
                    if (s_tileMap == null) s_tileMap = GameObject.FindObjectOfType<Tilemap>();
                    s_aStar = new AStar(s_tileMap);
                }
                return s_aStar;
            }
        }

        public static bool PathIsClear(Collider2D startCollider, Transform target)
        {
            Vector2 startPosition = startCollider.transform.position;
            Vector2 ray = (Vector2)target.position - startPosition;

            RaycastHit2D[] hit = new RaycastHit2D[1];
            startCollider.Raycast(ray, hit);
            Debug.DrawRay(startPosition, ray, Color.red);
            return hit[0].collider.transform == target;
        }

        public static bool PathIsClear(Collider2D startCollider, Vector2 position)
        {
            Vector2 startPosition = startCollider.transform.position;
            Vector2 ray = (Vector2)position - startPosition;

            int layerMask = (1 << LayerMask.NameToLayer("Ground"));

            RaycastHit2D[] hit = new RaycastHit2D[1];
            startCollider.Raycast(ray * 0.8f, hit,layerMask);
            Debug.DrawRay(startPosition, ray * 0.8f, Color.red);
            return hit[0].collider == null;
        }

        public static void DrawDebug()
        {
            astar.DrawDebug(Color.green);
        }

        public static Vector2 realStart { get { return astar.realStart; } }


         public static AStarPoint[] GetJumpingPoints(Vector2 startPosition,
            Vector2 target, float jumpDistance)
        {
            return astar.FindPath(startPosition, target, jumpDistance);
        }
        
    }
}

