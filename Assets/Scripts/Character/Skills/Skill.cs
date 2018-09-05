using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A skill is any ability a character can do. It could be firing projectiles or 
//lifting huge boulders.

namespace RunningTeyze
{
    public class Skill : MonoBehaviour
    {
        
        protected Character m_character;

        protected void Start()
        {
            m_character = GetComponent<Character>();
        }
    }
}
