using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze.UI
{
    public class Drawer : MonoBehaviour
    {

        [SerializeField]
        Vector3 m_openPosition;

        [SerializeField]
        Vector3 m_closedPosition;

        Vector3 m_targetPosition;

        bool m_doMove = false;

        [SerializeField]
        float m_speed;

        public void Toggle()
        {
            Debug.Log("Toggling");
            if (transform.position == m_openPosition) m_targetPosition = m_closedPosition;
            else m_targetPosition = m_openPosition;

            m_doMove = true;
        }

        private void FixedUpdate()
        {
            if (m_doMove)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, m_speed);
                if (Vector3.Distance(m_targetPosition, transform.position) <= 0.01f)
                {
                    m_doMove = false;
                    
                }
            }
        }

    }
}
