﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actually spawns the buttons when the Music Handler demands it
/// </summary>
public class SpawnButtons : MonoBehaviour
{
    // Needs a reference to some game state vars
    public MotionPath path;
    public GameObject buttonPrefab;
    private GameHandler gameHandler;
    // Prototype var, not needed?
    //public float UV = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference to the Game Handler
        GameHandler[] objects = FindObjectsOfType<GameHandler>();
        gameHandler = objects[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Spawns the button using a prefab and initiates it with the variables you give it. Also returns the button as a gameObject
    /// </summary>
    /// <param name="uv">Percentage of the path</param>
    /// <param name="type">Type of button</param>
    /// <param name="beat">Beat of button</param>
    /// <param name="indexin">Index of the button</param>
    /// <returns></returns>
    public GameObject spawn(float uv, float type, float beat, int indexin)
    {
        GameObject button = Instantiate(buttonPrefab, new Vector3(path.PointOnNormalizedPath(uv).x, path.PointOnNormalizedPath(uv).y, buttonPrefab.transform.position.z), new Quaternion(0, 0, 0, 0));
        // If type given does not exist, switch to Star(0)
        if (Mathf.RoundToInt(type) >= gameHandler.types.Length)
        {
            type = 0;
        }
        Button btnClass = button.GetComponent<Button>();
        btnClass.Init(Mathf.RoundToInt(type), beat, indexin, btnClass);
        return button;
    }
}
