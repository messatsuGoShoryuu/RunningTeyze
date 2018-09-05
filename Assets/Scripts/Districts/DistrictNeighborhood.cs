using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RunningTeyze
{
    public class DistrictNeighborhood : MonoBehaviour
    {
        [SerializeField]
        GameObject m_level;
        [SerializeField]
        bool m_isUnlocked = false;
        public bool isUnlocked { get { return m_isUnlocked; } }


        [SerializeField]
        static Dictionary<string, bool> s_unlockedList;
        public static Dictionary<string, bool> unlockedList
        {
            get
            {
                if (s_unlockedList == null) s_unlockedList = new Dictionary<string, bool>();
                return s_unlockedList;
            }
        }

        static string s_current;
        public static string current { get { return s_current; } }

        DistrictNavigationItem m_navItem;
        public DistrictNavigationItem navItem { get { return m_navItem; } }

        public void SetNavigation(DistrictNavigationItem item)
        {
            m_navItem = item;
            if (IsCurrent() && Level.success)
            {
                StartCoroutine(CR_Complete());
            }

        }
        IEnumerator CR_Complete()
        {
            yield return new WaitForEndOfFrame();
            Complete();
        }
        
        private void Start()
        {
            if (isUnlocked) GetComponent<SpriteRenderer>().enabled = false;

            if (!unlockedList.ContainsKey(gameObject.name))
            {
                unlockedList.Add(gameObject.name, m_isUnlocked);
            }
            else if (unlockedList[gameObject.name])
            {
                Unlock();
            }

        }

        public void Unlock()
        {
            if (!m_isUnlocked)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                EventSystem.Dispatch(new Event_DistrictNeighborhoodUnlocked(this));
                m_isUnlocked = true;
            }
        }

        public void Complete()
        {
            DistrictNeighborhood n = null;
            
            Queue<DistrictNavigationItem> items = new Queue<DistrictNavigationItem>();
            items.Enqueue(navItem);
            HashSet<DistrictNavigationItem> processed = new HashSet<DistrictNavigationItem>();

            while(items.Count > 0)
            {
                DistrictNavigationItem i = items.Dequeue();
                processed.Add(i);

                if (i != navItem)
                {
                    if (i.isCity)
                    {
                        i.neighborhood.Unlock();
                    }
                    else
                    {
                        if (i.left && !processed.Contains(i.left)) items.Enqueue(i.left);
                        if (i.right && !processed.Contains(i.right)) items.Enqueue(i.right);
                        if (i.top && !processed.Contains(i.top)) items.Enqueue(i.top);
                        if (i.bottom && !processed.Contains(i.bottom)) items.Enqueue(i.bottom);
                    }
                     
                }
                else
                {
                    if (i.left && !processed.Contains(i.left)) items.Enqueue(i.left);
                    if (i.right && !processed.Contains(i.right)) items.Enqueue(i.right);
                    if (i.top && !processed.Contains(i.top)) items.Enqueue(i.top);
                    if (i.bottom && !processed.Contains(i.bottom)) items.Enqueue(i.bottom);
                }
            }

            EventSystem.Dispatch(new Event_DistrictNeighborhoodCompleted(this));
        }

        private void OnMouseDown()
        {
            if(m_isUnlocked)
                EventSystem.Dispatch(new Event_DistrictNeighborhoodSelected(this));
        }
         
        public void LoadLevel()
        {
            if (m_level == null) return;
            Level.SetPrefab(m_level);
            s_current = gameObject.name;
            SceneManager.LoadSceneAsync("TileMapScene");
        }

        public bool IsCurrent()
        {
            return gameObject.name == s_current;
        }
    }
}
