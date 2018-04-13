using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public enum DAMAGE_CHANNEL
    {
        NONE = 0,
        FRIENDLY = (1 << 1),
        ENEMY = (1 << 2)
    }

    [System.Serializable]
    public struct Damage
    {
        [Header("Default Properties")]
        public float damage;

        [Header("CC")]
        public float knockbackVelocity;
        public float stunSeconds; 

        [Header("Damage Over Time")]
        public float tickCooldown;
        public float tickCount;

        [Header("AOE")]
        public float AOERadius;

        [Header("Messaging Info")]
        public int hitCount;
        public int ownerID;

        public static void ApplyModifierToDamage(ref Damage dmg, ref DamageModifier modifier)
        {
            dmg.AOERadius *= modifier.AOERadiusModifier;
            dmg.damage *= modifier.damageModifier;
            dmg.knockbackVelocity *= modifier.knockbackModifier;
            dmg.stunSeconds = modifier.stunModifier;
            dmg.tickCooldown = modifier.tickCooldownModifier;
            dmg.tickCount = modifier.tickCountModifier;
        }
    }
}
