using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Ai for the boss enemy
/// </summary>
public class BossAI : EnemyAI
{
    /// <summary>
    /// exit door
    /// </summary>
    public Triggerable door;

    /// <summary>
    /// activate the movement and attack
    /// </summary>
    public bool run = true;

    /// <summary>
    /// transformer on which the Boss load his live
    /// </summary>
    public List<GameObject> transformer;

    /// <summary>
    /// shield of the boss
    /// </summary>
    public GameObject shield;

    /// <summary>
    /// animator of the boss model
    /// </summary>
    public Animator animator;

    /// <summary>
    /// prefab of the boss rocket
    /// </summary>
    public GameObject rangeProjektile;

    /// <summary>
    /// transform which will used by for the start position of the range projectile
    /// </summary>
    public Transform projektileStartPoint;

    /// <summary>
    /// the position from the last  update
    /// </summary>
    public Vector3 lastPosition;

    /// <summary>
    /// time to the next ranged attack
    /// </summary>
    public float rangeAtackCooldown = 20;

    /// <summary>
    /// current time to the next range attack
    /// </summary>
    public float curentRangeAtackCooldown = 20;

    /// <summary>
    /// delay for waiting of the animation before we instanciate the rocket of the ranged attack
    /// </summary>
    public float rangAnimationDelay = 0;

    /// <summary>
    /// texture for the health bar background
    /// </summary>
    public bool charge = false;

    /// <summary>
    /// speed in which the boss charges on transformers
    /// </summary>
    public float chargeSpeed = 100;

    /// <summary>
    /// Texture for healthBar
    /// </summary>
    public Texture healthBarTexture;

    /// <summary>
    /// Texture for the background of the health bar
    /// </summary>
    public Texture barBackgroundTexture;

    /// <summary>
    /// How to draw the skill point message
    /// </summary>
    private GUIStyle skillPointMessageguiStyle;

    /// <summary>
    /// Called by Unity
    /// </summary>
    protected override void Start()
    {
        shield.gameObject.SetActive(false);
        base.Start();
        UnityStart_GUI();
        animator = GetComponentInChildren<Animator>();
        lastPosition = transform.position;
        nma.Stop();
        EnableBlindFollowing();
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    protected override void Update()
    {
        
        if (run) //true if igor enter the boss room
        {
            #region Boss is active logic
            if (!IsDead())
            {
                #region Boss is alive logic
                if (!charge) //true if the boss is currently not charging
                {

                    #region boss logic
                    if (healthPoints >= maxHealthPoints / 5 || transformer.Count == 0) //check for need charging
                    {
                        #region Battel logic

                        shield.gameObject.SetActive(false);

                        SetCustomDestination(Game.GetIgor().transform.position); //set igor as moving target
                        //check for RangeAttack
                        if (PlayerInSight() && Vector3.Distance(Game.GetIgor().transform.position, transform.position) > meleeAttackRadius + 5 && curentRangeAtackCooldown == rangeAtackCooldown)
                        {
                            #region istaciate rangeattack
                            //range Attack
                            StartCoroutine("RangeAttack");


                            animator.SetInteger("Animation", 2);
                            nma.Stop();
                            rangAnimationDelay = 1.8f;
                            curentRangeAtackCooldown = 0;
                            ResetUpdate();
                            #endregion

                        }
                        else
                        {
                            if (rangAnimationDelay > 0)
                            {
                                #region wait for animation range Attack
                                rangAnimationDelay -= Time.deltaTime;

                                transform.LookAt(Game.GetIgor().transform.position);
                                ResetUpdate();
                                #endregion
                            }
                            else
                            {

                                nma.Resume();
                                //meeleAttack
                                if (Vector3.Distance(Game.GetIgor().transform.position, transform.position) < meleeAttackRadius && Game.GetIgorComponent().currentState != EntityState.dead)
                                {
                                    #region istaciate meele attack
                                    animator.SetInteger("Animation", 3);
                                    Skills.GetSkill("Fist").skill.OnUse(this.gameObject, null, 1);
                                    nma.Stop();
                                    #endregion
                                }
                                else
                                {
                                    #region set walk and stand animation
                                    //walk
                                    if (lastPosition != transform.position)
                                    {
                                        animator.SetInteger("Animation", 1);
                                    }
                                    else
                                    {
                                        animator.SetInteger("Animation", 0);
                                    }
                                    #endregion
                                }
                            }
                        }


                        #region update cooldowns
                        if (curentRangeAtackCooldown < rangeAtackCooldown)
                        {
                            curentRangeAtackCooldown += Time.deltaTime;
                        }
                        if (curentRangeAtackCooldown > rangeAtackCooldown)
                        {
                            curentRangeAtackCooldown = rangeAtackCooldown;
                        }

                        #endregion


                        #endregion
                    }
                    else
                    {
                        #region instanciat charging


                        shield.gameObject.SetActive(true);

                        if (rangAnimationDelay > 0) //waiting for the end of the range attack
                        {
                            //range Attack wait for animation
                            rangAnimationDelay -= Time.deltaTime;

                            transform.LookAt(Game.GetIgor().transform.position);
                            ResetUpdate();
                        }
                        else
                        {

                            #region walk to the next transfomer to charge
                            nma.Resume();

                            //#region remove distroyed Transformers from list
                            //List<GameObject> remove = new List<GameObject>();
                            //foreach (GameObject e in transformer)
                            //{
                            //    if (transformer[0].GetComponent<CommonEntity>().healthPoints <= 0)
                            //    {
                            //        remove.Add(e);
                            //    }
                            //}

                            //while (remove.Count > 0)
                            //{
                            //    transformer.Remove(remove[0]);
                            //    remove.Remove(remove[0]);
                            //}
                            //#endregion remove distroyed Transformers from list

                            if (transformer.Count > 0)
                            {
                                SetCustomDestination(transformer[0].transform.position);

                                if (Vector3.Distance(transform.position, transformer[0].transform.position) < 8)
                                {
                                    animator.SetInteger("Animation", 5);
                                    ResetUpdate();
                                    charge = true;
                                }
                                else
                                {
                                    //walk
                                    if (lastPosition != transform.position)
                                    {
                                        animator.SetInteger("Animation", 1);
                                    }
                                    else
                                    {
                                        animator.SetInteger("Animation", 0);
                                    }
                                }
                            }
                            else
                            {
                                // istaciate charging by reach the transformer
                                charge = false;
                            }
                            #endregion walk to the next transfomer to charge
                        }
                        #endregion instanciat charging
                    }
                    #endregion
                }
                else
                { //charging
                    #region  in charging logic



                    if (transformer.Count > 0) // check for remaning transformers(charging objects)
                    {


                        if (transformer[0].GetComponent<CommonEntity>().healthPoints > 0) //check for remaning live by the Transformer wich will use for Charging
                        {
                            animator.SetInteger("Animation", 5); //play charge animation
                            ResetUpdate();
                            SetCustomDestination(transformer[0].transform.position);
                            transformer[0].GetComponent<CommonEntity>().healthPoints -= Time.deltaTime * chargeSpeed;
                            this.healthPoints += Time.deltaTime * chargeSpeed * 2;

                            if (healthPoints > maxHealthPoints)
                            {
                                healthPoints = maxHealthPoints;
                            }

                        }
                        else
                        {
                            // abort charging if all transfomer live spend in charging
                            transformer.Remove(transformer[0]);
                            charge = false;
                        }
                    }
                    else
                    {
                        //exit charging when no transformers (charging objects)  remaining
                        charge = false;
                    }
                    #endregion
                }

                #endregion
            }
            else //if boss live <0
            {
                #region die animation and open exit door

                animator.SetInteger("Animation", -1);
                GetComponent<CapsuleCollider>().enabled = false;
                if (door != null)
                {
                    door.OnTrigger(this.gameObject, true);
                }
                GetComponent<SphereCollider>().enabled = true;
                shield.gameObject.SetActive(false);
                #endregion

            }



            #endregion boss is Activ Logic end

            //safe old position to reset base.update   transforms
            lastPosition = transform.position;
            base.Update();
        }
        else
        {
        //    healthPoints = maxHealthPoints;
            //waiting for entering igor into bossrom
            if (Vector3.Distance(Game.GetIgor().transform.position, transform.position) < 86)
            {
                run = true;
            }
        }
    }

    /// <summary>
    /// Let's the boss take damage
    /// </summary>
    /// <param name="hp">Amount of damage to deal</param>
    public override void TakeDamage(float hp)
    {
        if (transformer.Count > 0 && healthPoints - hp < maxHealthPoints * 0.2f)
        {
            healthPoints = maxHealthPoints * 0.2f - 1;
        }
        else
        {
            base.TakeDamage(hp);
        }
        
        
    }

    /// <summary>
    /// Activate rocket after 1 second 
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator RangeAttack()
    {
        yield return new WaitForSeconds(1f);
        Instantiate(rangeProjektile, projektileStartPoint.position, Quaternion.identity);
    }


    /// <summary>
    /// reset base.Update transform changes cause of base.update must be calculate all the time
    /// </summary>
    private void ResetUpdate()
    {
        transform.position = lastPosition;
    }


    /// <summary>
    /// Start animation with specific name.
    /// </summary>
    /// <param name="animation">the name of the animation.</param>
    public override void StartAnimation(string animation)
    {
        switch (animation)
        {
            case "Fist":
                animator.SetInteger("Animation", 3);
                break;
        }
    }

    /// <summary>
    /// Return the length of an animation.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <returns>The length.</returns>
    public override float GetAnimationLength(string animation)
    {
        switch (animation)
        {
            case "Fist":
                return 0.6f;
        }
        return 0f;
    }

    /// <summary>
    /// Returns whether this entity can be hacked by the hacking skill.
    /// </summary>
    /// <returns>Always false.</returns>
    public override bool IsHackable()
    {
        return false;
    }

    /// <summary>
    /// draw boss health bar
    /// </summary>
    private void OnGUI()
    {
        if (healthPoints > 0 && run)
        {
            // Bars
            float healthPercentage = healthPoints / maxHealthPoints;

            int xStart = 15;
            int xSize = Screen.width - 30;
            int yStart = Screen.height - 120;
            int ySize = Screen.height / 55;
            int yDistance = (int)(ySize * 1.2f);

            GUI.Label(new Rect(xStart, yStart - (2f * yDistance) - 1, 256, 30), "BOSS", skillPointMessageguiStyle);
            GUI.DrawTexture(new Rect(xStart, yStart, xSize, ySize), barBackgroundTexture, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(xStart, yStart, xSize * healthPercentage, ySize), healthBarTexture, ScaleMode.StretchToFill);
        }
    }

    /// <summary>
    /// Called by the Unity Start() method. Extracted for better readability.
    /// </summary>
    private void UnityStart_GUI()
    {
        skillPointMessageguiStyle = new GUIStyle();
        skillPointMessageguiStyle.wordWrap = true;
        skillPointMessageguiStyle.normal.textColor = new Color(0.0f, 0.2f, 0.0f);
        skillPointMessageguiStyle.alignment = TextAnchor.UpperLeft;
        skillPointMessageguiStyle.fontSize = 12;
        skillPointMessageguiStyle.fontStyle = FontStyle.Bold;
    }
}
