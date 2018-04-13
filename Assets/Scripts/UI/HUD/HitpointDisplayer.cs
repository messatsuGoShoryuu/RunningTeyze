using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze.UI.HUD
{
    public class HitpointDisplayer : MonoBehaviour
    {
        [SerializeField]
        CharacterProps m_props;
        [SerializeField]
        ProgressBar m_progressBar;

        private void Start()
        {
            float ratio = m_props.hitpoints.health / m_props.hitpoints.maxHealth;
            m_progressBar.SetCutoutValue(ratio, true);
        }
        private void OnEnable()
        {
            m_props.OnHitpointsChanged += OnHitpointsChanged;
        }

        private void OnDisable()
        {
            m_props.OnHitpointsChanged -= OnHitpointsChanged;
        }

        void OnHitpointsChanged(Hitpoints hp)
        {
            float ratio = hp.health / hp.maxHealth;
            m_progressBar.SetCutoutValue(ratio);
        }


    }
}
