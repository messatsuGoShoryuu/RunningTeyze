using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class SFireProjectile : Skill
    {
        [SerializeField]
        Projectile m_projectile;
        [SerializeField]
        GameObject m_muzzle;
        public Transform muzzle { get { return m_muzzle.transform; } }

        [SerializeField]
        CharacterProps m_props;
        public float dmgModifier { get { return m_props.dmgModifier.damageModifier; } }

        [SerializeField]
        DAMAGE_CHANNEL m_channel;

        Vector2 m_direction = Vector2.right;
        Vector2 m_pressedDirections = Vector2.right;

        protected void Start()
        {
            base.Start();
        }

        public void Fire()
        {
            Projectile projectile = GameObject.Instantiate<Projectile>(m_projectile, m_muzzle.transform.position, Quaternion.identity);
            projectile.transform.localScale = m_muzzle.transform.localScale;
            projectile.SetOwner(this);
            Vector2 direction = m_direction;
            if(m_pressedDirections.x == 0.0f && m_pressedDirections.y != 0.0f) direction.x = 0.0f;
            projectile.Fire(direction.normalized, m_channel, m_character.GetInstanceID(), m_character.props.dmgModifier);
        }

        public void Fire(float overrideVelocity)
        {
            Projectile projectile = GameObject.Instantiate<Projectile>(m_projectile, m_muzzle.transform.position, Quaternion.identity);
            projectile.transform.localScale = m_muzzle.transform.localScale;
            projectile.SetCelerity(overrideVelocity);
            projectile.SetOwner(this);
            Vector2 direction = m_direction;
            if(m_pressedDirections.x == 0.0f && m_pressedDirections.y != 0.0f) direction.x = 0.0f;
            projectile.Fire(direction.normalized, m_channel, m_character.GetInstanceID(), m_character.props.dmgModifier);
        }

        public void AimX(float value)
        {
            if (value != 0.0f)
            {
                m_direction.x = value;
            }
            m_pressedDirections.x = value;
        }

        public void AimXPlayer(float value)
        {
            if (value > 0.0f)m_direction.x = 1.0f;
            else if (value < 0.0f)m_direction.x = -1.0f;            

            m_pressedDirections.x = value;
        }

        public void AimYPlayer(float value)
        {
            if (value > 0.0f)m_direction.y = 1.0f;
            else if (value < 0.0f)m_direction.y = -1.0f;
            else 
            {
                m_direction.y = 0.0f;
            }

            m_pressedDirections.y = value;
        }

        public void AimY(float value)
        {
            if (value != 0.0f)m_direction.y = value;
             m_pressedDirections.y = value;
        }

        public void Aim(float value)
        {
            m_direction.x = Mathf.Cos(value);
            m_direction.y = Mathf.Sin(value);
        }
    }
}