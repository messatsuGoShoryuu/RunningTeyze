using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public enum PROJECTILE_DESTROY_TYPE
    {
        LIFETIME,
        COLLISION,
        COLLISION_TARGET,
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField]
        float m_celerity = 5.0f;

        [Header("Lifetime")]
        [SerializeField]
        float m_lifeTime = 0.0f;
        [SerializeField]
        PROJECTILE_DESTROY_TYPE m_destroyType = PROJECTILE_DESTROY_TYPE.COLLISION;

        [Header("Damage")]
        [SerializeField]
        DAMAGE_CHANNEL m_channel;
        [SerializeField]
        Damage m_damage;

        Rigidbody2D m_rigidBody;

        SFireProjectile m_owner;

        // Use this for initialization
        void Start()
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
            if (m_lifeTime > 0.0f) Destroy(gameObject, m_lifeTime);   
        }

        public void SetOwner(SFireProjectile owner)
        {
            m_owner = owner;
        }
        public  void Fire(Vector2 direction, DAMAGE_CHANNEL channel, int ownerID, DamageModifier modifier)
        {
            m_channel = channel;
            m_damage.ownerID = ownerID;

            Damage.ApplyModifierToDamage(ref m_damage, ref modifier);

            Vector2 scale = transform.localScale;
            if (Vector2.Dot(direction, Vector2.right) < 0.0) scale.y *= -1.0f;
            transform.localScale = scale;

            if(m_rigidBody == null)
                m_rigidBody = GetComponent<Rigidbody2D>();
            m_rigidBody.velocity = direction * m_celerity * modifier.celerityModifier;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (m_rigidBody.velocity.sqrMagnitude > 0.0f)
            {
                transform.right = m_rigidBody.velocity.normalized;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (m_destroyType == PROJECTILE_DESTROY_TYPE.COLLISION)
            {
                if (collision.otherCollider.tag != tag && collision.gameObject != m_owner.gameObject)
                {
                    if (m_lifeTime == 0.0f) GameObject.Destroy(gameObject);
                    else gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            DamageApplier applier = other.GetComponent<DamageApplier>();

            if(applier != null)
            {
                float effectiveDmg = m_owner.dmgModifier * m_damage.damage;
                applier.ApplyDamage(effectiveDmg);
            }

            if (m_destroyType == PROJECTILE_DESTROY_TYPE.COLLISION)
            {
                if (other.tag != tag && other.gameObject != m_owner.gameObject)
                {
                    if (m_lifeTime == 0.0f) GameObject.Destroy(gameObject);
                    else gameObject.SetActive(false);
                }
            }
        }
    }
}
