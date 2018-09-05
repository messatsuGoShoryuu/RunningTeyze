using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public enum CAMERA_FOLLOW_MODE
    {
        SNAP,
        LERP,
        MOVE,
        CURVE
    }
    public class CameraFollower : MonoBehaviour
    {
        private void Awake()
        {
            s_singleton = this;
        }

        static CameraFollower s_singleton;

        [SerializeField]
        float m_delay;

        [SerializeField]
        CAMERA_FOLLOW_MODE m_followMode;

        [SerializeField]
        Transform m_target;


        public static void SetTarget(Transform target, CAMERA_FOLLOW_MODE mode = CAMERA_FOLLOW_MODE.SNAP)
        {
            s_singleton.m_target = target;
            s_singleton.m_followMode = mode;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_target)
            {
                switch (m_followMode)
                {
                    //TODO: Implement all modes
                    case CAMERA_FOLLOW_MODE.SNAP:
                        {
                            Vector3 pos = transform.position;
                            pos.x = m_target.position.x;
                            pos.y = m_target.position.y;
                            transform.position = pos;
                        }
                        break;
                }
            }
        }
    }
}
