using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class LineOfSight : MonoBehaviour
    {
        public delegate void LineOfSightEntered(Transform t);
        public LineOfSightEntered OnLineOfSightEntered;


        public void SetCallback(LineOfSightEntered e)
        {
            OnLineOfSightEntered = e;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (OnLineOfSightEntered != null) OnLineOfSightEntered(collision.transform);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (OnLineOfSightEntered != null) OnLineOfSightEntered(null);
        }
    }
}
