using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class SMelee : Skill
    {
        [SerializeField]
        Animator m_animator;
        [SerializeField]
        Damage m_damage;
        [SerializeField]
        DAMAGE_CHANNEL m_channel;
        [SerializeField]
        Collider2D m_collider;

        // Use this for initialization
        protected void Start()
        {
            base.Start();
        }

        IEnumerator CR_resetTrigger()
        {
            yield return new WaitForSeconds(0.1f);
            m_animator.ResetTrigger("Hit");
        }

        public void Hit()
        {
            m_animator.SetTrigger("Hit");
            m_collider.enabled = true;
            StopAllCoroutines();
            StartCoroutine(CR_resetTrigger());
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            DamageApplier applier = other.GetComponent<DamageApplier>();

            if (applier != null)
            {
                if (applier.channel != m_channel) return;
                float effectiveDmg = m_character.props.dmgModifier.damageModifier * m_damage.damage;
                applier.ApplyDamage(effectiveDmg);
                m_collider.enabled = false;
            }
            
        }

    }
}
