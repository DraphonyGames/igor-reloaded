using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Global status class for our game
/// </summary>
public static class Game
{
    #region PauseLogic

    /// <summary>
    /// Internal variable for the paused status
    /// </summary>
    private static bool paused = false;

    /// <summary>
    /// Paused as boolean.
    /// </summary>
    public static bool IsPaused
    {
        get
        {
            return paused;
        }
        set
        {
            if (paused != value)
            {
                TriggerPause();
                paused = value;
                Screen.showCursor = value;
            }
        }
    }

    /// <summary>
    /// Paused as float
    /// </summary>
    public static float PausedAsFloat
    {
        get { return paused ? 0f : 1f; }
        private set { }
    }

    /// <summary>
    /// Paused as integer.
    /// </summary>
    public static int PausedAsInt
    {
        get { return paused ? 0 : 1; }
        private set { }
    }

    /// <summary>
    /// Registered handlers
    /// </summary>
    private static ArrayList pauseHandlers = new ArrayList();

    /// <summary>
    /// Adds a new handler
    /// </summary>
    /// <param name="handler">The handler to register</param>
    public static void AddPauseHandler(IPauseHandler handler)
    {
        pauseHandlers.Add(handler);
    }

    /// <summary>
    /// Removes a handler
    /// </summary>
    /// <param name="handler">Handler to remove</param>
    public static void RemovePauseHandler(IPauseHandler handler)
    {
        pauseHandlers.Remove(handler);
    }

    /// <summary>
    /// Triggers the event
    /// </summary>
    private static void TriggerPause()
    {
        foreach (IPauseHandler handler in pauseHandlers)
        {
            handler.PauseStateChanged();
        }
    }

    #endregion

    #region Difficulty

    /// <summary>
    /// Difficulty modes of the game.
    /// </summary>
    public enum Difficulty
    {
        /// <summary>
        /// The normal difficulty.
        /// </summary>
        Medium,
        /// <summary>
        /// It's harder to play the game.
        /// </summary>
        Hard
    }

    /// <summary>
    /// Current difficulty mode.
    /// </summary>
    public static Difficulty currentDifficulty = Difficulty.Medium;

    /// <summary>
    /// Float we can multiply for difficulty things. Is 1 for lowest and higher for higher difficulties.
    /// </summary>
    public static float DifficultyMultiplier
    {
        get
        {
            switch (currentDifficulty)
            {
                case Difficulty.Hard:
                    return 1.3f;
            }
            return 1f;
        }
    }

    #endregion

    /// <summary>
    /// shows if an menu (inventory, skill gui, etc...) is open
    /// </summary>
    public static bool isMenuOpen = false;

    #region CutsceneLogic

    /// <summary>
    /// Internal variable for the cutscene status
    /// </summary>
    private static bool cutscene = false;

    /// <summary>
    /// Enables the bars to be hidden
    /// </summary>
    public static bool ignoreBars = false;

    /// <summary>
    /// True if inside cutscene
    /// </summary>
    public static bool IsCutscene
    {
        get
        {
            return cutscene;
        }
        set
        {
            if (cutscene != value)
            {
                TriggerCutscene();
                cutscene = value;
            }
        }
    }

    /// <summary>
    /// Returns whether we are allowed to move, etc, or not
    /// </summary>
    public static bool InGame
    {
        get
        {
            return !IsCutscene && !IsPaused;
        }
    }

    /// <summary>
    /// 1 if inside cutscene else 0
    /// </summary>
    public static float CutsceneAsFloat
    {
        get { return cutscene ? 0f : 1f; }
        private set { }
    }

    /// <summary>
    /// 1 if inside cutscene else 0
    /// </summary>
    public static int CutsceneAsInt
    {
        get { return cutscene ? 0 : 1; }
        private set { }
    }

    /// <summary>
    /// Registered handlers
    /// </summary>
    private static ArrayList cutsceneHandlers = new ArrayList();

    /// <summary>
    /// Adds a new handler
    /// </summary>
    /// <param name="handler">The handler to register</param>
    public static void AddCutsceneHandler(IPauseHandler handler)
    {
        cutsceneHandlers.Add(handler);
    }

    /// <summary>
    /// Removes a handler
    /// </summary>
    /// <param name="handler">Handler to remove</param>
    public static void RemoveCutsceneHandler(IPauseHandler handler)
    {
        pauseHandlers.Remove(handler);
    }

    /// <summary>
    /// Triggers the cutscene event
    /// </summary>
    private static void TriggerCutscene()
    {
        foreach (IPauseHandler handler in pauseHandlers)
        {
            handler.PauseStateChanged();
        }
    }

    #endregion

    #region GetIgor

    /// <summary>
    /// caches the igor game object to speed up successive calls
    /// </summary>
    private static GameObject igorGameObject;

    /// <summary>
    /// caches the igor game object to speed up successive calls
    /// </summary>
    /// <returns>IGOR WHITE SPACE</returns>
    public static GameObject GetIgor()
    {
        // note that this also works on a destroyed igor
        if (igorGameObject == null)
        {
            igorGameObject = GameObject.FindGameObjectWithTag("Unit Player");
        }
        return igorGameObject;
    }

    /// <summary>
    /// Spawns an Igor if there is no Igor
    /// </summary>
    public static void SpawnIgor()
    {
        igorGameObject = GameObject.FindGameObjectWithTag("Unit Player");
        if (igorGameObject == null)
        {
            GameObject spawn = GameObject.FindGameObjectWithTag("IgorSpawn");
            if (spawn != null)
            {
                SpawnPoint spawnPoint = spawn.GetComponent<SpawnPoint>();
                spawnPoint.SpawnIgor();
            }
        }
    }

    /// <summary>
    /// caches the igor game object to speed up successive calls
    /// </summary>
    private static Igor igorComponent;

    /// <summary>
    /// Gets Igor, but as Igor class
    /// </summary>
    /// <returns>Igor, but as Igor object</returns>
    public static Igor GetIgorComponent()
    {
        if (igorComponent == null)
        {
            igorComponent = GetIgor().GetComponent<Igor>();
        }
        return igorComponent;
    }

    #endregion

    #region Audio

    /// <summary>
    /// The current audio volume.
    /// </summary>
    private static float currentVolume = 1f;
    /// <summary>
    /// flag for whether the current volume has been at least queried once and those been loaded from the preferences
    /// </summary>
    private static bool currentVolumeInitialized = false;
    /// <summary>
    /// The current audio volume.
    /// </summary>
    public static float CurrentVolume
    {
        get
        {
            if (!currentVolumeInitialized)
            {
                currentVolumeInitialized = true;
                currentVolume = PlayerPrefs.GetFloat("AudioVolume", 1f);
            }
            return currentVolume;
        }
        set
        {
            currentVolume = value;
            PlayerPrefs.SetFloat("AudioVolume", currentVolume);
            AudioListener.volume = currentVolume;
        }
    }

    /// <summary>
    /// The list of all background songs.
    /// </summary>
    public static List<AudioClip> BackgroundSongs
    {
        get
        {
            return _backgroundSongs;
        }
        set
        {
            if (value.Count > 0)
            {
                _backgroundSongs = value;
            }
        }
    }

    /// <summary>
    /// internal background sound song list
    /// </summary>
    private static List<AudioClip> _backgroundSongs;

    /// <summary>
    /// Play an audio clip at a specific static position.
    /// </summary>
    /// <param name="clip">The clip to play.</param>
    /// <param name="position">The position at which it should be played.</param>
    /// <param name="volumeScale">The volume scale.</param>
    public static void PlayAudioAtStaticPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        GameObject go = new GameObject();
        AudioSource audio = go.AddComponent<AudioSource>();
        go.transform.position = position;
        audio.PlayOneShot(clip, volumeScale);
        GameObject.Destroy(go, clip.length);
    }
    /// <summary>
    /// Play an audio clip attached to a transform.
    /// </summary>
    /// <param name="clip">The clip to play.</param>
    /// <param name="parent">The transform to which it should be attached.</param>
    /// <param name="volumeScale">The volume scale.</param>
    public static void PlayAudioAtParent(AudioClip clip, Transform parent, float volumeScale = 1f)
    {
        GameObject go = new GameObject();
        go.AddComponent<AudioSource>();
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.audio.PlayOneShot(clip, volumeScale);
        GameObject.Destroy(go, clip.length);
    }

    #endregion
}
