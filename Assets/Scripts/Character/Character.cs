using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Basic character with movement. Allows smooth movement on slopes and irregular 
//surfaces.

namespace RunningTeyze
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Animator))]
    public class Character : MonoBehaviour
    {
        #region PRIVATE_MEMBER_Refs
        //GameObject References
        protected Rigidbody2D m_rigidBody;
        protected BoxCollider2D m_collider;
        protected Animator m_animator;
        protected CharacterProps m_props;
        public CharacterProps props { get { return m_props; } }
        #endregion

        #region INSPECTOR
        [Header("Movement Properties")]
        [SerializeField]
        protected float m_runSpeed = 5.0f;

        [SerializeField]
        protected float m_jumpVelocity = 4.0f;

        [Header("Ground Normal Following")]
        [SerializeField]
        protected bool m_followGroundNormals = true;
        [SerializeField]
        protected float m_maxSlopeAngle = 45.0f;
        [SerializeField]
        protected   float m_minimumTranslationDistance = 0.5f;

        [Header("Collision Properties")]
        [SerializeField]
        protected float m_groundDetectionRadius = 0.3f;



        [Header("Debug")]
        [SerializeField]
        protected bool m_showContactNormals = true;
        [SerializeField]
        protected bool m_showForwardVector = true;
        #endregion

        #region PRIVATE_MEMBER_Movement
        //Internal Properties
        //Follow this vector when grounded and m_followGroundNormals == true
        Vector2 m_forwardMovementVector;

        //Cache stuff for internal calculations
        protected float m_defaultGravityScale = 3.0f;
        protected float m_groundX;

      
        //Ground checking
        [SerializeField]
        protected   bool m_isGrounded = false;
        public bool isGrounded { get { return m_isGrounded; } }
        protected bool m_proximityChecked = false;
        
        //Used to prevent going to "grounded" state right after a jump
        float m_jumpTimeStamp = 0.0f;

        //Positioning
        protected bool m_faceRight = true;
        public bool faceRight { get { return m_faceRight; } }
        public Vector2 groundPosition { get { return new Vector2(m_collider.bounds.center.x, m_collider.bounds.min.y); } }

        //Contact Cache to check if we are grounded
        ContactPoint2D[] m_contactCache;
        #endregion

        #region DELEGATES
        public delegate void GroundTouched(Vector2 normal);
        public delegate void GroundLeft();

        public GroundTouched OnGroundTouched;
        public GroundLeft OnGroundLeft;
        #endregion

        #region INIT
        // Use this for initialization
        protected   void Start()
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
            m_rigidBody.isKinematic = false;
            m_collider = GetComponent<BoxCollider2D>();
            m_animator = GetComponent<Animator>();
            m_props = GetComponent<CharacterProps>();
            m_defaultGravityScale = m_rigidBody.gravityScale;
            m_contactCache = new ContactPoint2D[6];
        }

        protected void OnEnable()
        {
            if (m_rigidBody != null)
            {
                m_rigidBody.isKinematic = false;
                m_rigidBody.simulated = true;
            }
            if (m_animator != null) m_animator.enabled = true;
        }

        protected void OnDisable()
        {
            if (m_rigidBody != null)
            {
                m_rigidBody.isKinematic = true;
                m_rigidBody.simulated = false;
            }
            if (m_animator != null) m_animator.enabled = false;
        }

        #endregion

        #region API

        #region PUBLIC_Movement
        public void MoveHorizontal(float normalizedValue)
        {
            if (m_rigidBody == null) return;
            if (normalizedValue > 0)
            {
                m_faceRight = true;
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            if (normalizedValue < 0)
            {
                m_faceRight = false;
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }

            //Apply forward vector only if we are grounded.
            //Jump check is to ensure we don't prevent jumping
            if (m_followGroundNormals && m_isGrounded && Time.time - m_jumpTimeStamp > 0.1f)
            {
                //Prevent body from sliding
                m_rigidBody.gravityScale = 0.0f;
                if (Mathf.Approximately(normalizedValue, 0.0f))
                {
                    m_rigidBody.velocity = Vector2.zero;
                }
                else
                {
                    m_rigidBody.velocity = m_forwardMovementVector * m_runSpeed * Mathf.Abs(normalizedValue); 
                }
                if (Mathf.Abs(normalizedValue) >= 0.8f)
                {
                    if(!m_proximityChecked)m_groundX = Mathf.Abs(m_rigidBody.velocity.x);
                }
                else m_groundX = m_runSpeed;
            }
            else
            {
                setVelX(Mathf.Min(m_runSpeed, m_groundX) * normalizedValue);
                m_rigidBody.gravityScale = m_defaultGravityScale;
            }
        }
        

        public void Jump()
        {
            if (!m_isGrounded) return;
            if (Time.time - m_jumpTimeStamp < 0.1f) return;
            
            setVelY(m_jumpVelocity);
            m_jumpTimeStamp = Time.time;
            m_isGrounded = false;

        }
        #endregion

        #endregion


        #region PRIVATE_Movement
        //HELPERS
        protected void setVelX(float value)
        {
            if (m_rigidBody == null) return;
            Vector2 vel = m_rigidBody.velocity;
            vel.x = value;
            m_rigidBody.velocity = vel;
        }

        protected void setVelY(float value)
        {
            if (m_rigidBody == null) return;
            Vector2 vel = m_rigidBody.velocity;
            vel.y = value;
            m_rigidBody.velocity = vel;
        }

        void checkGround()
        {
            //Cache info 
            bool wasGrounded = m_isGrounded;
            m_isGrounded = false;

            float cos = Mathf.Cos(m_maxSlopeAngle * Mathf.Deg2Rad);


            int count = m_rigidBody.GetContacts(m_contactCache);

            //Reallocate cache if our current buffer is not enough
            if (count > m_contactCache.Length)
            {
                m_contactCache = new ContactPoint2D[count];
                m_rigidBody.GetContacts(m_contactCache);
            }

            //Compute maxDot and minDot for each grounded contact
            float maxDot = -Mathf.Infinity;
            float minDot = Mathf.Infinity;

            int minIndex = 0;
            int maxIndex = 0;

            for (int i = 0; i < count; i++)
            {
                float dot = Vector2.Dot(m_contactCache[i].normal, Vector2.up);

                if (m_contactCache[i].enabled)
                {
                    Color color = Color.white;
                    if ((dot >= cos ||  dot <= -cos) //Ensure we don't exceed max allowed slope
                        && Mathf.Abs(m_contactCache[i].point.y - m_collider.bounds.min.y) < m_groundDetectionRadius) //Apply radius
                    {
                        if (dot > maxDot)
                        {
                            maxDot = dot;
                            maxIndex = i;
                        }
                        if (dot < minDot)
                        {
                            minDot = dot;
                            minIndex = i;
                        }

                        color = Color.red;
                        m_isGrounded = true;
                    }
                    if(m_showContactNormals)
                        Debug.DrawRay(m_contactCache[i].point, m_contactCache[i].normal, color);
                       
                }
            }

            if (!m_followGroundNormals)
            {
                //DispatchEvents
                if (!wasGrounded && m_isGrounded)
                    if (OnGroundTouched != null) OnGroundTouched(m_contactCache[maxIndex].normal);
                    else if (wasGrounded && !m_isGrounded)
                        if (OnGroundLeft != null) OnGroundLeft();
                return;
            }


            //Follow ground normals stuff

            m_forwardMovementVector = Vector2.zero;
            Vector2 n = m_contactCache[minIndex].normal;
            if (m_isGrounded)
            {
                //If we have one point the contact normal is this one
                if (minDot == maxDot)
                {
                    n = m_contactCache[minIndex].normal;
                }
                //If minDot points upwards, pick it
                else if (maxDot >= 1.0f && minDot > 0.0f)
                {
                    n = m_contactCache[minIndex].normal;
                }
                else if (maxDot >= 1.0f)
                {
                    n = m_contactCache[maxIndex].normal;
                }

                //Compute m_forwardMovementVector
                Vector2 t = new Vector2(-n.y, n.x);

                float dot = Vector2.Dot(t, Vector2.right);
                m_forwardMovementVector = t * Mathf.Sign(dot) * transform.localScale.x;
            }

            //Check if ground is close in case we overshoot. If we do, correct the forward vector, 
            //else clear the m_isGrounded state.
            if (m_isGrounded) m_proximityChecked = false; 
            if (wasGrounded && !m_isGrounded && !m_proximityChecked && Time.time - m_jumpTimeStamp > 0.1f) checkGroundProximity();

            if(m_showForwardVector)
                Debug.DrawRay(transform.position, m_forwardMovementVector);

            //DispatchEvents
            if (!wasGrounded && m_isGrounded)
                if (OnGroundTouched != null) OnGroundTouched(n);
                else if (wasGrounded && !m_isGrounded)
                    if (OnGroundLeft != null) OnGroundLeft();
        }

        public void checkGroundProximity()
        {
            //Right and left bottom points of the collider
            Vector2 left = m_collider.bounds.min;
            Vector2 right = m_collider.bounds.min;
            right.x += m_collider.bounds.size.x;

            //Raycast for ground
            m_collider.enabled = false;
            RaycastHit2D leftHit = Physics2D.Raycast(left, Vector2.down * m_minimumTranslationDistance);
            RaycastHit2D rightHit = Physics2D.Raycast(right, Vector2.down * m_minimumTranslationDistance);
            if(m_showContactNormals)Debug.DrawRay(left, Vector2.down * m_minimumTranslationDistance, Color.white,10.0f);
            if (m_showContactNormals) Debug.DrawRay(right, Vector2.down * m_minimumTranslationDistance, Color.white,10.0f);
            m_collider.enabled = true;

            float cos = Mathf.Cos(m_maxSlopeAngle);
            float minPenetration = Mathf.Infinity;

            Vector2 d = Vector2.zero;

            Vector2 n = Vector2.up;
            Vector2 t = Vector2.right;
            if(leftHit.collider != null)
            {
                float dot = Vector2.Dot(leftHit.normal, Vector2.up);

                //Slope check
                if (Mathf.Abs(dot) >= cos)
                {
                    
                    m_isGrounded = true;
                    m_proximityChecked = true;
                    if (minPenetration > leftHit.distance)
                    {
                        minPenetration = leftHit.distance;

                        n = leftHit.normal;
                        //Compute m_forwardMovementVector
                        t = new Vector2(-n.y, n.x);

                        dot = Vector2.Dot(t, Vector2.right);
                        m_forwardMovementVector = t * Mathf.Sign(dot) * transform.localScale.x;

                        d = left - leftHit.point;
                    }
                }
            }
            if(rightHit.collider != null)
            {
                float dot = Vector2.Dot(rightHit.normal, Vector2.up);
                //Slope check
                if (Mathf.Abs(dot) >= cos)
                {
                    m_isGrounded = true;
                    m_proximityChecked = true;
                    if (minPenetration > rightHit.distance)
                    {
                        minPenetration = rightHit.distance;

                        n = rightHit.normal;
                        //Compute m_forwardMovementVector
                        t = new Vector2(-n.y, n.x);

                        dot = Vector2.Dot(t, Vector2.right);
                        m_forwardMovementVector = t * Mathf.Sign(dot) * transform.localScale.x;

                        d = right - rightHit.point;
                    }
                }
            }

            if (minPenetration > m_minimumTranslationDistance) m_isGrounded = false;

            //Correct position if we are grounded
            if (m_isGrounded)
            {
                transform.Translate(Vector2.down * minPenetration);
                transform.Translate(t * Vector2.Dot(t,d));
            }
            
        }
        #endregion

        #region PRIVATE_Update

        private void FixedUpdate()
        {
            checkGround();
        }

        private void LateUpdate()
        {
            if (!m_proximityChecked) m_animator.SetFloat("VelX", Mathf.Abs(m_rigidBody.velocity.x));
            m_animator.SetFloat("VelY", m_rigidBody.velocity.y);
            m_animator.SetBool("IsGrounded", m_isGrounded);
        }
        #endregion
    }
}
