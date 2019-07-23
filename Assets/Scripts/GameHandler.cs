﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game state, does not handle the music and note spawning portion
/// </summary>
public class GameHandler : MonoBehaviour
{
    // Mostly just stores objects right now.
    public bool debugMode = false;
    public Sprite[] types;
    public List<ButtonClass> buttons;
    public KeyCode[] inputs;
    public static float frameTime { get; private set; } = 1f / 60f;

    #region FPS
    // For fps calculation.
    private int frameCount;
    private float elapsedTime;
    [HideInInspector]
    public double frameRate { get; private set; }
    #endregion

    public int[] hits = new int[5];

    private MusicHandler musicHandler;

    private void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get reference to music handler
        if (FindObjectsOfType<MusicHandler>()[0] != null)
            musicHandler = FindObjectsOfType<MusicHandler>()[0];
        // Make sure this object doesnt unload, for the results screen
        transform.SetParent(null);
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Only run this code in-game
        if(musicHandler != null)
        {
            if(musicHandler.finished == true)
            {
                LoadScene(2);
            }
        }

        // FPS calculation
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;
        if (elapsedTime > 0.5f)
        {
            frameRate = System.Math.Round(frameCount / elapsedTime, 1, System.MidpointRounding.AwayFromZero);
            frameCount = 0;
            elapsedTime = 0;
        }
    }
}
