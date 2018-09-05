using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze.UI.HUD
{
    public class HitpointDisplayer : MonoBehaviour
    {
        [SerializeField]
        ProgressBar m_progressBar;

        private void Start()
        {
            
        }
        private void OnEnable()
        {
            EventSystem.AddListener<Event_LevelTeyzeLoaded>(OnTeyzeLoaded);
        }

        private void OnDisable()
        {
            EventSystem.RemoveListener<Event_LevelTeyzeLoaded>(OnTeyzeLoaded);
        }

        void OnTeyzeLoaded(Event_LevelTeyzeLoaded e)
        {
            e.props.OnHitpointsChanged += OnHitpointsChanged;

            float ratio = e.props.hitpoints.health / e.props.hitpoints.maxHealth;
            m_progressBar.SetCutoutValue(ratio, true);
        }

        void OnHitpointsChanged(Hitpoints hp)
        {
            float ratio = hp.health / hp.maxHealth;
            m_progressBar.SetCutoutValue(ratio);
        }


    }
}
