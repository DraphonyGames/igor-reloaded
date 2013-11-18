using UnityEngine;
using System.Collections;



/// <summary>
/// generic AI helpers
/// </summary>
public class EnemyAI : CommonEntity
{
    /// <summary>
    /// enum for definition enemy type
    /// </summary>
    public enum EnemyTpye
    {
        /// <summary>
        /// Enemy only has a melee attack.
        /// </summary>
        Melee,
        /// <summary>
        /// Enemy only has a range attack.
        /// </summary>
        Range,
    }

    /// <summary>
    /// the type of enemy
    /// </summary>
    public EnemyTpye type;

    /// <summary>
    /// The waypoints array
    /// </summary>
    public Transform[] NavMeshTransforms = null;

    /// <summary>
    /// position array for save game
    /// </summary>
    public Vector3[] NavMeshTransformsPosition = null;

    /// <summary>
    /// initial position of enemy
    /// </summary>
    public Vector3 initialPosition;

    /// <summary>
    /// radius the player needs to be within to be detected by enemy
    /// </summary>
    public float detectionRange;

    /// <summary>
    /// radius the player needs to be within to be followed by the enemy
    /// (followingRange > detectionRange)
    /// </summary>
    public float followingRange;

    /// <summary>
    /// the max vertical sight angle that a range enemy can see
    /// </summary>
    public float rangeEnemyMaxVerticalSightAngle = 65;

    /// <summary>
    /// the max vertical sight angle that a melee enemy can see
    /// </summary>
    public float meleeEnemyMaxVerticalSightAngle = 20;

    /// <summary>
    /// the max range that range enemy will take a attack
    /// </summary>
    public float rangeAttackRange;

    /// <summary>
    /// view angle for enemy
    /// </summary>
    public float viewAngle = 60f;

    /// <summary>
    /// Navigation mesh agent
    /// </summary>
    public NavMeshAgent nma;

    /// <summary>
    /// The i-st waypoint
    /// </summary>
    public int currentWaypoint = 0;

    /// <summary>
    /// direction of Patrol
    /// true = positive
    /// false = negative
    /// </summary>
    private bool posOrNega = true;

    /// <summary>
    /// whether patrolling is allowed
    /// </summary>
    public bool isPatrolling = true;

    /// <summary>
    /// If enemy run away
    /// </summary>
    private bool isEscape = false;

    /// <summary>
    /// whether the enemy needs to return to the path after following finished (player out of sight)
    /// </summary>
    private bool isCurrentlyFollowing = false;

    /// <summary>
    /// step of the current escape
    /// </summary>
    private float escapeStep = 0f;

    /// <summary>
    /// point to return to after a successful escape
    /// </summary>
    private Vector3 escapeReturnPoint;

    /// <summary>
    /// time the robot needs to explode after death
    /// </summary>
    public float timeToDestruction = 3;

    /// <summary>
    /// whether the enemy is a instance that instantiated in loading game
    /// </summary>
    public bool isloaded = false;

    /// <summary>
    /// starts an escape for "seconds" with the velocity of "speed"
    /// </summary>
    /// <param name="seconds">in seconds</param>
    /// <param name="speed">the speed</param>
    public void ExecuteEscape(float seconds = 3f, float speed = 0f)
    {
        isEscape = true;
        escapeStep = seconds + 2f;

        if (speed == 0f)
        {
            speed = nma.speed;
        }

        escapeReturnPoint = transform.position;
        ReturnToRandomWaypoint();
    }

    /// <summary>
    /// stops the current escape
    /// </summary>
    public void StopEscape()
    {
        isEscape = false;
    }

    /// <summary>
    /// controls whether the patrol logic is active
    /// </summary>
    /// <param name="whetherToActivate">some parameter</param>
    public void ExecutePatroling(bool whetherToActivate)
    {
        isPatrolling = whetherToActivate;
    }

    /// <summary>
    /// called on Death of the AI
    /// </summary>
    public override void Die()
    {
        base.Die();
        if (nma != null)
        {
            nma.Stop();
        }
    }

    /// <summary>
    /// destruction directive of the enemy
    /// </summary>
    internal virtual void Destruct()
    {
    }

    /// <summary>
    /// called when the enemy is in melee range and executes an attack
    /// </summary>
    /// <param name="player">the player</param>
    public virtual void OnMeleeAttack(GameObject player)
    {
        StartMeleeAttackCooldown(1);
    }

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void Awake()
    {
        initialPosition = transform.position;
    }

    /// <summary>
    /// Unity Start
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // if the nav-mesh is empty, add one node at the current position (as return point f.e.)
        // first: kill NMT if it contains invalid waypoints...
        if ((NavMeshTransforms != null) && (NavMeshTransforms.Length > 0))
        {
            if (NavMeshTransforms[0] == null) //invalid object
            {
                NavMeshTransforms = null;
            }
        }
        if ((NavMeshTransforms == null) || (NavMeshTransforms.Length == 0))
        {
            NavMeshTransforms = new Transform[1];
            GameObject dummy = new GameObject();
            NavMeshTransforms[0] = dummy.transform;
            NavMeshTransforms[0].position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        if (nma == null)
        {
            nma = GetComponent<NavMeshAgent>();
        }

        if (nma)
        {
            nma.speed = speed;
            nma.Resume();
        }

        NavMeshTransformsPosition = new Vector3[NavMeshTransforms.Length];
        for (int i = 0; i < NavMeshTransforms.Length; i++)
        {
            NavMeshTransformsPosition[i] = NavMeshTransforms[i].position;
        }
        
        if (isloaded && nma != null && NavMeshTransforms.Length > 1)
        {
            nma.SetDestination(NavMeshTransformsPosition[currentWaypoint]);
        }
    }

    /// <summary>
    /// Unity Update
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if (IsDead())
        {
            if (timeToDestruction > 0)
            {
                timeToDestruction -= Time.deltaTime;
            }
            else
            {
                Destruct();
            }
            return;
        }

        CheckTarget();

        // when the enemy has a custom destination, ignore everything else!
        if (CheckCustomDestination())
        {
            return;
        }

        // execute navigation logic only if NMA is assigned (e.g. not for stationary arms..)
        if (nma != null && currentState == EntityState.normal)
        {
            if (isEscape)
            {
                EnemyEscape();
                return;
            }

            if (currentTarget != null)
            {
                if (type == EnemyTpye.Melee)
                {
                    if (InMeleeAttackRange())
                    {
                        LookAtPlayer();
                        if (meleeAttackCurrentCooldown <= 0)
                        {
                            nma.ResetPath();
                            OnMeleeAttack(currentTarget.gameObject);
                        }
                        return;
                    }
                }
                else if (type == EnemyTpye.Range)
                {
                    if (PlayerInSight() && InBattleMode(rangeAttackRange) && Mathf.Abs(GetVerticalAngleBetweenEnemyAndPlayer()) < rangeEnemyMaxVerticalSightAngle)
                    {
                        StopPatrol();
                        LookAtPlayer();

                        if (rangeAttackCurrentCooldown <= 0f)
                        {
                            OnRangeAttack();
                        }
                        else
                        {
                            rangeAttackCurrentCooldown -= 1f * Time.deltaTime;
                        }
                        return;
                    }
                }

                if ((blindFollowingEnabled || (PlayerInSight() && (type == EnemyTpye.Melee))) && (Mathf.Abs(GetVerticalAngleBetweenEnemyAndPlayer()) < meleeEnemyMaxVerticalSightAngle || GetDistanceBetweenEnemyAndPlayer() < 5.0f))
                {
                    EnemyFollow();
                    return;
                }
                else if (isCurrentlyFollowing)
                {
                    isCurrentlyFollowing = false;
                    ReturnToNearWaypoint();
                    return;
                }
            } // currentTarget != null

            if (isPatrolling && NavMeshTransformsPosition.Length > 1)
            {
                OrdinalPatrol();
                return;
            }
            else if (!InWayPoint() && NavMeshTransformsPosition.Length > 0)
            {
                ReturnToNearWaypoint();
                return;
            }
        }
    }

    #region Navigation

    /// <summary>
    /// if the enemy has a custom destination, it will not follow the player etc until that destination was reached
    /// </summary>
    public bool hasCustomDestination = false;

    /// <summary>
    /// sets a custom destination for the navigation mesh agent, a callback will be called once the destination has been reached
    /// </summary>
    /// <param name="destination">the destination</param>
    public void SetCustomDestination(Vector3 destination)
    {
        nma.SetDestination(destination);
        nma.Resume();
        hasCustomDestination = true;
    }

    /// <summary>
    /// behavior that is executed when the enemy has reached a custom destination
    /// </summary>
    /// <returns>whether no other behavior allowed at the moment</returns>
    public bool CheckCustomDestination()
    {
        if (!hasCustomDestination)
        {
            return false;
        }

        // already reached the spot?
        if (nma.remainingDistance <= 2f)
        {
            hasCustomDestination = false;
            OnCustomDestinationReached();
        }

        return true;
    }

    /// <summary>
    /// this callback is called once the enemy has reached a custom destination
    /// </summary>
    public virtual void OnCustomDestinationReached()
    {

    }

    /// <summary>
    /// indicates whether enemy is in the way point
    /// </summary>
    /// <returns>if in way point</returns>
    public bool InWayPoint()
    {
        for (int i = 0; i < NavMeshTransformsPosition.Length; i++)
        {
            Vector3 diffVec = NavMeshTransformsPosition[i] - transform.position;
            if (diffVec.magnitude < 2)
            {
                return true;
            }
        }
        return false;

    }

    /// <summary>
    ///  The object move ordinal from a waypoint to another waypoint
    /// </summary>
    public void OrdinalPatrol()
    {
        // currently calculating a path? Do not overwrite! remainingDistance would be 0..
        if (nma.pathPending)
        {
            return;
        }

        if (posOrNega == true && nma.remainingDistance < 1f)
        {   // If the object has finish the patrol from start waypoint to end waypoint , patrol it from end waypoint to start waypoint.

            if (currentWaypoint == NavMeshTransformsPosition.Length - 1)
            {
                posOrNega = false;
                currentWaypoint--;
                nma.SetDestination(NavMeshTransformsPosition[currentWaypoint]);
                return;
            }
            currentWaypoint++;
            nma.SetDestination(NavMeshTransformsPosition[currentWaypoint]);
        }
        else if (posOrNega == false && nma.remainingDistance < 1f)
        {    // If the object has finish the patrol from end waypoint to start waypoint , patrol it from start waypoint to end waypoint.
            if (currentWaypoint == 0)
            {
                posOrNega = true;
                currentWaypoint++;
                nma.SetDestination(NavMeshTransformsPosition[currentWaypoint]);
                return;
            }
            currentWaypoint--;
            nma.SetDestination(NavMeshTransformsPosition[currentWaypoint]);
        }

    }

    /// <summary>
    /// The object move randomly from a waypoint to another waypoint
    /// </summary>
    private void RandomPatrol()
    {
        if (nma.remainingDistance <= 1f && !nma.pathPending)
        {
            nma.SetDestination(NavMeshTransformsPosition[Random.Range(0, NavMeshTransformsPosition.Length)]);
        }
    }

    /// <summary>
    /// The enemy return to a random way point
    /// </summary>
    private void ReturnToRandomWaypoint()
    {
        nma.SetDestination(NavMeshTransformsPosition[Random.Range(0, NavMeshTransformsPosition.Length)]);
    }

    /// <summary>
    /// The enemy return to the nearest way point
    /// </summary>
    public void ReturnToNearWaypoint()
    {
        if (nma.pathPending)
        {
            return;
        }

        float min = (transform.position - NavMeshTransformsPosition[0]).magnitude;
        int index = 0;
        for (int i = 1; i < NavMeshTransformsPosition.Length; i++)
        {

            if ((transform.position - NavMeshTransformsPosition[i]).magnitude < min)
            {
                index = i;
                min = (transform.position - NavMeshTransformsPosition[i]).magnitude;
            }
        }
        nma.SetDestination(NavMeshTransformsPosition[index]);
    }

    /// <summary>
    /// enemy stop patrol
    /// </summary>
    public void StopPatrol()
    {
        nma.ResetPath();
    }
    #endregion

    #region BasicAIBehavior

    /// <summary>
    /// when enabled, the AI will still follow its target even if it is not in sight
    /// </summary>
    private bool blindFollowingEnabled = false;

    /// <summary>
    /// enables or disables blind following
    /// </summary>
    /// <param name="whether">if enabled</param>
    public void EnableBlindFollowing(bool whether = true)
    {
        blindFollowingEnabled = whether;
    }

    /// <summary>
    /// indicates whether the player in the sight of enemy
    /// The sight area is a fan shape which defined by @detectionRange and @viewAngle
    /// </summary>
    /// <returns>if in sight</returns>
    protected bool PlayerInSight()
    {
        // If the current target does not exist anymore, we do not see it
        if (!currentTarget)
        {
            return false;
        }

        float theta = viewAngle / 2;
        Vector3 forward = transform.forward;
        Vector3 diffVec = currentTarget.gameObject.transform.position - transform.position;
        float angle = Vector3.Angle(forward, diffVec);

        if (System.Math.Abs(angle) <= theta && diffVec.magnitude <= detectionRange)
        {
            float playerDist = detectionRange + 1;
            float smallestDist = detectionRange;

            LayerMask layer = 1 << 2;
            layer = ~layer;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, currentTarget.gameObject.collider.bounds.center - transform.position, detectionRange, layer);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    continue;
                }

                if (hit.distance < smallestDist)
                {
                    smallestDist = hit.distance;
                }

                if (hit.collider.gameObject == currentTarget.gameObject)
                {
                    playerDist = hit.distance;
                }
            }

            if (playerDist == smallestDist)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// enemy escape from battle,run toward the opposite direction of player and stop 
    /// </summary>
    private void EnemyEscape()
    {
        escapeStep -= 1.0f * Time.deltaTime;

        if (escapeStep <= 0)
        {
            StopEscape();
        }
        else if (escapeStep <= 2f)
        {
            nma.SetDestination(escapeReturnPoint);
        }
    }

    /// <summary>
    /// enemy follow player
    /// </summary>
    private void EnemyFollow()
    {
        isCurrentlyFollowing = true;
        if (GetDistanceBetweenEnemyAndPlayer() <= meleeAttackRadius)
        {
            LookAtPlayer();
            nma.ResetPath();
            return;
        }

        nma.SetDestination(currentTarget.gameObject.transform.position);
    }

    /// <summary>
    /// enemy stop follow player
    /// </summary>
    private void StopFollow()
    {
        nma.ResetPath();
    }
    /// <summary>
    /// change the rotation to face the player
    /// </summary>
    public void LookAtPlayer()
    {
        Vector3 lookAt = currentTarget.gameObject.transform.position - transform.position;
        lookAt.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookAt), 0.5f);
    }

    /// <summary>
    /// the enemy move to behind of player
    /// </summary>
    private void RotateToBehind()
    {
        if (System.Math.Abs(Vector3.Angle(transform.forward, currentTarget.gameObject.transform.forward)) < 10)
        {
            return;
        }
        Vector3 diffVec = currentTarget.gameObject.transform.position - transform.position;
        Vector2 forward = transform.forward;
        float playerEnemyAngle = Vector3.Angle(forward, diffVec);
        float rotateAngle = 180f - playerEnemyAngle;
        transform.RotateAround(currentTarget.gameObject.transform.position, Vector3.up, rotateAngle * Time.deltaTime);
    }

    /// <summary>
    /// get the distance between enemy and player
    /// </summary>
    /// <returns>the distance</returns>
    public float GetDistanceBetweenEnemyAndPlayer()
    {
        return (currentTarget.gameObject.transform.position - transform.position).magnitude;
    }

    /// <summary>
    /// returns the angle
    /// </summary>
    /// <returns>some angle</returns>
    public float GetVerticalAngleBetweenEnemyAndPlayer()
    {
        if (GetDistanceBetweenEnemyAndPlayer() != 0)
        {
            return Mathf.Asin((currentTarget.gameObject.collider.bounds.center.y - transform.position.y) / GetDistanceBetweenEnemyAndPlayer()) * 180 / Mathf.PI;
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// Get Vertical Angle Between Object And Target
    /// </summary>
    /// <param name="objectPosition">object position</param>
    /// <returns>vertical angle between input object and currentTarget</returns>
    public float GetVerticalAngleBetweenObjectAndTarget(Vector3 objectPosition)
    {
        if (GetDistanceBetweenEnemyAndPlayer() != 0)
        {
            return Mathf.Asin((currentTarget.gameObject.collider.bounds.center.y - objectPosition.y) / GetDistanceBetweenEnemyAndPlayer()) * 180 / Mathf.PI;
        }
        else
        {
            return 0;
        }
    }
    #endregion

    #region MeleeAttackBehavior

    /// <summary>
    /// melee attack area
    /// </summary>
    public float meleeAttackRadius = 5f;
    /// <summary>
    /// melee attack area
    /// </summary>
    public float meleeAttackAngle = 60;

    /// <summary>
    /// current cooldown of the melee attack
    /// </summary>
    private int meleeAttackCurrentCooldown = 0;

    /// <summary>
    /// OnMeleeAttack will not be called for "seconds" seconds
    /// </summary>
    /// <param name="seconds">the seconds</param>
    public void StartMeleeAttackCooldown(int seconds)
    {
        if (meleeAttackCurrentCooldown == 0)
        {
            meleeAttackCurrentCooldown = seconds;
            StartCoroutine(DecreaseMeleeAttackCooldown());
        }
        else
        {
            meleeAttackCurrentCooldown = seconds;
        }
    }

    /// <summary>
    /// decreases the melee attack cooldown by 1 every second
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator DecreaseMeleeAttackCooldown()
    {
        while (meleeAttackCurrentCooldown > 0)
        {
            --meleeAttackCurrentCooldown;
            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// test if the enemy should take a attack
    /// </summary>
    /// <param name="ignoreAngle">whether to ignore the angle</param>
    /// <param name="rangeFactor">scale the view range</param>
    /// <returns>if in range</returns>
    private bool InMeleeAttackRange(bool ignoreAngle = false, float rangeFactor = 1f)
    {
        float theta = meleeAttackAngle / 2;
        Vector3 forward = transform.forward;
        Vector3 diffVec = currentTarget.gameObject.transform.position - transform.position;
        float angle = Vector3.Angle(forward, diffVec);
        if ((ignoreAngle || System.Math.Abs(angle) <= theta) && diffVec.magnitude <= rangeFactor * meleeAttackRadius)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    ///  whether enemy in battle mode
    /// </summary>
    /// <param name="battleDistance">if the distance between enemy and player then in to the battle mode</param>
    /// <returns>in the battle mode return true ;else return false</returns>
    public bool InBattleMode(float battleDistance)
    {
        if (GetDistanceBetweenEnemyAndPlayer() < battleDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region RangeAttack

    /// <summary>
    /// remaining cooldown of attack in seconds
    /// </summary>
    private float rangeAttackCurrentCooldown = 0f;

    /// <summary>
    /// ranged attack will not execute while on cooldown
    /// </summary>
    /// <param name="seconds">for seconds</param>
    public void StartRangeAttackCooldown(float seconds)
    {
        rangeAttackCurrentCooldown = seconds;
    }

    /// <summary>
    /// will be called when the enemy is ready for a ranged attack and in sight
    /// </summary>
    public virtual void OnRangeAttack()
    {
        StartRangeAttackCooldown(1f);
    }

    #endregion

    #region TargetSearching

    /// <summary>
    /// on a faction change, refresh the target
    /// </summary>
    public override void OnFactionChange()
    {
        base.OnFactionChange();
        FindNewTarget();
    }

    /// <summary>
    /// the CommonEntity which is the current target of this enemy
    /// </summary>
    protected CommonEntity currentTarget = null;

    /// <summary>
    /// currently targeted by this enemy
    /// </summary>
    /// <returns>the target</returns>
    public CommonEntity GetCurrentTarget()
    {
        return currentTarget;
    }

    /// <summary>
    /// possibly refreshes the current target
    /// </summary>
    protected void CheckTarget()
    {
        if (!currentTarget || !currentTarget.gameObject || currentTarget.IsDead() || GetDistanceBetweenEnemyAndPlayer() > followingRange)
        {
            FindNewTarget();
        }
    }

    /// <summary>
    /// looks for a new target
    /// </summary>
    protected void FindNewTarget()
    {
        currentTarget = FindNearestEnemy(detectionRange);
    }
    #endregion
}
