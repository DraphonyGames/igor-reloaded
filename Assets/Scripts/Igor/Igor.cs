using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Behavior class for our main character.
/// <remarks>Attention: The CameraTarget object which is attached to Igor and which is set as the target for the IgorCamera has to be the second child.</remarks>
/// </summary>
public class Igor : CommonEntity
{
    #region Camera Stuff

    /// <summary>
    /// Should Igor be displayed or not?
    /// </summary>
    private bool hidden = false;

    /// <summary>
    /// Property to get and set whether Igor should be hidden (not displayed) or not.
    /// </summary>
    public bool Hidden
    {
        get
        {
            return hidden;
        }

        set
        {
            if (value != hidden)
            {
                //GetComponentInChildren<ParticleSystem>().renderer.enabled = _hidden;
                foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = hidden;
                }

                hidden = value;
            }
        }
    }

    /// <summary>
    /// How fast should Igor turn when beginning to walk after mouse rotation while standing.
    /// </summary>
    public float turningSpeed = 10f;

    /// <summary>
    /// Rotate Igor to our camera direction smoothly (has to be called multiple times).
    /// </summary>
    public void RotateToCameraSmoothly()
    {
        // Get the camera target object
        Transform cameraTarget = transform.GetChild(1);

        // Calculate smooth angle movement
        float angle = Mathf.LerpAngle(0, cameraTarget.localEulerAngles.y, Time.deltaTime * turningSpeed);

        // Rotate Igor by that angle and the camera target in the opposite direction to stay globally static
        transform.Rotate(0f, angle, 0f);
        cameraTarget.Rotate(0f, -angle, 0f);
    }

    /// <summary>
    /// Rotate Igor to our camera direction in one step (aka directly).
    /// </summary>
    public void RotateToCameraHard()
    {
        // Get the camera target object
        Transform cameraTarget = transform.GetChild(1);
        float angle = cameraTarget.localEulerAngles.y;

        // Rotate Igor by that angle and the camera target in the opposite direction to stay globally static
        transform.Rotate(0f, angle, 0f);
        cameraTarget.Rotate(0f, -angle, 0f);
    }

    #endregion

    #region Initial Values for Passive Skills

    /// <summary>
    /// Initial health points without skill points
    /// </summary>
    public float initialMaxHealthPoints = 100;

    /// <summary>
    /// Initial mana points without skill points
    /// </summary>
    public float initialMaxMana = 100;

    /// <summary>
    /// Initial time until health regeneration without skill points
    /// </summary>
    public float initialTimeUntilRegeneration = 10;

    /// <summary>
    /// Initial health regeneration without skill points
    /// </summary>
    public float initialHealthRegeneration = 10;

    /// <summary>
    /// Initial mana regeneration without skill points
    /// </summary>
    public float initialManaRegeneration = 5;

    /// <summary>
    /// Initial jump force without skill points
    /// </summary>
    public float initialJumpForce = 5.0f;

    /// <summary>
    /// Initial jump regeneration without skill points
    /// </summary>
    public float initialJumpForceRegeneration = 1.0f;

    /// <summary>
    /// Initial speed without skill points
    /// </summary>
    public float initialSpeed = 10.0f;
    #endregion

    #region Experience and Skill System

    /// <summary>
    /// Defines how much skill points igor gets at level up
    /// </summary>
    public int skillpointsPerLevel = 1;

    /// <summary>
    /// Unused skill points
    /// </summary>
    public int skillpoints;

    /// <summary>
    /// Current level
    /// </summary>
    public int level = 1;

    /// <summary>
    /// experience at CURRENT level, not overall experience.
    /// </summary>
    public int experience = 0;

    /// <summary>
    /// experience at CURRENT level, not overall experience. read only.
    /// </summary>
    public int Experience
    {
        get { return experience; }
    }

    /// <summary>
    /// How much experience is left to the next level
    /// </summary>
    private int ExpUntilLevelUp
    {
        get
        {
            return (int)(10 * Mathf.Exp((level - 1) * 0.3f));
        }
    }

    /// <summary>
    /// Called by the Unity Start() method. Extracted for better readability.
    /// </summary>
    private static void UnityStart_Skills()
    {
        // this instantiates the skill stuff when called for the first time
        bool startedForTheVeryFirstTime = Skills.InstantiateSkills();
        SkillBar skillBar = SkillBar.GetInstance();
        SkillTree.GetInstance();

        if (startedForTheVeryFirstTime)
        {
            string[] stdSkills = new string[]
            {
                "Fist",
                "Laser",
                "Max Health Points",
                "Health Regeneration",
                "Time Until Regeneration",
                "Speed",
                "Jump Force",
                "Jump Force Regeneration",
                "Max Mana",
                "Mana Regeneration",
            };
            int skillCount = -1;
            foreach (string skill in stdSkills)
            {
                ++skillCount;
                Skills.SetSkillLevel(skill, 1);
                Skills.SkillData skillData = Skills.GetSkill(skill);
                if (!skillData.skill.IsPassive())
                {
                    skillBar.AssignSlot(skillCount, skillData.skill.GetName(), SkillBar.SkillBarItem.ItemType.SKILL, skillData.skill.GetIconTexture());
                }
            }
        }
        else
        {
            // refresh all the existing skills
            // otherwise, passive skills would not receive OnLevelUp and thus Igor would starve to death (because of lacking mana regeneration)
            foreach (Skills.SkillData skill in Skills.GetSkills())
            {
                Skills.SetSkillLevel(skill.skill.GetName(), skill.level);
            }
        }
    }

    /// <summary>
    /// resets experience points
    /// </summary>
    public void ResetExperience()
    {
        experience = 0;
    }

    /// <summary>
    /// adds experience points to igor
    /// </summary>
    /// <param name="amount">amount of experience points</param>
    public void AddExperience(uint amount)
    {
        experience += (int)amount;

        if (experience >= ExpUntilLevelUp)
        {
            skillpoints += skillpointsPerLevel;

            experience -= ExpUntilLevelUp;
            level += 1; // has to be AFTER the above line, because expUntilLevelUp changes!

            if (experience > ExpUntilLevelUp) // we need for the next level at least as much as for the current
            {
                AddExperience(0); // recursive call, so we can get multiple levels at once
            }
        }
    }
    #endregion

    #region GUI
    /// <summary>
    /// Texture for healthBar
    /// </summary>
    public Texture healthBarTexture;

    /// <summary>
    /// Texture for manaBar
    /// </summary>
    public Texture manaBarTexture;

    /// <summary>
    /// Texture for jumpBar
    /// </summary>
    public Texture jumpBarTexture;

    /// <summary>
    /// Texture for experience Bar
    /// </summary>
    public Texture expBarTexture;

    /// <summary>
    /// Texture for the background of the bars
    /// </summary>
    public Texture barBackgroundTexture;

    /// <summary>
    /// How to draw the skill point message
    /// </summary>
    private GUIStyle skillPointMessageguiStyle;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Use this for initialization.
    /// Attention: Initialization in Awake state is very important,please don't move it to Start()
    /// </summary>
    protected void Awake()
    {
        UnityStart_GUI();
        UnityStart_Skills();
    }
    /// <summary>
    /// Use this for initialization.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        UnityStart_Pushing();
        Screen.showCursor = false;
        animator = GetComponent<Animator>();

        SetFaction(CommonEntity.Faction.PLAYER);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        /* here, to prevent weird behaviour when letting boxes go */
        if (currentState == EntityState.startpushing)
        {
            UnityUpdate_State_StartPushing();
        }
        else if (currentState == EntityState.pushing)
        {
            UnityUpdate_State_Pushing();
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    protected void FixedUpdate()
    {
        RegenerateJumpForce();

        // Speed up animation when in melee attack
        AnimatorStateInfo curState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);
        if (curState.IsName("Base Layer.Attack1") || nextState.IsName("Base Layer.Attack1"))
        {
            animator.speed = ((MeleeAttack)Skills.GetSkill("Fist").skill).MeleeAttackSpeed;
        }
        else
        {
            animator.speed = 1;
        }

        UnityUpdate_JumpAudioVolume();

        // resets the current movement direction, so that Igor does not keep movement from the last frame
        ResetCurrentMovement();

        if (currentState == EntityState.normal)
        {
            UnityUpdate_State_Normal();
        }
        else if (currentState == EntityState.dead)
        {
            UnityUpdate_State_Dead();
        }

        // execute movement after all possible speed changes have been applied
        ExecCurrentMovement();
    }

    /// <summary>
    /// Called by the Unity Start() method. Extracted for better readability.
    /// </summary>
    private void UnityUpdate_State_Normal()
    {
        if (!Game.IsPaused && !Game.IsCutscene && !Game.isMenuOpen)
        {
            Jump();
            MoveOnPlane();
            FallDown(); // always call this!
            CheckUsableObjects();
        }
        else
        {
            if (inAction == 0 && !Game.IsCutscene && !IsLocked())
            {
                animator.SetInteger("AnimationIndex", 0);
            }
            FallDown(); // always call this!
        }

        if (healthPoints <= 0)
        {
            animator.SetInteger("AnimationIndex", -1);
        }

        if (Input.GetKey(KeyCode.Delete) && Input.GetKey(KeyCode.X))
        {
            TakeDamage(100000);
        }

        /* Allow cheating if in editor */
        if (Application.isEditor && Input.GetKeyDown("o"))
        {
            ItemDropper.DynamicDrop(transform, 10f);
        }
    }
    #endregion

    #region Movement

    /// <summary>
    /// the direction that Igor will move this frame
    /// </summary>
    private Vector3 currentMovementDirection = Vector3.zero;

    /// <summary>
    /// don't fall down every frame when flying at the ceiling..
    /// </summary>
    private bool jumpWasCappedAtCeilingLastMove = false;

    /// <summary>
    /// define the factor to lower the backwards and sideways speed in relation to forward speed
    /// </summary>
    public float backwardSlowdown = 0.5f;

    /// <summary>
    /// should be called once per frame before the movement is executed
    /// </summary>
    private void ResetCurrentMovement()
    {
        currentMovementDirection = Vector3.zero;
    }

    /// <summary>
    /// if you want to move Igor, use this function
    /// </summary>
    /// <param name="direction">direction to move into</param>
    private void AddCurrentMovement(Vector3 direction)
    {
        currentMovementDirection += direction;
    }

    /// <summary>
    /// called once per frame to execute the movement
    /// </summary>
    private void ExecCurrentMovement()
    {
        rigidbody.AddForce(-rigidbody.velocity + transform.rotation * currentMovementDirection, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Move forward, backward, left and right if user presses the corresponding keys.
    /// </summary>
    private void MoveOnPlane()
    {
        float inputVertical = Input.GetAxis("Vertical");
        float inputHorizontal = Input.GetAxis("Horizontal");

        if (inputHorizontal != 0 || inputVertical != 0)
        {
            // Rotate Igor to the correct direction
            RotateToCameraSmoothly();

            if (inputVertical != 0)
            {
                // Move Igor forward  / backward
                float movementForwardBackward = inputVertical * speed;
                if (movementForwardBackward * Time.deltaTime > 0)
                {
                    if (inAction == 0 && !inJump && !IsLocked())
                    {
                        animator.SetInteger("AnimationIndex", 1);
                    }
                }
                else
                {
                    movementForwardBackward *= backwardSlowdown;
                    if (inAction == 0 && !inJump && !IsLocked())
                    {
                        animator.SetInteger("AnimationIndex", 2);
                    }
                }
                AddCurrentMovement(Vector3.forward * movementForwardBackward);

                // right left move additional to forward move
                float movementLeftRight = inputHorizontal * speed * backwardSlowdown;
                AddCurrentMovement(Vector3.right * movementLeftRight);
            }
            else
            {
                // Move Igor left / right without forwardmove
                float movementLeftRight = inputHorizontal * speed * backwardSlowdown;
                AddCurrentMovement(Vector3.right * movementLeftRight);

                if (inAction == 0 && !inJump && !IsLocked())
                {
                    animator.SetInteger("AnimationIndex", 2);
                }
            }
        }
        else
        {
            if (inAction == 0 && !inJump && !IsLocked())
            {
                animator.SetInteger("AnimationIndex", 0);
            }
        }
    }
    #endregion

    #region GUI
    /// <summary>
    /// Needed for game over screen
    /// </summary>
    public Texture2D gameOverImage;

    /// <summary>
    /// Time until a objectHelpMessage should be shown
    /// </summary>
    private float objectHelpMessageDelay = 0.07f;

    /// <summary>
    /// Time already waited
    /// </summary>
    private float objectHelpMessageWaited = 0;

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

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void OnGUI()
    {
        // Bars
        float healthPercentage = healthPoints / maxHealthPoints;
        float manaPercentage = mana / maxMana;
        float jumpPercentage = jumpCurrentForce / jumpForce;
        float expPercentage = Experience / (float)ExpUntilLevelUp;

        int xStart = 15;
        int xSize = Screen.width / 4;
        int yStart = 15;
        int ySize = Screen.height / 55;
        int yDistance = (int)(ySize * 1.2f);

        // Background
        if (skillpoints == 0)
        {
            MessageBoard.DrawMessageBackground(xStart, yStart, xSize, (int)(ySize * 3.7f));
        }
        else
        {
            MessageBoard.DrawMessageBackground(xStart, yStart, xSize, (int)(ySize * 4.4f));
        }

        GUI.DrawTexture(new Rect(xStart, yStart, xSize, ySize), barBackgroundTexture, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(xStart, yStart, xSize * healthPercentage, ySize), healthBarTexture, ScaleMode.StretchToFill);

        GUI.DrawTexture(new Rect(xStart, yStart + yDistance, xSize, ySize), barBackgroundTexture, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(xStart, yStart + yDistance, xSize * manaPercentage, ySize), manaBarTexture, ScaleMode.StretchToFill);

        GUI.DrawTexture(new Rect(xStart, yStart + (2 * yDistance), xSize, ySize), barBackgroundTexture, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(xStart, yStart + (2 * yDistance), xSize * jumpPercentage, ySize), jumpBarTexture, ScaleMode.StretchToFill);

        GUI.DrawTexture(new Rect(xStart, yStart + (3 * yDistance), xSize, ySize * 0.3f), barBackgroundTexture, ScaleMode.StretchToFill, true);
        GUI.DrawTexture(new Rect(xStart, yStart + (3 * yDistance), xSize * expPercentage, ySize * 0.3f), expBarTexture, ScaleMode.StretchToFill);

        if (skillpoints > 0)
        {
            GUI.Label(new Rect(xStart, yStart + (3.3f * yDistance) - 1, 256, 30), skillpoints.ToString() + " skillpoints available (press k for menu)", skillPointMessageguiStyle);
        }

        // Allows resetting the timer
        bool drawObjectHelpMsg = false;

        // hint that Igor is able to use an object in front of him
        if (currentHighlightedObject != null && currentState == EntityState.normal)
        {
            DrawObjectHelpMessage(currentHighlightedObject.gameObject, 95, 30, "press [E] to use " + currentHighlightedObject.name);
            drawObjectHelpMsg = true;
        }

        // Hint when pulling/pushing something
        if (currentState == EntityState.pushing && pushLeft == 0)
        {
            DrawObjectHelpMessage(currentPushBox.gameObject, 100, 45, "press [W] to push\npress [S] to pull\npress [E] to leave");
            drawObjectHelpMsg = true;
        }

        if (!drawObjectHelpMsg)
        {
            objectHelpMessageWaited = 0;
        }

        // Game over :(
        if (currentState == EntityState.dead)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), gameOverImage, ScaleMode.StretchToFill);
        }
    }

    /// <summary>
    /// Renders a beautiful help message for an object
    /// </summary>
    /// <param name="obj">The object</param>
    /// <param name="rectSizeX">How big the rectangle should be</param>
    /// <param name="rectSizeY">How big the rectangle should be</param>
    /// <param name="text">The text to display</param>
    private void DrawObjectHelpMessage(GameObject obj, int rectSizeX, int rectSizeY, string text)
    {
        // Only draw after a certain delay
        if (objectHelpMessageWaited >= objectHelpMessageDelay)
        {
            Bounds bounds = (obj.collider != null) ? obj.collider.bounds : obj.renderer.bounds;
            Vector2 screenCoords = Camera.main.WorldToScreenPoint(bounds.center + Vector3.up * (4f * bounds.extents.y));
            screenCoords.y = Screen.height - screenCoords.y;
            Rect rect = new Rect(Mathf.Max(screenCoords.x - 40f, 10), Mathf.Max(screenCoords.y - 20f, 10), rectSizeX, rectSizeY);
            MessageBoard.DrawMessageBackground(rect);
            GUI.Label(rect, text, skillPointMessageguiStyle);
        }

        objectHelpMessageWaited += Time.deltaTime;
    }

    #endregion

    #region Usage

    /// <summary>
    /// define witch attack  is current run  (0 means no attack is run)
    /// </summary>
    public int inAction = 0;

    /// <summary>
    /// used to highlight usable objects
    /// </summary>
    public GameObject sparklingParticlesPrefab;

    /// <summary>
    /// the currently active effect to highlight the closest, usable object
    /// </summary>
    private GameObject currentHighlightingEffect;

    /// <summary>
    /// used for drawing "Use XYZ"-text in OnGUI
    /// </summary>
    private GameObject currentHighlightedObject;

    /// <summary>
    /// safe the Effect from the Item witch is used on igor
    /// </summary>
    private GameObject useEffectItem;

    /// <summary>
    /// look for and highlight usable objects (that are tagged "Usable") in front of Igor.
    /// Use them on key stroke.
    /// </summary>
    private void CheckUsableObjects()
    {
        if (this.IsLocked())
        {
            return;
        }

        GameObject found = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5.0f);
        for (int i = 0; i < hitColliders.Length; ++i)
        {
            Collider other = hitColliders[i];
            GameObject otherGameObject = other.gameObject;

            // only allow objects in cone in front of Igor
            if (Vector3.Angle(otherGameObject.transform.position - transform.position, transform.forward) > 45f)
            {
                continue;
            }

            Bounds bounds = (otherGameObject.renderer != null) ? otherGameObject.renderer.bounds : otherGameObject.collider.bounds;
            float maximumUsageDistance = bounds.extents.magnitude + 2f;
            if ((transform.position - otherGameObject.transform.position).magnitude > maximumUsageDistance)
            {
                continue;
            }


            if (other.tag.Contains("Usable"))
            {
                found = other.gameObject;
                break;
            }
        }

        // clean up old effect if necessary
        if (!found)
        {
            currentHighlightedObject = null;

            if (currentHighlightingEffect != null)
            {
                Destroy(currentHighlightingEffect);
            }

            return;
        }
        else if (found)
        {
            if (currentHighlightingEffect == null)
            {
                currentHighlightingEffect = (GameObject)Instantiate(sparklingParticlesPrefab, found.transform.position, Quaternion.identity);
            }

            // found for the very first time?
            if (!currentHighlightedObject)
            {
                if (found.renderer != null)
                {
                    currentHighlightingEffect.transform.position = found.renderer.bounds.center;
                    currentHighlightingEffect.transform.localScale = found.renderer.bounds.size;
                }
                else
                {
                    currentHighlightingEffect.transform.position = found.collider.bounds.center;
                    currentHighlightingEffect.transform.localScale = found.collider.bounds.size;
                }
                if (currentHighlightingEffect.transform.localScale == Vector3.zero)
                {
                    currentHighlightingEffect.transform.localScale = new Vector3(3f, 3f, 3f);
                }
            }

            // for the GUI text later
            currentHighlightedObject = found;

            if (Input.GetButtonDown("Use") || Input.GetKeyDown("e"))
            {
                IUsable usableObject = (IUsable)found.GetComponent(typeof(IUsable));
                if (usableObject != null)
                {
                    usableObject.OnUse(gameObject);
                }
            }
        }
    }
    #region UseObjectAnimation

    /// <summary>
    /// object which will receive an OnUse callback after the use animation has been played
    /// the object should only use a if(!Igor.PlayUseObjectInFrontAnimation(this)) to check for validity
    /// </summary>
    private GameObject useObjectInFrontCallbackObject = null;

    /// <summary>
    /// plays an animation and locks the hands
    /// </summary>
    /// <param name="usable">usable object</param>
    /// <returns>true if the object can be used instantly, otherwise false and might call again</returns>
    public bool PlayUseObjectInFrontAnimation(GameObject usable = null)
    {
        if (usable.GetComponent<MonoBehaviour>() == null)
        {
            return false; // this doesn't even make sense
        }

        // the object has already been in use for some time?
        if (usable != null && usable == useObjectInFrontCallbackObject)
        {
            return true;
        }

        if (IsLocked())
        {
            return false;
        }
        Lock();

        useObjectInFrontCallbackObject = usable;
        inAction = -1;
        animator.SetInteger("AnimationIndex", 21);
        StartCoroutine("UseObjectInFrontAnimation");

        // usable objects will receive a callback later
        if (usable != null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// unlocks and stops the animation eventually, too
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator UseObjectInFrontAnimation()
    {
        yield return new WaitForSeconds(1f);

        // actually use object?
        if (useObjectInFrontCallbackObject != null)
        {
            // turning and running away don't count
            bool validMove = true;
            Bounds bounds = (useObjectInFrontCallbackObject.renderer != null) ? useObjectInFrontCallbackObject.renderer.bounds : useObjectInFrontCallbackObject.collider.bounds;
            float maximumUsageDistance = bounds.extents.magnitude + 2f;
            if (Vector3.Angle(transform.forward, useObjectInFrontCallbackObject.transform.position - transform.position) > 45f)
            {
                validMove = false;
            }
            if ((transform.position - useObjectInFrontCallbackObject.transform.position).magnitude > maximumUsageDistance)
            {
                validMove = false;
            }

            if (validMove)
            {
                // now look for IUsable components and use them....
                foreach (MonoBehaviour mono in useObjectInFrontCallbackObject.GetComponents<MonoBehaviour>())
                {
                    IUsable usable = mono as IUsable;
                    if (usable == null)
                    {
                        continue;
                    }
                    usable.OnUse(gameObject);
                }
            }
            useObjectInFrontCallbackObject = null;
        }

        inAction = 0;
        animator.SetInteger("AnimationIndex", 0);
        Unlock();
    }

    #endregion

    /// <summary>
    /// Play the animation for the Item Use
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator UseItemSelfAnimation()
    {
        yield return new WaitForSeconds(1);
        inAction = 0;
        Instantiate(useEffectItem, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        animator.SetInteger("AnimationIndex", 0);
    }

    /// <summary>
    /// play Animation and particle effect for used item of igor
    /// </summary>
    /// <param name="partikleffect">particle effect for the item which is used</param>
    public void UseItemSelf(GameObject partikleffect)
    {
        inAction = -1;
        animator.SetInteger("AnimationIndex", 3);
        StartCoroutine("UseItemSelfAnimation");

        useEffectItem = partikleffect;
    }
    #endregion

    #region Death

    /// <summary>
    /// Needed for game over screen
    /// </summary>
    private float timeSinceIgorDied = 0;

    /// <summary>
    /// Is called by the Unity Update() method. Extracted for better readability.
    /// </summary>
    private void UnityUpdate_State_Dead()
    {
        healthPoints = 0;
        timeSinceIgorDied += Time.deltaTime;

        if (timeSinceIgorDied > 5)
        {
            //Please don't remove this line ,it's important !!!
            Skills.SkillsClear();
            Application.LoadLevel(1); // 1 = Start screen
        }

        // move to the ground when dead, to it faster, though
        FallDown(4f);
    }

    /// <summary>
    /// When Igor dies we play the die animation and disable the jetpack.
    /// </summary>
    public override void Die()
    {
        base.Die();
        animator.SetInteger("AnimationIndex", -1);
        GetComponentInChildren<ParticleSystem>().enableEmission = false;
    }
    #endregion

    #region Pushing
    /// <summary>
    /// Our real size, read only
    /// </summary>
    public Vector3 RealSize
    {
        get { return realSize; }
    }

    /// <summary>
    /// Our real size: internal variable
    /// </summary>
    private Vector3 realSize;

    /// <summary>
    /// When pushing: current box
    /// </summary>
    private PushableBox currentPushBox = null;

    /// <summary>
    /// how much we still have to push
    /// </summary>
    private float pushLeft = 0;

    /// <summary>
    /// How long we are in startPushing, so we can abort after a certain time
    /// </summary>
    private float startPushingTime;

    /// <summary>
    /// Maximum time until we abort startPushing
    /// </summary>
    private const float maxStartPushingTime = 2.5f;

    /// <summary>
    /// Is called by the Unity Start() method. Extracted for better readability.
    /// </summary>
    private void UnityStart_Pushing()
    {
        // get real size
        SkinnedMeshRenderer[] skinMeshs = GetComponentsInChildren<SkinnedMeshRenderer>();
        float skinSizeX = -1;
        float skinSizeY = -1;
        float skinSizeZ = -1;

        foreach (SkinnedMeshRenderer smr in skinMeshs)
        {
            if (skinSizeX < smr.localBounds.size.x)
            {
                skinSizeX = smr.localBounds.size.x;
            }

            if (skinSizeY < smr.localBounds.size.y)
            {
                skinSizeY = smr.localBounds.size.y;
            }

            if (skinSizeZ < smr.localBounds.size.z)
            {
                skinSizeZ = smr.localBounds.size.z;
            }
        }

        realSize = new Vector3(skinSizeX, skinSizeY, skinSizeZ);
    }

    /// <summary>
    /// Is called by the Unity Update() method. Extracted for better readability.
    /// </summary>
    private void UnityUpdate_State_Pushing()
    {
        FallDown();

        // abort if the distance is too high (e.g. box falls down)
        if ((transform.position - currentPushBox.transform.position).magnitude > (currentPushBox.RealSize.x * 0.5f) + 3.5f)
        {
            /* reset pushbox, if the pushbox is not falling down, so we don't create unsolveable puzzles */
            if (!currentPushBox.FallingDown)
            {
                currentPushBox.LoadOldPosition();
            }

            EndPushing();
            return;
        }

        if (pushLeft == 0)
        {
            /* Initiate push move */
            float vertInput = Input.GetAxis("Vertical");

            if (Mathf.Abs(vertInput) > 0.2f && currentPushBox.PushingAllowed(gameObject, vertInput > 0))
            {
                pushLeft = Mathf.Sign(vertInput) * currentPushBox.transform.localScale.x;

                /* Save position of the box for possible revert later */
                currentPushBox.SavePosition();
            }
            else if (Input.GetButtonDown("Use") || Input.GetKeyDown("e"))
            {
                EndPushing();
            }

            // Animation
            if (pushLeft < 0)
            {
                animator.SetInteger("AnimationIndex", 9);
            }
            else if (pushLeft > 0)
            {
                animator.SetInteger("AnimationIndex", 10);
            }
        }
        else
        {
            float pushSpeed = currentPushBox.moveSpeed;
            float pushSpeedThisFrame = pushSpeed * Time.deltaTime;
            Vector3 pushDir = currentPushBox.GetPushingDirection(transform.position);

            if (Mathf.Abs(pushLeft) < pushSpeedThisFrame)
            {
                gameObject.transform.Translate(pushDir * pushLeft, Space.World);
                currentPushBox.transform.Translate(pushDir * pushLeft, Space.World);
                pushLeft = 0;
            }
            else
            {
                gameObject.transform.Translate(pushDir * pushSpeedThisFrame * Mathf.Sign(pushLeft), Space.World);
                currentPushBox.transform.Translate(pushDir * pushSpeedThisFrame * Mathf.Sign(pushLeft), Space.World);
                pushLeft -= pushSpeedThisFrame * Mathf.Sign(pushLeft);
            }
        }
    }

    /// <summary>
    /// Is called by the Unity Update() method. Extracted for better readability.
    /// </summary>
    private void UnityUpdate_State_StartPushing()
    {
        float moveSpeed = 7;
        float moveSpeedThisFrame = moveSpeed * Time.deltaTime;

        Vector3 targetPos = currentPushBox.transform.position - (currentPushBox.GetPushingDirection(transform.position) * currentPushBox.collider.bounds.size.x);
        Vector3 distVec = targetPos - transform.position;

        FallDown();

        // Just in case: get out of this mode if we fall down, etc
        if (distVec.magnitude > (currentPushBox.RealSize.x * 0.5f) + 3f || startPushingTime >= maxStartPushingTime)
        {
            EndPushing();
            return;
        }

        targetPos.y = transform.position.y;
        distVec.y = 0;

        /* We reached our goal */
        if (distVec.magnitude < moveSpeedThisFrame)
        {
            this.transform.position = targetPos;
            currentState = EntityState.pushing;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            this.transform.Translate(distVec.normalized * moveSpeedThisFrame, Space.World);
        }

        // ugly hack...
        transform.LookAt(new Vector3(currentPushBox.transform.position.x, transform.position.y, currentPushBox.transform.position.z));

        startPushingTime += Time.deltaTime;
    }

    /// <summary>
    /// Start pushing a box
    /// </summary>
    /// <param name="pb">the box</param>
    public void InitiatePushing(PushableBox pb)
    {
        if ((this.IsLocked() && currentState == EntityState.normal) || pb == null) // test for null: just to be sure, had some wierd bug...
        {
            return;
        }
        else
        {
            this.Lock();

            currentPushBox = pb;
            currentState = EntityState.startpushing;
            startPushingTime = 0;
            animator.SetInteger("AnimationIndex", 10);

            /* Save position of the box for possible revert later
             * (here as well, to prevent possible wierd bugs..) */
            currentPushBox.SavePosition();
        }
    }

    /// <summary>
    /// End pushing a box
    /// </summary>
    public void EndPushing()
    {
        currentPushBox = null;
        pushLeft = 0;
        currentState = EntityState.normal;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        animator.SetInteger("AnimationIndex", -1);
        this.Unlock();
    }

    /// <summary>
    /// End pushing a box for load game ,call this function only in GameDataRecover
    /// </summary>
    public void EndPushingForLoad()
    {
        currentPushBox = null;
        pushLeft = 0;
        currentState = EntityState.normal;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        this.Unlock();
    }

    #region Fall Down

    /// <summary>
    /// Defines how fast Igor falls when MANUALLY FALL is used
    /// </summary>
    public float fallSpeed = 8;

    /// <summary>
    /// Lets Igor fall down.
    /// </summary>
    /// <param name="speedFactor">speed factor - lets igor fall slower/faster</param>
    /// <remarks>Only needed if isKinematic = true!</remarks>
    public void FallDown(float speedFactor = 1f)
    {
        if (currentState != EntityState.pushing) // only in pushing, Igor is treated differently
        {
            // always fall down when the vertical speed is 0
            // this is important so that the character controller properly collides with the floor each frame
            if ((currentMovementDirection.y == 0f && !jumpWasCappedAtCeilingLastMove) || (currentState == EntityState.dead))
            {
                AddCurrentMovement(Vector3.down * fallSpeed * speedFactor);
            }
        }
        else /* when isKinematic we need the old falling down.. */
        {
            float fallDistance = fallSpeed * Time.deltaTime;
            Vector3 basevec = transform.position + (Vector3.up * 0.7f); // position as directly at the lower end, push it a bit up

            RaycastHit hit;
            float downDist;

            if (Physics.Raycast(basevec, Vector3.down, out hit))
            {
                downDist = hit.distance;
            }
            else
            {
                return;
            }

            if (downDist < fallDistance)
            {
                if (downDist > 0.2f) // don't allow falling through the ground
                {
                    transform.Translate(Vector3.down * 0.4f * downDist);
                }
            }
            else
            {
                transform.Translate(Vector3.down * fallDistance);
            }
        }
    }
    #endregion
    #endregion

    #region Mana

    /// <summary>
    /// Returns the current mana.
    /// </summary>
    /// <returns>the current mana.</returns>
    public float GetMana()
    {
        return mana;
    }

    /// <summary>
    /// manipulates Igor's mana
    /// </summary>
    /// <param name="amount">The amount to add or subtract from the mana</param>
    public void DoMana(float amount)
    {
        mana += amount;
        if (mana < 0f)
        {
            mana = 0f;
        }
        else if (mana > maxMana)
        {
            mana = maxMana;
        }
    }
    #endregion

    #region Jump
    /// <summary>
    /// force which is consumed by bust(jump)
    /// </summary>
    public float jumpForce = 5.0f;

    /// <summary>
    /// define the power which remains for the boost(jump)
    /// </summary>
    private float jumpCurrentForce = 0;

    /// <summary>
    /// max height which can igor reach
    /// </summary>
    public float jumpMaxHeight = 5.0f;

    /// <summary>
    /// The maximum position on the y axis during our current jump
    /// </summary>
    private float currentJumpMaxY;

    /// <summary>
    /// define how fast igor moves higher in the jump
    /// </summary>
    public float jumpSpeed = 10;

    /// <summary>
    /// time delay to the next jump
    /// </summary>
    public float jumpCooldown = 20;

    /// <summary>
    /// flag representing that igor is currently jumping
    /// </summary>
    private bool inJump = true;

    /// <summary>
    /// The height of our jump (y position in our world).
    /// </summary>
    private float jumpHeight = 0;

    /// <summary>
    /// set the regeneration of the jump force
    /// </summary>
    public float jumpForceRegeneration = 5f;

    /// <summary>
    /// Called by Unity Update() function. Sets the volume of the jetpack depending on jumping or not.
    /// </summary>
    private void UnityUpdate_JumpAudioVolume()
    {
        if (inJump)
        {
            if (!IsLocked())
            {
                if (Game.CutsceneAsInt != 0) //dont play fly animation when cutsce is on cause of igor cant fall in cutscenes
                {
                    animator.SetInteger("AnimationIndex", 5);
                }
            }
            audio.volume = 1;
        }
        else
        {
            audio.volume = 0.5f;
        }
    }

    /// <summary>
    /// Jumps if user presses the 'Jump' key.
    /// </summary>
    private void Jump()
    {
        // reset flag
        jumpWasCappedAtCeilingLastMove = false;

        jumpHeight = transform.position.y;

        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        ParticleSystem jetParticles = null;
        foreach (ParticleSystem particles in particleSystems)
        {
            if (particles.name.Contains("IgorJet"))
            {
                jetParticles = particles;
            }
        }

        // Have some kind of hysteresis when on ground: no rapid up and down
        if (Input.GetButton("Jump") && (jumpCurrentForce > 0.2f || !IsOnGround()) && jumpCurrentForce > 0)
        {
            float heightAboveGround = DistanceToGround();

            currentJumpMaxY = Mathf.Max(jumpHeight - heightAboveGround + jumpMaxHeight, currentJumpMaxY);

            if (heightAboveGround < jumpMaxHeight || jumpHeight < currentJumpMaxY)
            {
                AddCurrentMovement(Vector3.up * jumpSpeed);
            }
            else
            {
                jumpWasCappedAtCeilingLastMove = true;
            }

            jumpCurrentForce -= Time.deltaTime;
            if (jumpCurrentForce < 0)
            {
                jumpCurrentForce = 0;
            }

            if (jetParticles)
            {
                jetParticles.emissionRate = 250;
                jetParticles.startSpeed = 60;
            }
        }
        else
        {
            if (jetParticles)
            {
                jetParticles.emissionRate = 50;
                jetParticles.startSpeed = 10;
            }

            currentJumpMaxY = jumpHeight;
        }

        if (IsOnGround())
        {
            inJump = false;
        }
        else
        {
            inJump = true;

            // regenerate only if we didn't hit 0, so we cant fly indefinitely
            if (jumpCurrentForce > 0)
            {
                jumpCurrentForce += Time.deltaTime * jumpForceRegeneration / 10.0f;
            }

            if (jumpCurrentForce > jumpForce)
            {
                jumpCurrentForce = jumpForce;
            }

            //play Animation
            if (inAction == 0 && !IsLocked())
            {
                if (transform.position.y > jumpHeight)
                {
                    animator.SetInteger("AnimationIndex", 4);
                }
                else if (transform.position.y < jumpHeight)
                {
                    animator.SetInteger("AnimationIndex", 6);
                }
                else
                {
                    animator.SetInteger("AnimationIndex", 5);
                }
            }
        }
    }

    /// <summary>
    /// Regenerate jump force
    /// </summary>
    private void RegenerateJumpForce()
    {
        if (!inJump)
        {
            jumpCurrentForce += Time.deltaTime * jumpForceRegeneration;
            if (jumpCurrentForce > jumpForce)
            {
                jumpCurrentForce = jumpForce;
            }
        }
    }

    #endregion

    #region Animations

    /// <summary>
    /// reference to the animator
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Start animation with specific name.
    /// </summary>
    /// <param name="animation">the name of the animation.</param>
    public override void StartAnimation(string animation)
    {
        switch (animation)
        {
            case "Fist":
            case "Electro Fist":
                animator.SetInteger("AnimationIndex", 7);
                break;

            case "Laser":
            case "Prism Laser":
                animator.SetInteger("AnimationIndex", 8);
                break;

            case "Microwave":
                animator.SetInteger("AnimationIndex", 20);
                break;

            case "Headbang":
                animator.SetInteger("AnimationIndex", 22);
                break;

            case "Lightning":
                animator.SetInteger("AnimationIndex", 11);
                break;

            case "Hacking":
                animator.SetInteger("AnimationIndex", 25);
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
            case "Electro Fist":
                return 0.8f;

            case "Laser":
            case "Prism Laser":
                return 1.2f;

            case "Microwave":
                return 3.5f;

            case "Headbang":
                return 0.7f;

            case "Lightning":
                return 2.2f;

            case "Hacking":
                return 3f;
        }
        return 0f;
    }
    #endregion
}
