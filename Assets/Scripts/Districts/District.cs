using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*A district is a group of neighbourhoods. Once you finish a neighborhood which is 
at the border of another district, that new district should be open (TO BE IMPLEMENTED)
 */

namespace RunningTeyze
{
    public class District : MonoBehaviour
    {
        [SerializeField]
        DistrictNeighborhood[] m_neighborhoods;

        [SerializeField]
        bool m_isUnlocked;

        public bool isUnlocked { get { return m_isUnlocked; } }
        public void Unlock() { m_isUnlocked = true; }
    }
}
