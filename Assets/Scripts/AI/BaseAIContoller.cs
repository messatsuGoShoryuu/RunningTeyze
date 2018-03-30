using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RunningTeyze
{
    public class BaseAIContoller : MonoBehaviour
    {
        Character m_character;
        [SerializeField]    
        Transform m_targetTransform;
        [SerializeField]
        float m_minMoveDistance;
        [SerializeField]
        float m_characterWidth;
        [SerializeField]
        float m_jumpDistance;
        float m_minMoveDistanceSquare;
        Vector2 m_targetVector2;

        Vector2[] m_pathToFollow;

        int m_pointBeforeJumping;

        Tilemap m_tileMap;
        
        bool m_isMoving = true;
        bool m_movementHorizontalOK = false;
        bool m_movementVerticalOK = false;
        bool m_shouldJump = false;
        bool m_isWaitingToBeGrounded = false;

        int m_nextPoint = 0;

        // Use this for initialization
        void Start()
        {
            m_character = GetComponent<Character>();
            m_tileMap = FindObjectOfType<Tilemap>();
            m_minMoveDistanceSquare = m_minMoveDistance * m_minMoveDistance;
        }


        public void MoveTowards(Transform target, float slack)
        {
            m_targetTransform = target;
            m_isMoving = true;
            m_minMoveDistance = slack;
            m_minMoveDistanceSquare = slack * slack;
        }

        // Update is called once per frame
        void Update()
        {
            if(m_isMoving)
            {
                m_movementVerticalOK = false;
                m_movementHorizontalOK = false;
                Vector2 pos = (m_targetTransform == null) ? m_targetVector2 : (Vector2)m_targetTransform.position;

                Vector2 distance = pos - (Vector2)transform.position;
                float direction = Mathf.Sign(pos.x - transform.position.x);

                float dotX = Vector2.Dot(distance, Vector2.right);
                float dotY = Vector2.Dot(distance, Vector2.up);

                if (m_tileMap == null) m_tileMap = FindObjectOfType<Tilemap>();
                bool pathIsClear = AIPath.PathIsClear(GetComponent<Collider2D>(),
                    m_targetTransform);

                m_movementHorizontalOK = Mathf.Abs(dotX) < m_minMoveDistance && pathIsClear;
                m_movementVerticalOK = Mathf.Abs(dotY) < m_minMoveDistance && pathIsClear;

                


                if (m_movementVerticalOK && m_movementHorizontalOK)
                {
                    direction = 0;
                    m_pathToFollow = null;
                }
                else if (!pathIsClear)
                {
                    if (m_pathToFollow == null)
                    {
                        if (m_character.isGrounded)
                        {
                            m_pathToFollow = AIPath.GetJumpingPoints(m_character.groundPosition, m_targetTransform.position, Vector2.one);
                            m_nextPoint = 0;
                        }
                    }
                }
                else m_pathToFollow = null;

                if (m_pathToFollow != null && m_pathToFollow.Length > 0)
                {
                    if (m_character.isGrounded)
                    {
                        if (m_isWaitingToBeGrounded)
                        {
                         //   float beforeDist = Vector2.Distance(m_character.groundPosition, m_pathToFollow[m_pointBeforeJumping]);
                          //  float nowDist = Vector2.Distance(m_character.groundPosition, m_pathToFollow[m_nextPoint]);
                           // if (beforeDist <= nowDist)
                            {
                          //      m_nextPoint = m_pointBeforeJumping;
                            }
                            m_isWaitingToBeGrounded = false;
                        }

                        if(m_nextPoint >= m_pathToFollow.Length)
                        {
                            m_pathToFollow = null;
                            return;
                        }
                        direction = Mathf.Sign(m_pathToFollow[m_nextPoint].x - m_character.groundPosition.x);
                        Debug.DrawLine(m_character.groundPosition, m_pathToFollow[m_nextPoint], Color.yellow);
                        if (m_nextPoint < m_pathToFollow.Length)
                        {


                            if (Vector2.Distance(m_character.groundPosition, m_pathToFollow[m_nextPoint]) >= m_jumpDistance)
                            {
                                Debug.Log("SHOULD JUMP!");
                                m_pointBeforeJumping = m_nextPoint;
                                m_character.Jump();
                                m_isWaitingToBeGrounded = true;
                            }
                        }   
                    }
                    if(m_nextPoint >= m_pathToFollow.Length)
                    {
                        m_pathToFollow = null;
                        return;
                    }
                    if (Vector2.Distance(m_character.groundPosition, m_pathToFollow[m_nextPoint]) < 1.2f)
                    {
                        m_nextPoint++;
                    }
                    direction = Mathf.Sign(m_pathToFollow[m_nextPoint].x - m_character.groundPosition.x);
                    Debug.DrawRay(m_pathToFollow[m_nextPoint], Vector2.one, Color.green);
                    AIPath.DrawDebug();
                }

                m_character.MoveHorizontal(direction);

            }
        }

        IEnumerator JumpWithDelay()
        {
            yield return new WaitForSeconds(0.5f);
            m_character.Jump();
            m_shouldJump = false;
        }

        //HELPERS
        bool findClosestVertex(out Vector2 outParam, Vector2 target)
        {
            outParam = Vector2.zero;
            Vector2 normal = (Vector2)m_targetTransform.position - (Vector2)transform.position;

            RaycastHit2D[] hits = new RaycastHit2D[1];
            GetComponent<Collider2D>().Raycast(normal, hits);
            RaycastHit2D hit = hits[0];


            Debug.DrawRay(transform.position, normal, Color.red);
            

            if (hit.collider == null || hit.transform == m_targetTransform) return false;

            Vector3Int tilePos = m_tileMap.WorldToCell(hit.point);

            for(int i = 0; i<100;i++)
            {
                Vector3Int right = tilePos + Vector3Int.right*i;
                Vector3Int left = tilePos + Vector3Int.left*i;

                if(!m_tileMap.HasTile(right))
                {
                    outParam = m_tileMap.GetCellCenterWorld(right);
                    return true;
                }

                if (!m_tileMap.HasTile(left))
                {
                    outParam = m_tileMap.GetCellCenterWorld(left);
                    return true;
                }
            }
            return false;
        }
    }
}
