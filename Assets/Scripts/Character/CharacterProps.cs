using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public struct Hitpoints
    {
        public float maxHealth;
        public float health;
        public float resistance;
        
    }

    public struct DamageModifier
    {
        float damageModifier;
        float rateModifier;
    }

    public struct CharacterProps
    {
        public Hitpoints hitpoints;
        public DamageModifier modifier;
    }

    public class CharacterPropOperations
    {
        public static bool DealDamage(ref Hitpoints hp, float damage)
        {
            damage *= 1.0f - hp.resistance;
            hp.health -= damage;
            hp.health = Mathf.Min(0.0f, hp.health);
            return hp.health == 0.0f;
        }

        public static float GetNormalizedHealth(Hitpoints hp)
        {
            return hp.health / hp.maxHealth;
        }
    }
}
