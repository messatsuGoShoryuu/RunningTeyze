using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class GameState 
    {        
        //Reference to the current prefab
        static TeyzeCharacter m_currentTeyzeCharacter;

        //Current selected Teyze's actual data
        static Teyze s_currentTeyze;

        //List of playable teyzes
        static List<Teyze> s_ownedTeyzes;
        public static Teyze[] ownedTeyzes { get { return s_ownedTeyzes.ToArray(); } }

        public static void Init()
        {
            s_ownedTeyzes = new List<Teyze>();
        }

        public static Teyze currentTeyze { get { return s_currentTeyze; } }

        //Make a Teyze playable
        public static void OwnTeyze(string name)
        {
            for(int i = 0; i<s_ownedTeyzes.Count;i++)
            {
                if (s_ownedTeyzes[i].name == name) return;
            }

            s_ownedTeyzes.Add(TeyzeManager.GetInstance(name));
        }

        public static void SetCurrentTeyze(string name)
        {
            OwnTeyze(name);
            s_currentTeyze = TeyzeManager.GetInstance(name);
        }
        public static void SetCurrentTeyze(TeyzeCharacter character)
        {
            s_currentTeyze = character.teyze;
            m_currentTeyzeCharacter = character;

        }
    }
}
