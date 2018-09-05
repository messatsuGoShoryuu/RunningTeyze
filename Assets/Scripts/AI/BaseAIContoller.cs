using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RunningTeyze
{
    public class BaseAIContoller : MonoBehaviour
    {
        protected  Character m_character;
        [SerializeField]    
        protected Transform m_targetTransform;
        [SerializeField]
        protected float m_minMoveDistance;
        [SerializeField]
        protected float m_characterWidth;
        [SerializeField]
        protected float m_jumpThreshold;
        [SerializeField]
        protected float m_maxJumpDistance;

        protected float m_minMoveDistanceSquare;
        protected Vector2 m_targetVector2;

        protected TilemapAstarPoint[] m_pathToFollow;

        protected int m_pointBeforeJumping;

        protected Tilemap m_tileMap;

        protected bool m_isMoving = true;
        protected bool m_movementHorizontalOK = false;
        protected bool m_movementVerticalOK = false;
        protected bool m_shouldJump = false;
        protected bool m_isWaitingToBeGrounded = false;

        protected bool m_followTarget = false;

        protected int m_nextPoint = 0;
        protected bool m_waitingForPath = false;

        protected bool m_stop = false;

        protected float m_lastPathfindingTime = 0.0f;
        protected float m_checkpointTime = 0.0f;
        [SerializeField]
        protected float m_checkpointCooldown = 1.5f;
        [SerializeField]
        protected float m_pathFindingCooldown = 4.0f;

        // Use this for initialization
        protected void Start()
        {
            m_character = GetComponent<Character>();
            m_tileMap = FindObjectOfType<Tilemap>();
            m_minMoveDistanceSquare = m_minMoveDistance * m_minMoveDistance;
        }

        protected void MoveTowards(Transform target, float slack)
        {
            m_targetTransform = target;
            m_isMoving = true;
            m_minMoveDistance = slack;
            m_minMoveDistanceSquare = slack * slack;
        }

        protected virtual void targetAcquired()
        {

        }

        // Update is called once per frame
        protected void Update()
        {
            if(m_followTarget)followTarget();
        }

        protected void followTarget()
        {
            Vector2 pos = (m_targetTransform == null) ? m_targetVector2 : (Vector2)m_targetTransform.position;

            Vector2 distance = pos - (Vector2)transform.position;
            float direction = Mathf.Sign(pos.x - transform.position.x);

            float dotX = Vector2.Dot(distance, Vector2.right);
            float dotY = Vector2.Dot(distance, Vector2.up);

            m_movementHorizontalOK = Mathf.Abs(dotX) < m_minMoveDistance;
            m_movementVerticalOK = Mathf.Abs(dotY) < m_minMoveDistance;

            if (m_movementHorizontalOK && m_movementVerticalOK)
            {
                direction = 0.0f;
                targetAcquired();
            }

            if (m_stop) direction = 0.0f;
            m_character.MoveHorizontal(direction);
        }

        protected IEnumerator refreshPath()
        {
            m_waitingForPath = true;
            yield return new WaitForSeconds(1.0f);
            

            bool pathIsClear = AIPath.PathIsClear(GetComponent<Collider2D>(),
                   m_targetTransform);

            if (!pathIsClear)
            {
                m_pathToFollow = null;
                m_pathToFollow = AIPath.GetJumpingPoints(m_character.groundPosition, m_targetTransform.position, m_maxJumpDistance);
                m_lastPathfindingTime = Time.time;
                m_nextPoint = 0;
                
               
            }
            else m_waitingForPath = false;

        }

        protected void drawRect(Vector2 pos, Color color)
        {
            Vector2 first = pos - Vector2.one * 0.1f;
            Debug.DrawRay(first, Vector2.right * 0.2f, color);
            first += Vector2.right * 0.2f;
            Debug.DrawRay(first, Vector2.up * 0.2f, color);
            first += Vector2.up * 0.2f;
            Debug.DrawRay(first, Vector2.left * 0.2f, color);
            first += Vector2.left * 0.2f;
            Debug.DrawRay(first, Vector2.down * 0.2f, color);
        }

        protected bool checkPointReached(float width, float height, float groundTolerance)
        {
            bool requiresGrounded = m_pathToFollow[m_nextPoint].jump;
            bool groundCheck = requiresGrounded ? m_character.isGrounded : true;
            bool widthCheck = Mathf.Abs(m_character.groundPosition.x - m_pathToFollow[m_nextPoint].point.x) <= width * 0.5f;
            bool heightCheck = Mathf.Abs(m_character.groundPosition.y - groundTolerance + height - m_pathToFollow[m_nextPoint].point.y) <= height + groundTolerance;

            return widthCheck && heightCheck && groundCheck;
        }

        protected IEnumerator JumpWithDelay()
        {
            yield return new WaitForSeconds(0.5f);
            m_character.Jump();
            m_shouldJump = false;
        }

        //HELPERS
        protected bool findClosestVertex(out Vector2 outParam, Vector2 target)
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
