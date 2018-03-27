using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class Skill : MonoBehaviour
    {
        
        protected Character m_character;

        void Start()
        {
            m_character = GetComponent<Character>();
        }
    }
}
