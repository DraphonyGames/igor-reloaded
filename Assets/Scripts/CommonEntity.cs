using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains common stuff for entities (e.g. player, enemies, etc..)
/// </summary>
public class CommonEntity : MonoBehaviour, IHitable
{
    #region State

    /// <summary>
    /// the state of the entity
    /// there are several helper functions like IsAlive and IsDead for convenience
    /// </summary>
    public enum EntityState
    {
        /// <summary>
        /// Entity is dead.
        /// </summary>
        dead,
        /// <summary>
        /// Usual state: Moving around, etc.
        /// </summary>
        normal,
        /// <summary>
        /// (start)state when pushing/pulling boxes around
        /// </summary>
        startpushing,
        /// <summary>
        /// actual push
        /// </summary>
        pushing
    }

    /// <summary>
    /// Current state (e.g. being alive, dead, etc)
    /// </summary>
    public EntityState currentState = EntityState.normal;

    /// <summary>
    /// whether the object is dead and thus can not perform any actions
    /// </summary>
    /// <returns>whether dead</returns>
    public bool IsDead()
    {
        return currentState == EntityState.dead;
    }

    /// <summary>
    /// we do what we must because we can
    /// </summary>
    /// <returns>if alive</returns>
    public bool IsAlive()
    {
        return !IsDead();
    }

    #endregion

    #region Speed

    /// <summary>
    /// speed of the enemy
    /// </summary>
    public float speed;

    #endregion

    #region Unity methods

    /// <summary>
    /// Use this for initialization, HAS TO BE CALLED FROM SUBCLASSES!
    /// </summary>
    protected virtual void Start()
    {
        damageOverTimeEffects = new List<DamageOverTime>();

        // Get distance from center to ground
        distanceToGround = collider.bounds.extents.y;
    }

    /// <summary>
    /// HAS TO BE CALLED FROM SUBCLASSES!
    /// </summary>
    protected virtual void Update()
    {
        RegenerateHealth();
        RegenerateMana();
        CalculateDamageOverTime();
    }

    #endregion

    #region Death

    /// <summary>
    /// Entity dies :(
    /// </summary>
    public virtual void Die()
    {
        currentState = EntityState.dead;
    }

    #endregion

    #region Damage

    /// <summary>
    /// Saves on effect of damage over time
    /// </summary>
    public class DamageOverTime
    {
        /// <summary>
        /// Last time (in seconds) since the damage happened
        /// </summary>
        public float lastTime;

        /// <summary>
        /// Damage taken
        /// </summary>
        public float damagePerInterval;

        /// <summary>
        /// interval of dot
        /// </summary>
        public float interval;

        /// <summary>
        /// time left
        /// </summary>
        public float timeLeft;
    }

    /// <summary>
    /// Takes information about current damage over time effects
    /// </summary>
    public List<DamageOverTime> damageOverTimeEffects;

    /// <summary>
    /// Deals hp damage to the object, see IHitable
    /// </summary>
    /// <param name="hp">damage dealt</param>
    public virtual void TakeDamage(float hp)
    {
        if (currentFaction == Faction.PLAYER)
        {
            hp *= Game.DifficultyMultiplier;
        }
        else
        {
            hp /= Game.DifficultyMultiplier;
        }

        healthPoints -= hp;
        lastHitTime = Time.time;

        if (healthPoints <= 0)
        {
            healthPoints = 0;

            if (currentState != EntityState.dead)
            {
                Die();
            }
        }
    }

    /// <summary>
    /// Deals damage to the object over a period of time in a certain interval, see IHitable
    /// </summary>
    /// <param name="damagePerInterval">Damage dealt per interval</param>
    /// <param name="interval">Interval in which damage is dealt</param>
    /// <param name="seconds">Time until effect stops</param>
    public void TakeDamageOverTime(float damagePerInterval, float interval, float seconds)
    {
        DamageOverTime dot = new DamageOverTime();
        dot.damagePerInterval = damagePerInterval;
        dot.interval = interval;
        dot.timeLeft = seconds;
        dot.lastTime = Time.time;

        damageOverTimeEffects.Add(dot);
    }

    /// <summary>
    /// Calculates damage over time at e.g. every frame
    /// </summary>
    private void CalculateDamageOverTime()
    {
        // Unity's Editor sometimes is weird.. just to be sure, even if set at Start() ...
        if (damageOverTimeEffects == null)
        {
            return;
        }

        // backwards, so removed elements don't change our index
        for (int i = damageOverTimeEffects.Count - 1; i >= 0; i--)
        {
            DamageOverTime current = damageOverTimeEffects[i];

            // if no more damage is to deal, remove element from list
            if (current.timeLeft <= 0)
            {
                damageOverTimeEffects.RemoveAt(i);
                continue;
            }

            if (current.lastTime + current.interval <= Time.time)
            {
                float hp = current.damagePerInterval;
                if (currentFaction == Faction.PLAYER)
                {
                    hp *= Game.DifficultyMultiplier;
                }
                else
                {
                    hp /= Game.DifficultyMultiplier;
                }

                current.timeLeft -= current.interval;
                current.lastTime = Time.time;
                healthPoints -= hp;
                lastHitTime = Time.time;
            }
        }

        if (healthPoints <= 0 && currentState != EntityState.dead)
        {
            Die();
        }
    }

    #endregion

    #region Health

    /// <summary>
    /// Current health points.
    /// 
    /// If not set initially, it is set to maxHealthPoints
    /// </summary>
    public float healthPoints = 100;

    /// <summary>
    /// Maximum health points.
    /// 
    /// If not set initially, it is set to 100
    /// </summary>
    public float maxHealthPoints = 100;

    /// <summary>
    /// Health regeneration OFF fight, per second
    /// 
    /// If not set it defaults to 5
    /// </summary>
    public float healthRegeneration = 5;

    /// <summary>
    /// Time in seconds since last hit needed until regeneration
    /// 
    /// If not set it defaults to 10
    /// </summary>
    public float timeUntilRegeneration = 10;

    /// <summary>
    /// When did we get hit the last time? (Both direct hit and damage over time)
    /// </summary>
    protected float lastHitTime = -1000; // We should definetly be able to regenerate health at start

    /// <summary>
    /// When did we attack the last time?
    /// </summary>
    protected float lastAttackTime = -1000; // We should definetly be able to regenerate health at start

    /// <summary>
    /// Regenerate health in the set amount (usually out of fight)
    /// </summary>
    private void RegenerateHealth()
    {
        if ((!Game.IsPaused) && lastHitTime + timeUntilRegeneration <= Time.time && lastAttackTime + timeUntilRegeneration <= Time.time)
        {
            InstantaneousHeal(healthRegeneration * Time.deltaTime);
        }
    }

    /// <summary>
    /// Heal entity
    /// </summary>
    /// <param name="hp">Amount to heal</param>
    public void InstantaneousHeal(float hp)
    {
        healthPoints += hp;

        if (healthPoints > maxHealthPoints)
        {
            healthPoints = maxHealthPoints;
        }
    }

    #endregion

    #region Mana

    /// <summary>
    /// Energy used for jumping/shooting, etc
    /// 
    /// set to maxMana, if unset
    /// </summary>
    public float mana = 100;

    /// <summary>
    /// max mana
    /// 
    /// Set to 100, if unset
    /// </summary>
    public float maxMana = 100;

    /// <summary>
    /// holds the time when mana is used last
    /// </summary>
    protected float lastManaUsageTime = -1000;

    /// <summary>
    /// Mana regeneration  per second
    /// 
    /// If not set it defaults to 10
    /// </summary>
    public float manaRegeneration = 10;

    /// <summary>
    /// Time in seconds since last mana usage needed until mana regeneration
    /// 
    /// If not set it defaults to 10
    /// </summary>
    public float timeUntilManaRegeneration = 10;

    /// <summary>
    /// Regenerate mana in the set amount
    /// </summary>
    private void RegenerateMana()
    {
        if (!Game.IsPaused && lastManaUsageTime + timeUntilManaRegeneration <= Time.time)
        {
            InstantaneousManaRegeneration(manaRegeneration * Time.deltaTime);
        }
    }

    /// <summary>
    /// regenerate mana
    /// </summary>
    /// <param name="addMana">how much</param>
    public void InstantaneousManaRegeneration(float addMana)
    {
        mana += addMana;

        if (mana > maxMana)
        {
            mana = maxMana;
        }
    }

    #endregion

    #region Distance Checking

    /// <summary>
    /// Distance to the lower bounding box.
    /// </summary>
    private float distanceToGround;

    /// <summary>
    /// Checks whether entity is currently standing on something solid.
    /// </summary>
    /// <returns>True if it's on the ground, else false.</returns>
    public virtual bool IsOnGround()
    {
        //TODO: get ReycastHit info
        return Physics.Raycast(collider.bounds.center, -Vector3.up, distanceToGround + 0.1f);
    }

    /// <summary>
    /// Get the distance the entity is above something.
    /// </summary>
    /// <returns>The vertical distance to the next thing below the entity.</returns>
    public float DistanceToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(collider.bounds.center, -Vector3.up, out hit))
        {
            return collider.bounds.center.y - collider.bounds.extents.y - hit.point.y;
        }
        return Mathf.Infinity;
    }

    /// <summary>
    /// Get the distance the entity is below something.
    /// </summary>
    /// <returns>The vertical distance to the next thing above the entity.</returns>
    public float DistanceToRoof()
    {
        RaycastHit hit;
        if (Physics.Raycast(collider.bounds.center, Vector3.up, out hit))
        {
            return hit.point.y - collider.bounds.center.y - collider.bounds.extents.y;
        }
        return Mathf.Infinity;
    }

    #endregion

    #region Locking

    /// <summary>
    /// Our lock variable.
    /// </summary>
    public int _lock;

    /// <summary>
    /// Return true if entity is locke.
    /// </summary>
    /// <returns>True iff locked.</returns>
    public bool IsLocked()
    {
        return _lock > 0;
    }

    /// <summary>
    /// Lock entity.
    /// </summary>
    public void Lock()
    {
        ++_lock;
    }

    /// <summary>
    /// Unlock entity.
    /// </summary>
    public void Unlock()
    {
        --_lock;
    }

    #endregion

    #region Skills

    /// <summary>
    /// Start animation with specific name.
    /// </summary>
    /// <param name="animation">the name of the animation.</param>
    public virtual void StartAnimation(string animation)
    {
    }

    /// <summary>
    /// Return the length of an animation.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <returns>The length.</returns>
    public virtual float GetAnimationLength(string animation)
    {
        return 0f;
    }

    /// <summary>
    /// Returns whether this entity can be hacked by the hacking skill.
    /// </summary>
    /// <returns>Always true.</returns>
    public virtual bool IsHackable()
    {
        return true;
    }

    #endregion

    #region FactionBehavior

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    public enum Faction
    {
        /// <summary>
        /// No faction (neutral).
        /// </summary>
        NONE,
        /// <summary>
        /// Plays with the player.
        /// </summary>
        PLAYER,
        /// <summary>
        /// Plays against the player.
        /// </summary>
        ENEMY
    }

    /// <summary>
    /// The faction of the enemy determines what objects are considered hostile. For enemies, this is usually Igor (or one of his friends). For Igor's friends, any "Enemy"-tagged unit is hostile
    /// NOTE: standard faction is assumed by different scripts to be ENEMY
    /// </summary>
    public Faction currentFaction = Faction.ENEMY;

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    /// <returns>the faction</returns>
    public Faction GetFaction()
    {
        return currentFaction;
    }

    /// <summary>
    /// this function is called when a CommonEntity changes its faction
    /// use it for example to search for new targets..
    /// </summary>
    public virtual void OnFactionChange()
    {

    }

    /// <summary>
    /// sets the current faction
    /// </summary>
    /// <param name="newFaction">new faction</param>
    public void SetFaction(Faction newFaction)
    {
        currentFaction = newFaction;
        OnFactionChange();
    }

    /// <summary>
    /// changes the faction of the enemy
    /// </summary>
    public void ChangeFaction()
    {
        switch (currentFaction)
        {
            case Faction.NONE:
            case Faction.PLAYER:
                currentFaction = Faction.ENEMY;
                break;
            case Faction.ENEMY:
                currentFaction = Faction.PLAYER;
                break;
        }
        OnFactionChange();
    }

    /// <summary>
    /// checks whether the other object is considered a friend by this object
    /// </summary>
    /// <param name="other">to check</param>
    /// <returns>if friend</returns>
    public bool IsFriend(CommonEntity other)
    {
        return IsFriend(other.currentFaction);
    }

    /// <summary>
    /// returns whether the other faction is allied to this object
    /// </summary>
    /// <param name="otherFaction">to check</param>
    /// <returns>if friend</returns>
    public bool IsFriend(Faction otherFaction)
    {
        // am I neutral? I HATE THE WORLD
        if (currentFaction == Faction.NONE)
        {
            return false;
        }
        return otherFaction == currentFaction;
    }

    #endregion

    #region Enemies

    /// <summary>
    /// finds the nearest enemy matching the condition
    /// </summary>
    /// <param name="detectionRange">range to detect</param>
    /// <param name="angle">angle to detect</param>
    /// <returns>i just said that</returns>
    public CommonEntity FindNearestEnemy(float detectionRange, float angle = 180f)
    {
        // targets usually have a collider and that is exactly what will be looked for right now!
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);

        // look for the nearest possible target
        // note that this has the following pitfall:
        // when an enemy could see Igor and can not see Igor's helper but the latter is closer, it will ignore Igor
        // todo: fix when anyone notices that..
        CommonEntity other = null;
        float otherDistance = 0f;

        foreach (Collider collider in colliders)
        {
            // is it even a possible target..
            CommonEntity commonEntity = collider.gameObject.GetComponent<CommonEntity>();
            if (!commonEntity || commonEntity.IsDead() || collider.gameObject == gameObject)
            {
                continue;
            }
            if (IsFriend(commonEntity))
            {
                continue;
            }
            if (Vector3.Angle(transform.forward, collider.gameObject.transform.position - transform.position) > angle)
            {
                continue;
            }

            float distance = (transform.position - commonEntity.gameObject.transform.position).magnitude;
            if (other == null || distance < otherDistance)
            {
                other = commonEntity;
                otherDistance = distance;
            }
        }

        return other;
    }

    #endregion
}
