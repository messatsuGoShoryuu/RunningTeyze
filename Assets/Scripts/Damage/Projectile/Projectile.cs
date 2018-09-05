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

        public void SetCelerity(float celerity) { m_celerity = celerity; }

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
            if (m_owner == null) return;
            DamageApplier applier = other.GetComponent<DamageApplier>();

            if(applier != null)
            {
                float effectiveDmg = m_owner.dmgModifier * m_damage.damage;
                if(applier.channel == m_channel)
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

        public static float PredictCelerity(Vector2 origin, Vector2 destination, float angle, float gravity)
        {
            Vector2 d = destination - origin;
            float b = Mathf.Abs(d.x) * Mathf.Abs(gravity);
            float c = d.y * Mathf.Abs(gravity);
            float w = angle;

            float cos = Mathf.Cos(w);
            float sin = Mathf.Sin(w);

            float sec = 1.0f / cos;

            float a = (b * b * sec) / (2.0f * (b * sin - c * cos));

            return Mathf.Sqrt(a);
        }

        public static Vector2 PredictDirection(Vector2 origin, Vector2 destination, float celerity, float gravity)
        {
            Vector2 n = Vector2.zero;

             return n;
        }
    }
}
