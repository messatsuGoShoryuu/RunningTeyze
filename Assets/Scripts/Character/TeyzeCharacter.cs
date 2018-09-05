using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class TeyzeCharacter : Character
    {
        [Header("Inherited")]
        [SerializeField]
        string m_name;

        Teyze m_teyze;
        public Teyze teyze { get { return m_teyze; } }

        // Use this for initialization
        new void Start()
        {
            base.Start();
            m_teyze = TeyzeManager.GetInstance(m_name);

            Hitpoints hp = new Hitpoints();
            hp.maxHealth = m_teyze.maxHealth;
            hp.health = hp.maxHealth;
            hp.resistance = m_teyze.resistance;
            m_props.SetHitpoints(hp);

            DamageModifier dmgMod = new DamageModifier();
            dmgMod = m_teyze.damageModifier;
            m_props.SetDmgModifier(dmgMod);

            m_runSpeed = m_teyze.speed;
            m_jumpVelocity = m_teyze.weight / m_teyze.strength;
        }
    }
}
