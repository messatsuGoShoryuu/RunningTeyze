using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze
{
    public class IngameUI : MonoBehaviour
    {
        [SerializeField]
        Text m_teyzeInfo;

        [SerializeField]
        Text m_economy;

        // Use this for initialization
        void Start()
        {
            OnStatsChanged(null);
            EventSystem.AddListener<Event_PlayerStateChanged>(OnStatsChanged);
        }

        private void OnDestroy()
        {
            EventSystem.RemoveListener<Event_PlayerStateChanged>(OnStatsChanged);
        }

        void OnStatsChanged(Event_PlayerStateChanged e)
        {
            string text = GameState.currentTeyze.name + " Teyze";
            text += "\nReputation : " + GameState.currentTeyze.reputation;
            m_teyzeInfo.text = text;
            text = GameState.currentTeyze.wealth.ToString();
            
            m_economy.text = text;
            
        }
    }
}
