using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class DistrictNavigationItem : MonoBehaviour
    {
        public DistrictNavigationItem left;
        public DistrictNavigationItem right;
        public DistrictNavigationItem top;
        public DistrictNavigationItem bottom;
        public bool isCity = false;
        public DistrictNeighborhood neighborhood;
    

        public ASTAR_DIRECTION allowedDirections;

        public bool canStop = false;

        public static DistrictNavigationItem Create(Vector3 position)
        {
            GameObject go = new GameObject();
            go.transform.position = position;
            go.AddComponent<DistrictNavigationItem>();
            go.name = "DistrictNavigationItem";
            return go.GetComponent<DistrictNavigationItem>();
        }

      
        private void Start()
        {
            if(isCity)
            {
                for (int i = 0; i < DistrictNavigator.neighborhoods.Length; i++)
                {
                    DistrictNeighborhood n = DistrictNavigator.neighborhoods[i];
                    if (Vector3.Distance(n.transform.position, transform.position) < 0.01f)
                    {
                        neighborhood = n;
                        n.SetNavigation(this);
                    }
                }
            }
        }
    }

}