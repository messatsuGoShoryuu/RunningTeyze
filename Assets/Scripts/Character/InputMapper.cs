using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RunningTeyze
{
    
    public class InputMapper: MonoBehaviour
    {
        

        [System.Serializable]
        public struct ActionEntry
        {
            public KeyCode keyCode;
            public UnityEvent callback;
        }

        [System.Serializable]
        public class AxisCallback : UnityEvent<float> { }

        [System.Serializable]
        public class NavigationEntry
        {
            public KeyCode keyCode;
            public InputMapper mapper;
        }

        [System.Serializable]
        public struct AxisEntry
        {
            public string id;
            public AxisCallback callback;
        }

        [Header("Navigation")]
        [SerializeField]
        bool m_affectChildren = true;
        [SerializeField]
        bool m_affectParent = true;
        [SerializeField]
        InputMapper[] m_children;
        [SerializeField]
        InputMapper m_parent;
        [SerializeField]
        bool m_shouldActivateHost = true;
        public bool shouldActivateHost { get { return m_shouldActivateHost; } }
        [SerializeField]
        public MonoBehaviour[] m_affectedObjects;

        [Header("Mappings")]
        [SerializeField]
        ActionEntry[] m_actionMappingsDown;

        [SerializeField]
        ActionEntry[] m_actionMappingsUp;

        [SerializeField]
        AxisEntry[] m_axisMappings;

        [SerializeField]
        NavigationEntry[] m_navigationMappings;

        


        private void OnEnable()
        {
            if (m_affectChildren)
            {
                for (int i = 0; i < m_children.Length; i++)
                {
                    if(m_children[i] != null)
                        m_children[i].enabled = false;
                }
            }
            if (m_affectParent)
                if (m_parent != null)
                    m_parent.enabled = false;
        }

        private void OnDisable()
        {
            Disable();
            if (m_affectParent)
                if (m_parent != null)
                    m_parent.Enable();
        }

        public void Enable()
        {
            if (m_shouldActivateHost) gameObject.SetActive(true);
            enabled = true;
            for (int i = 0; i < m_affectedObjects.Length; i++)
                m_affectedObjects[i].enabled = true;
        }

        public void Disable()
        {
            for (int i = 0; i < m_affectedObjects.Length; i++)
                m_affectedObjects[i].enabled = false;
            if (m_shouldActivateHost) gameObject.SetActive(false);
        }


        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < m_actionMappingsDown.Length; i++)
            {
                if (Input.GetKeyDown(m_actionMappingsDown[i].keyCode))
                {
                    if(m_actionMappingsDown[i].callback != null)
                        m_actionMappingsDown[i].callback.Invoke();
                }
            }

            for (int i = 0; i < m_actionMappingsUp.Length; i++)
            {
                if (Input.GetKeyUp(m_actionMappingsUp[i].keyCode))
                {
                    if (m_actionMappingsUp[i].callback != null)
                        m_actionMappingsUp[i].callback.Invoke();
                }
            }

            for (int i = 0; i < m_axisMappings.Length; i++)
            {
                m_axisMappings[i].callback.Invoke(Input.GetAxis(m_axisMappings[i].id));
            }


            for (int i = 0; i < m_navigationMappings.Length; i++)
            {
                if(Input.GetKeyDown(m_navigationMappings[i].keyCode))
                {
                    m_navigationMappings[i].mapper.Enable();
                    break;
                }
            }
        }
    }
}
