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

        Vector2 m_direction = Vector2.right;

        public void Fire()
        {
            Projectile projectile = GameObject.Instantiate<Projectile>(m_projectile, m_muzzle.transform.position, Quaternion.identity);
            projectile.transform.localScale = m_muzzle.transform.localScale;
            projectile.SetOwner(this);
            projectile.Fire(m_direction.normalized);
        }

        public  void AimX(float value)
        {
            if (value != 0.0f) m_direction.x = value;
        }

        public void AimY(float value)
        {
            if (value != 0.0f) m_direction.y = value;
        }
    }
}
