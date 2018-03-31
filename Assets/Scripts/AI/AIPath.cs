using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RunningTeyze
{
    public class AIPath
    {
        static  List<Vector2> s_jumpingPoints;
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



         public static Vector2[] GetJumpingPoints(Vector2 startPosition,
            Vector2 target, float jumpDistance)
        {
            if (s_jumpingPoints == null) s_jumpingPoints = new List<Vector2>();
            s_jumpingPoints.Clear();
            Vector2[] range = astar.FindPath(startPosition, target, jumpDistance);
            if(range != null)
                s_jumpingPoints.AddRange(range);
            return s_jumpingPoints.ToArray();
        }

        public static void DrawDebug()
        {
            for(int i = 0; i<s_jumpingPoints.Count;i++)
            {
                if (s_jumpingPoints.Count == 1)
                    Debug.DrawRay(s_jumpingPoints[0], Vector2.up, Color.green);
                else if(i < s_jumpingPoints.Count - 1)
                    Debug.DrawLine(s_jumpingPoints[i], s_jumpingPoints[i + 1], Color.green);
            }
        }
    }
}

