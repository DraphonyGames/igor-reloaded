using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Background sound manager.
/// </summary>
public class BackgroundSound : MonoBehaviour
{
    /// <summary>
    /// checking the instance
    /// </summary>
    private static BackgroundSound _instance = null;

    /// <summary>
    /// The list of all background songs.
    /// </summary>
    public List<AudioClip> backgroundSongs;

    /// <summary>
    /// current track
    /// </summary>
    private int currentTrack = 0;

    /// <summary>
    /// the current audio clip object
    /// </summary>
    private AudioClip currentAudioClip;

    /// <summary>
    /// indicates whether a song is fading in
    /// </summary>
    private bool fadeIn = false;

    /// <summary>
    /// indicates whether a song is fading out
    /// </summary>
    private bool fadeOut = false;

    /// <summary>
    /// max volume to fade in
    /// </summary>
    public float maxVolume = 0.2f;

    /// <summary>
    /// min volume to fade out
    /// </summary>
    public float minVolume = 0.01f;

    /// <summary>
    /// step size for volume fading
    /// </summary>
    public float volumeFadingStep = 0.002f;

    /// <summary>
    /// Return the Instance of our class.
    /// </summary>
    public static BackgroundSound Instance
    {
        get { return _instance; }
    }

    /// <summary>
    /// Called by Unity when constructing the game object.
    /// Prevent Game object from being destroyed when loading something new.
    /// </summary>
    private void Awake()
    {
        // default music loading
        if (backgroundSongs.Count <= 0)
        {
            UnityEngine.Object[] resources = Resources.LoadAll("BackgroundMusic", typeof(AudioClip));
            backgroundSongs = new List<AudioClip>(resources.Length);
            int i = 0;
            foreach (UnityEngine.Object resource in resources)
            {
                backgroundSongs.Add((AudioClip)resource);
                i++;
            }
        }

        // set new song list before destruction
        Game.BackgroundSongs = backgroundSongs;

        if (audio == null)
        {
            gameObject.AddComponent<AudioSource>();
        }

        if (_instance != null && _instance != this)
        {
            Destroy(transform.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Start()
    {

        // if there is more then one background music object, there is already one... so destruct yourself
        GameObject[] backgroundMusicObjects = GameObject.FindGameObjectsWithTag("BackgroundMusic");
        if (backgroundMusicObjects.Length > 1)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (Game.GetIgor())
        {
            transform.position = Game.GetIgor().transform.position;
        }

        // smooth music fading
        if (!fadeIn && !fadeOut)
        {
            if (!audio.isPlaying)
            {
                FadeIn();
            }
        }
        else if (fadeOut)
        {
            if (audio.volume > minVolume)
            {
                audio.volume -= volumeFadingStep;
            }
            else
            {
                fadeOut = false;
                FadeIn();
            }
        }
        else if (fadeIn)
        {
            if (audio.volume < maxVolume)
            {
                audio.volume += volumeFadingStep;
            }
            else
            {
                fadeIn = false;
                audio.volume = maxVolume;
            }
        }
    }

    /// <summary>
    /// start game background sound.
    /// </summary>
    private void FadeIn()
    {
        fadeIn = true;
        audio.Stop();
        currentAudioClip = backgroundSongs[(currentTrack++) % backgroundSongs.Count];
        audio.clip = currentAudioClip;
        audio.volume = minVolume;
        audio.Play();
    }

    /// <summary>
    /// loads new music list when new level is loaded
    /// </summary>
    /// <param name="level">index of the new level</param>
    public void OnLevelWasLoaded(int level)
    {
        backgroundSongs = Game.BackgroundSongs;
        if (!backgroundSongs.Contains(currentAudioClip))
        {
            fadeIn = false;
            fadeOut = true;
        }
    }
}
