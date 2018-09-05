using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

//Applies damage to its owner character.

namespace RunningTeyze
{
    public class DamageApplier : MonoBehaviour
    {
        [SerializeField]
        DAMAGE_CHANNEL m_channel;
        public DAMAGE_CHANNEL channel { get { return m_channel; } }

        Character m_chacater;
        private void Start()
        {
            m_chacater = GetComponent<Character>();
        }

        public void ApplyDamage(float rawDmg)
        {
            Hitpoints hp = m_chacater.props.hitpoints;
            if (CharacterPropOperations.DealDamage(ref hp, rawDmg))
            {
                this.gameObject.SetActive(false);
                GameObject.Destroy(this.gameObject, 0.1f);
            }
            m_chacater.props.SetHitpoints(hp);
        }
    }
}
