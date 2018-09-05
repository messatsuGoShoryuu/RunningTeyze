using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class EnemyAI : BaseAIContoller
    {
        [SerializeField]
        LineOfSight m_los;
        [SerializeField]
        float m_targetLoseCooldown;

        [SerializeField]
        SFireProjectile m_fireProjectile;
        [SerializeField]
        float m_fireAngle;

        [SerializeField]
        float m_fireCooldown;

        [SerializeField]
        float m_maxProjectileCelerity = 10.0f;

        float m_lastFireTimestamp;

        [SerializeField]
        SMelee m_melee;

        [SerializeField]
        float m_attackCooldown = 2.0f;

        float m_patrolDirection = 1.0f;

        [SerializeField]
        Vector2 m_patrolVector;
        Vector2 m_patrolOrigin;

        Animator m_animator;

        Coroutine m_targetLoseCoroutine;
        // Use this for initialization
        new void Start()
        {
            base.Start();
            m_los.SetCallback(SetTransform);
            m_animator = GetComponent<Animator>();
            m_patrolOrigin = transform.position;
        }

        void SetTransform(Transform t)
        {
            if (t != null)
            {
                if (m_targetLoseCoroutine != null)
                {
                    StopCoroutine(m_targetLoseCoroutine);
                    m_targetLoseCoroutine = null;
                    
                }
                m_targetTransform = t;
                m_followTarget = true;
            }
            else if(gameObject.activeInHierarchy)m_targetLoseCoroutine = StartCoroutine(CR_TargetLose());
        }

        IEnumerator CR_TargetLose()
        {
            yield return new WaitForSeconds(m_targetLoseCooldown);
            m_targetTransform = null;
            m_followTarget = false;
            m_patrolOrigin = transform.position;
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
            if (m_targetTransform == null && !m_followTarget)
                patrolHorizontal();
        }

        protected override void targetAcquired()
        {
            if (Time.time - m_lastFireTimestamp >= m_fireCooldown)
                Fire();
        }

        void Fire()
        {
            if (m_targetTransform == null) return;
            m_animator.Play("Fire");
            m_lastFireTimestamp = Time.time;
            m_stop = true;

            if (m_fireProjectile)
            {
                float celerity = Projectile.PredictCelerity(m_fireProjectile.muzzle.position,
                    m_targetTransform.position, 
                    Mathf.Deg2Rad * m_fireAngle, 
                    Physics2D.gravity.magnitude);
                m_fireProjectile.AimX(Mathf.Cos(Mathf.Deg2Rad * m_fireAngle) * m_character.transform.localScale.x);
                m_fireProjectile.AimY(Mathf.Sin(Mathf.Deg2Rad * m_fireAngle));

                celerity = Mathf.Min(celerity, m_maxProjectileCelerity);
                m_fireProjectile.Fire(celerity);
            }
        }

        public void AnimEvent_EndFire()
        {
            m_animator.Play("Idle");
            m_stop = false;
        }

        void patrolHorizontal()
        {
            m_targetTransform = null;
            Vector2 min = m_patrolOrigin - m_patrolVector;
            Vector2 max = m_patrolOrigin + m_patrolVector;

            if(m_patrolDirection == 1.0f)
            {
                if (transform.position.x > max.x)
                    m_patrolDirection = -1.0f;
            }
            else if (m_patrolDirection == -1.0f)
            {
                if (transform.position.x < min.x)
                    m_patrolDirection = 1.0f;
            }

            m_character.MoveHorizontal(m_patrolDirection);
        }
    }
}
