using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    [System.Serializable]
    public struct Hitpoints
    {
        public float maxHealth;
        public float health;
        public float resistance;
        
    }

    [System.Serializable]
    public struct DamageModifier
    {
        public float damageModifier;
        public float rateModifier;
    }

    public class CharacterProps : MonoBehaviour
    {
        [SerializeField]
        private  Hitpoints m_hitpoints;
        [HideInInspector]
        public Hitpoints hitpoints { get { return m_hitpoints; } }
        [SerializeField]
        private   DamageModifier m_dmgModifier;
        [HideInInspector]
        public DamageModifier dmgModifier { get { return m_dmgModifier; } }

        public delegate void HitpointsChanged(Hitpoints hp);
        public HitpointsChanged OnHitpointsChanged;

        public delegate void DmgModifierChanged(DamageModifier dmgModifier);
        public DmgModifierChanged OnDmgModifierChanged;

        public void SetHitpoints(Hitpoints hp)
        {
            m_hitpoints = hp;
            if (OnHitpointsChanged != null) OnHitpointsChanged(hp);
        }

        public void SetDmgModifier(DamageModifier dmgModifier)
        {
            m_dmgModifier = dmgModifier;
            if (OnDmgModifierChanged != null) OnDmgModifierChanged(dmgModifier);
        }
    }

    public class CharacterPropOperations
    {
        public static bool DealDamage(ref Hitpoints hp, float damage)
        {
            damage *= 1.0f - hp.resistance;
            hp.health -= damage;
            hp.health = Mathf.Clamp(hp.health, 0.0f, hp.health);
            return hp.health == 0.0f;
        }

        public static float GetNormalizedHealth(Hitpoints hp)
        {
            return hp.health / hp.maxHealth;
        }
    }
}
