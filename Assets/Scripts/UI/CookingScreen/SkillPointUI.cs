using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze.UI
{
    public class SkillPointUI : MonoBehaviour
    {
        [SerializeField]
        Text m_name; 

        [SerializeField]
        Text m_salary;

        [SerializeField]
        Text m_maxHealth;

        [SerializeField]
        Text m_dmgResistance;

        [SerializeField]
        Text m_speed;

        [SerializeField]
        Text m_strength;

        [SerializeField]
        Text m_cooking;

        [SerializeField]
        Text m_dmgMod;
        [SerializeField]
        Text m_dmgModA;

        [SerializeField]
        Text m_celerity;
        [SerializeField]
        Text m_celerityA;

        [SerializeField]
        Text m_stun;
        [SerializeField]
        Text m_stunA;

        [SerializeField]
        Text m_knocBack;
        [SerializeField]
        Text m_knocBackA;

        [SerializeField]
        Text m_dotFreq;
        [SerializeField]
        Text m_dotFreqA;

        [SerializeField]
        Text m_dotDuration;
        [SerializeField]
        Text m_dotDurationA;

        [SerializeField]
        Text m_AOERadius;
        [SerializeField]
        Text m_AOERadiusA;

        [SerializeField]
        Text m_pointsToDistribute;

        private void OnEnable()
        {
            updateInfo(GameState.currentTeyze);
            EventSystem.AddListener<Event_CurrentTeyzeChanged>(OnTeyzeChanged);
        }

        void OnTeyzeChanged(Event_CurrentTeyzeChanged e)
        {
            updateInfo(e.newTeyze);
        }

        void setText(Text name, Text amount, float value)
        {
            if (value == 0.0f)
            {
                amount.text = "--";
                name.color = Color.grey;
            }
            else
            {
                name.color = Color.black;
                amount.text = value.ToString();
            }
        }

        void updateInfo(Teyze t)
        {
            m_name.text = t.name;
            m_salary.text = t.salary.ToString();
            m_maxHealth.text = t.maxHealth.ToString();
            m_dmgResistance.text = t.resistance.ToString();
            m_speed.text = t.speed.ToString();
            m_strength.text = t.strength.ToString();
            m_cooking.text = t.cooking.ToString();

            setText(m_dmgMod, m_dmgModA, t.damageModifier.damageModifier);
            setText(m_celerity, m_celerityA, t.damageModifier.celerityModifier);
            setText(m_knocBack, m_knocBackA, t.damageModifier.knockbackModifier);
            setText(m_stun, m_stunA, t.damageModifier.stunModifier);
            setText(m_dotDuration, m_dotDurationA, t.damageModifier.tickCooldownModifier);
            setText(m_dotFreq, m_dotFreqA, t.damageModifier.tickCountModifier);
            setText(m_AOERadius, m_AOERadiusA, t.damageModifier.AOERadiusModifier);

            m_pointsToDistribute.text = t.skillPointsToDistribute.ToString();

        }

        private void OnDisable()
        {
            EventSystem.RemoveListener<Event_CurrentTeyzeChanged>(OnTeyzeChanged);
        }
    }
}
