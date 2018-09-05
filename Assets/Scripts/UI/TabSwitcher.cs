using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze.UI
{
    public class TabSwitcher : MonoBehaviour
    {
        [SerializeField]
        GameObject[] m_tabs;

        [SerializeField]
        Drawer m_drawer;

        GameObject m_selectedTab = null;
        
        private void Start()
        {
            Debug.Log("Drawer == null" + (m_drawer == null));
        }

        public void BtnTabSwitched(GameObject obj)
        {
            Debug.Log("BtnTabSwitched");
            if(m_selectedTab != null)
            {
                if(m_selectedTab == obj)
                {
                    m_selectedTab = null;
                    if(m_drawer)m_drawer.Toggle();
                }
                else
                {
                    for(int i = 0; i<m_tabs.Length;i++)
                    {
                        if (m_tabs[i] == obj)
                        {
                            m_tabs[i].SetActive(true);
                            m_selectedTab = m_tabs[i];
                        }
                        else m_tabs[i].SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_tabs.Length; i++)
                {
                    if (m_tabs[i] == obj)
                    {
                        m_tabs[i].SetActive(true);
                        m_selectedTab = m_tabs[i];
                    }
                    else m_tabs[i].SetActive(false);
                }
                if (m_drawer) m_drawer.Toggle();
            }

            Debug.Log("obj.name = " + obj.name);
            if (m_selectedTab != null) Debug.Log("Selected tab = " + m_selectedTab.name);
        }
    }
}
