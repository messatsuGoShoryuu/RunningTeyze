using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class DistrictChooser : MonoBehaviour
    {
        SpriteRenderer m_portrait;

        [SerializeField]
        float m_celerity;

        DistrictAStar m_aStar;

        DistrictNeighborhood m_targetNeighborhood;

        int m_pathCheckpoint = 0;
        Vector2[] m_path;

        [SerializeField]
        DistrictNeighborhood m_currentNeighborhood;

        // Use this for initialization

        private void Awake()
        {
            m_aStar = new DistrictAStar(GameObject.FindObjectOfType<UnityEngine.Tilemaps.Tilemap>());
        }
        void Start()
        {
            updatePortrait();
        }

        private void OnEnable()
        {
            EventSystem.AddListener<Event_CurrentTeyzeChanged>(OnTeyzeChanged);
            EventSystem.AddListener<Event_DistrictNeighborhoodSelected>(OnNeighborhoodSelected);
            EventSystem.AddListener<Event_DistrictNavigationInitialized>(OnNavigationInitialized);
        }

        void OnNavigationInitialized(Event_DistrictNavigationInitialized e)
        {
            for(int i = 0; i<e.items.Length;i++)
            {
                m_aStar.AddDistrictNavigationItem(e.items[i]);
            }
        }

        void OnTeyzeChanged(Event_CurrentTeyzeChanged e)
        {
            updatePortrait();
        }

        void OnNeighborhoodSelected(Event_DistrictNeighborhoodSelected e)
        {
            if (m_currentNeighborhood != null &&
                m_currentNeighborhood == e.neighborhood &&
                m_targetNeighborhood == null) m_currentNeighborhood.LoadLevel();
            else if (m_currentNeighborhood == null)
            {
                m_targetNeighborhood = e.neighborhood;
                m_path = m_aStar.GetPath(m_path[m_pathCheckpoint], m_targetNeighborhood.transform.position);
                if (m_path == null) m_targetNeighborhood = null;
                else m_pathCheckpoint = m_path.Length - 1;
            }
            else
            {
                m_targetNeighborhood = e.neighborhood;
                m_path = m_aStar.GetPath(transform.position, m_targetNeighborhood.transform.position);
                m_currentNeighborhood = null;
                if (m_path == null) m_targetNeighborhood = null;
                else m_pathCheckpoint = m_path.Length-1;
            }
        }

        private void OnDisable()
        {
            EventSystem.RemoveListener<Event_CurrentTeyzeChanged>(OnTeyzeChanged);
            EventSystem.RemoveListener<Event_DistrictNeighborhoodSelected>(OnNeighborhoodSelected);
            EventSystem.RemoveListener<Event_DistrictNavigationInitialized>(OnNavigationInitialized);
        }

        void updatePortrait()
        {
            if (m_portrait == null) m_portrait = GetComponent<SpriteRenderer>();
            m_portrait.sprite = Resources.Load<Sprite>("Sprites/TeyzePortraits/" + GameState.currentTeyze.name);
        }
        // Update is called once per frame
        void Update()
        {
            if(m_targetNeighborhood != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_path[m_pathCheckpoint], m_celerity * Time.deltaTime);
                if (Vector3.Distance(transform.position, m_path[m_pathCheckpoint]) < 0.01f)
                {
                    m_pathCheckpoint--;
                }

                if (Vector3.Distance(transform.position,m_targetNeighborhood.transform.position) < 0.01f)
                {
                    m_currentNeighborhood = m_targetNeighborhood;
                    m_targetNeighborhood = null;
                }

                if(m_path != null)
                    drawDebugPath();
            }
        }

        void drawDebugPath()
        {
            Debug.Log("Path length = " + m_path.Length);
            if ( m_path.Length< 2) return;

            for(int i =  1; i<m_path.Length;i++)
            {
                Vector3 a = m_path[i - 1];
                Vector3 b = m_path[i];
                a.z = -5;
                b.z = -5;
                Debug.DrawLine(a, b, Color.red);
            }
        }
    }
}
