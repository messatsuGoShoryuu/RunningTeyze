using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class PazarInteractor : MonoBehaviour
    {
        [SerializeField] bool m_isPlayer = false;
        
        public void InteractWithPazar()
        {
            EventSystem.Dispatch(new Event_PazarRequested(this.gameObject, m_isPlayer));
        }
 
    }
}
