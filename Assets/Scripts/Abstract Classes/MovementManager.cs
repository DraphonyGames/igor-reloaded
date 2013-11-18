using UnityEngine;
using System.Collections;

/// <summary>
/// Manages automatic movement
/// </summary>
public abstract class MovementManager : Triggerable
{
    /// <summary>
    /// Actions to perform
    /// </summary>
    private Queue actions;

    /// <summary>
    /// Currently performed actions
    /// </summary>
    public ArrayList running;

    /// <summary>
    /// Variable indicating if the manager is paused
    /// </summary>
    private bool pause;

    /// <summary>
    /// True if the movement manager starts when triggered
    /// </summary>
    public bool enable;

    /// <summary>
    /// Variable indicating if the cutscene is run
    /// </summary>
    public bool isRun = false;

    /// <summary>
    /// Variable indicating if the cutscene is loop
    /// </summary>
    public bool isLoop = false;

    /// <summary>
    /// Texture of the black bars to display
    /// </summary>
    private Texture2D bars;

    /// <summary>
    /// Saves the positions of the black bars
    /// </summary>
    private int positions;

    /// <summary>
    /// Saves the maximum position
    /// </summary>
    private int maxPosition;

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    public Vector3 initialPosition;

    /// <summary>
    /// Enables scripts to disable the black bars
    /// </summary>
    public bool ignoreCutscene;

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void Awake()
    {
        initialPosition = transform.position;
    }

    /// <summary>
    /// Called when the Manager is created
    /// </summary>
    private void Start()
    {
        // Generate the black texture for the bars
        bars = new Texture2D(1, 1);
        bars.SetPixel(0, 0, Color.black);
        bars.Apply();

        positions = 0;

        if (!isRun)
        {
            StartManager();
            enable = true;
        }

        // Calculate the bar size
        maxPosition = Screen.height / 6;
        if (maxPosition < 100)
        {
            maxPosition = 100;
        }
    }


    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    public void StartManager()
    {
        actions = new Queue();
        running = new ArrayList();
        pause = true;
        
        Script();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        if (Game.IsCutscene && !Game.ignoreBars && positions < maxPosition)
        {
            int delta = (int)(Time.deltaTime * maxPosition);

            if (positions + delta > maxPosition)
            {
                delta = maxPosition - positions;
            }

            positions += delta;
        }
        else if ((!Game.IsCutscene || Game.ignoreBars) && positions > 0)
        {
            int delta = (int)(Time.deltaTime * maxPosition);

            if (positions - delta < 0)
            {
                delta = positions;
            }

            positions -= delta;
        }

        // Reset speed
        if (running != null && running.Count == 0 && actions.Count == 0)
        {
            Time.timeScale = 1f;
            return;
        }
        
        if (!IsRunning || isRun)
        {
            return;
        }

        // Make cut scenes "skipable"
        if (Input.GetKey(KeyCode.F12) && Game.IsCutscene)
        {
            Time.timeScale = 10f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        Next();
    }

    /// <summary>
    /// Runs the actions in the running list and copies new actions to it
    /// </summary>
    private void Next()
    {
        RunCurrent();
        ReadNext();
    }

    /// <summary>
    /// Run the Actions in the running list
    /// </summary>
    private void RunCurrent()
    {
        for (int i = running.Count - 1; i >= 0; i--)
        {
            IAction action = (IAction)running[i];

            action.OnUpdate();
            if (action.Finished())
            {
                action.OnFinish();
                running.Remove(action);
            }
        }
    }

    /// <summary>
    /// Dequeue the next actions
    /// </summary>
    private void ReadNext()
    {
        while (actions.Count > 0)
        {
            IAction action = (IAction)actions.Peek();
            if (action is ActionSleep)
            {
                action.OnUpdate();
                if (action.Finished())
                {
                    actions.Dequeue();
                    return;
                }
                else
                {
                    return;
                }
            }
            else
            {
                action.OnStart();
                running.Add(action);
                try
                {
                    actions.Dequeue();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
        }

        isRun = true;
        IsRunning = false;
    }

    /// <summary>
    /// Render the GUI actions
    /// </summary>
    private void RunCurrentGUI()
    {
        foreach (IAction action in running)
        {
            action.GUI();
        }
    }

    /// <summary>
    /// Called for the GUI
    /// </summary>
    private void OnGUI()
    {
        // draw pretty close to the camera, only the message boxes should be closer
        int depthBackup = GUI.depth;
        GUI.depth = -2;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, positions), bars);
        GUI.DrawTexture(new Rect(0, Screen.height - positions, Screen.width, positions), bars);
        if (!isRun && IsRunning)
        {
            RunCurrentGUI();
        }

        // Reset depth to initial value (just to be sure)
        GUI.depth = depthBackup;
    }

    /// <summary>
    /// Appends a new IAction to the action queue
    /// </summary>
    /// <param name="action">The IAction to attach</param>
    public void Append(IAction action)
    {
        actions.Enqueue(action);
    }

    /// <summary>
    /// Returns if the Manager is running
    /// </summary>
    public bool IsRunning
    {
        get { return !pause || (actions.Count != 0 && running.Count != 0); }
        set { pause = !value; }
    }

    /// <summary>
    /// Implementation of Triggerable
    /// </summary>
    /// <param name="by">Calling object (ignored)</param>
    /// <param name="isTriggered">The trigger is on</param>
    public override void OnTrigger(GameObject by, bool isTriggered)
    {
        if (isTriggered)
        {
            if (enable)
            {
                StartManager();
                IsRunning = true;
            }
        }
    }

    /// <summary>
    /// Convenience function for sleep
    /// </summary>
    /// <param name="seconds">Seconds to sleep</param>
    public void Sleep(float seconds)
    {
        Append(new ActionSleep(seconds));
    }

    /// <summary>
    /// adds a reset-action
    /// </summary>
    public void Reset()
    {
        Append(new ActionReset(this));
        isLoop = true;
    }

    /// <summary>
    /// Script of the Cutscene
    /// </summary>
    public abstract void Script();
}
