﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class that holds information about a button concisely, for use in Ispector and the button array
[System.Serializable]
public class ButtonClass
{
    public GameObject btn;
    public KeyCode key;
    public Button btnClass;

    public ButtonClass(GameObject button, KeyCode code, Button buttonClass)
    {
        // Button gameobject reference
        this.btn = button;
        // Keycode to hit button according to the input array
        this.key = code;
        this.btnClass = buttonClass;
    }
}

public class Button : MonoBehaviour
{
    // Which button it is
    public int type;
    // The beat that this button resides on
    public float beat;
    // Which button in the list is this?
    public int index;

    public bool upNext = false;

    // ButtonClass reference, shouldn't be used, but just in case
    public ButtonClass btn;
    // Reference to the rank text prefab
    public GameObject rankPrefab;
    //Reference the game handlers
    private GameHandler gameHandler;
    private MusicHandler musicHandler;
    // Need reference to sprite renderer to change the type
    private new SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Hits the note in question based on the rate parameter
    /// </summary>
    /// <param name="songPosInBeats"></param>
    /// <param name="bpm"></param>
    /// <param name="rate"></param>
    public void Hit(float songPosInBeats, float bpm, int rate)
    {
        GameObject rankText = Instantiate(rankPrefab, this.gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        rankText.GetComponent<RankText>().Init(rate);
        Destroy(this.gameObject);
        gameHandler.hits[rate]++;
    }

    /// <summary>
    /// Kills the button with a missed ranking
    /// </summary>
    public void Missed()
    {
        GameObject rankText = Instantiate(rankPrefab, this.gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        rankText.GetComponent<RankText>().Init(4);
        Destroy(this.gameObject);
        gameHandler.hits[4]++;
    }

    /// <summary>
    /// Initiates the buttons defaults and sets any needed information
    /// </summary>
    /// <param name="typein">Type of Button as an integer, see GameHandler</param>
    /// <param name="curbeat">Current beat of the song</param>
    /// <param name="indexin"></param>
    public void Init(int typein, float curbeat, int indexin, Button buttonClass)
    {
        // Create button
        beat = curbeat;
        GameHandler[] objects = FindObjectsOfType<GameHandler>();
        gameHandler = objects[0];
        MusicHandler[] objects2 = FindObjectsOfType<MusicHandler>();
        musicHandler = objects2[0];
        Sprite[] types = objects[0].types;
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = types[typein];
        type = typein;
        index = indexin;
        btn = new ButtonClass(this.gameObject, gameHandler.inputs[type], buttonClass);
    }

    /// <summary>
    /// Determines what the current accuracy rating should be depending on the current state of the game.
    /// </summary>
    /// <param name="pos">Current song position in beats</param>
    /// <param name="bpm">Song Beats per Minute</param>
    /// <param name="beat">Beat of the note in question</param>
    /// <returns></returns>
    public int GetRank(float pos, float bpm, float beat)
    {
        // Cool Rank
        if ((pos <= beat && pos > beat - ((bpm / 60) / 5)) || (pos >= beat && pos < beat + ((bpm / 60) / 5)))
        {
            return 0;
        }
        // Fine rank
        else if ((pos <= beat && pos > beat - ((bpm / 60) / 3)) || (pos >= beat && pos < beat + ((bpm / 60) / 3)))
        {
            return 1;
        }
        // Safe rank
        else if ((pos <= beat && pos > beat - ((bpm / 60) / 2)) || (pos >= beat && pos < beat + ((bpm / 60) / 2)))
        {
            return 2;
        }
        // Sad Rank
        else if ((pos <= beat && pos > beat - ((bpm / 60))) || (pos >= beat && pos < beat + ((bpm / 60))))
        {
            return 3;
        }
        // Missed
        else
        {
            return 4;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the button reaches the "fadeEnd" part of the path, kill it with a missed ranking
        if ((beat / musicHandler.lengthInBeats) <= musicHandler.fadeEnd)
        {
            Missed();
        }
        // If the designated key is hit
        if (Input.GetKeyDown(btn.key))
        {
            // If this button is the next button that should be hit, get the rank and hit it
            float pos = musicHandler.songPosInBeats;
            float bpm = musicHandler.bpm;
            // If you would have missed the previous note anyway, hit this one
            if (upNext == false)
            {
                return;
            }
            Hit(pos, bpm, GetRank(pos, bpm, beat));
        }
    }
}
