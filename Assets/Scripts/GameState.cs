using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class GameState 
    {
        static TeyzeCharacter m_currentTeyzeCharacter;
        static Teyze s_currentTeyze;
        public static Teyze currentTeyze { get { return s_currentTeyze; } }

        public static void SetCurrentTeyze(string name)
        {
            s_currentTeyze = TeyzeManager.GetInstance(name);
        }
        public static void SetCurrentTeyze(TeyzeCharacter character)
        {
            s_currentTeyze = character.teyze;
            m_currentTeyzeCharacter = character;

        }
    }
}
