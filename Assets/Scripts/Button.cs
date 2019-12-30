﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class that holds information about a button concisely, for use in Ispector and the button array
[System.Serializable]
public class ButtonClass
{
    public GameObject btn;
    public KeyCode key;
    public KeyCode keyAlt;
    public Button btnClass;

    public ButtonClass(GameObject button, KeyCode code, KeyCode codeAlt, Button buttonClass)
    {
        // Button gameobject reference
        this.btn = button;
        // Keycode to hit button according to the input array
        this.key = code;
        this.keyAlt = codeAlt;
        this.btnClass = buttonClass;
    }
}

public class Button : MonoBehaviour
{
    /// <summary>
    /// What kind of button is this?
    /// </summary>
    public int type;

    /// <summary>
    /// Which of the two tracks this button is on; 0 = first, 1 = second
    /// </summary>
    public int track = 0;

    /// <summary>
    /// Which beat this button plays on
    /// </summary>
    public float beat;

    /// <summary>
    /// Where in the button list is this
    /// </summary>
    public int index;

    // Range which are sustain notes
    public static int[] susRange = { 8, 12 };

    /// <summary>
    /// Is this note the next note to play?
    /// </summary>
    public bool upNext = false;
    // Sustain note
    public bool sus = false;
    public bool holdingSus = false;

    // ButtonClass reference, shouldn't be used, but just in case
    public ButtonClass btn;

    public Button pair = null;
    // Reference to the rank text prefab
    public GameObject rankPrefab;
    //Reference the game handlers
    private GameHandler gameHandler;
    private MusicHandler musicHandler;
    // Need reference to sprite renderer to change the type
    private new SpriteRenderer renderer;

    /// <summary>
    /// Checks if an integer is between a given range
    /// </summary>
    /// <param name="numberToCheck">The integer you are checking</param>
    /// <param name="bottom">The first number in the range</param>
    /// <param name="top">The top number in the range</param>
    /// <returns></returns>
    public static bool IsSustain(float numberToCheck)
    {
        numberToCheck = Mathf.RoundToInt(numberToCheck);
        return (numberToCheck >= susRange[0] && numberToCheck <= susRange[1]);
    }

    /// <summary>
    /// Hits the note in question based on the rate parameter
    /// </summary>
    /// <param name="songPosInBeats">Position in the song in beats</param>
    /// <param name="bpm">Beats per Minute of the song</param>
    /// <param name="rate">Accuracy rating of the hit</param>
    public void Hit(float songPosInBeats, float bpm, GameHandler.Rank rate)
    {
        GameObject rankText = Instantiate(rankPrefab, this.gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        rankText.GetComponent<RankText>().Init(rate);
        Destroy(this.gameObject);
        gameHandler.hits[(int)rate]++;
    }

    /// <summary>
    /// Kills the button with a missed ranking
    /// </summary>
    public void Missed()
    {
        GameObject rankText = Instantiate(rankPrefab, this.gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        rankText.GetComponent<RankText>().Init(GameHandler.Rank.Missed);
        Destroy(this.gameObject);
        gameHandler.hits[4]++;
    }

    /// <summary>
    /// Initiates the buttons defaults and sets any needed information
    /// </summary>
    /// <param name="typein">Type of Button as an integer, see GameHandler</param>
    /// <param name="curbeat">Current beat of the song</param>
    /// <param name="indexin">The index of the button array that this button belongs to</param>
    public void Init(int typein, float curbeat, int indexin, int trackin, Button buttonClass)
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
        track = trackin;
        // For future me, this range is the "Hold" sprites in the GameHandler's type array.
        sus = IsSustain(type);
        btn = new ButtonClass(this.gameObject, gameHandler.inputs[type], gameHandler.inputsAlt[type], buttonClass);
    }

    /// <summary>
    /// Determines what the current accuracy rating should be depending on the current state of the game.
    /// </summary>
    /// <param name="pos">Current song position in beats</param>
    /// <param name="bpm">Song Beats per Minute</param>
    /// <param name="beat">Beat of the note in question</param>
    /// <returns>The rank index</returns>
    public GameHandler.Rank GetRank(float pos, float bpm, float beat)
    {
        if ((pos <= beat && pos > beat - ((bpm / 60) / 5)) || (pos >= beat && pos < beat + ((bpm / 60) / 5)))
        {
            return GameHandler.Rank.Cool;
        }
        else if ((pos <= beat && pos > beat - ((bpm / 60) / 4)) || (pos >= beat && pos < beat + ((bpm / 60) / 4)))
        {
            return GameHandler.Rank.Fine;
        }
        else if ((pos <= beat && pos > beat - ((bpm / 60) / 3)) || (pos >= beat && pos < beat + ((bpm / 60) / 3)))
        {
            return GameHandler.Rank.Safe;
        }
        else if ((pos <= beat && pos > beat - ((bpm / 60) / 2)) || (pos >= beat && pos < beat + ((bpm / 60) / 2)))
        {
            return GameHandler.Rank.Sad;
        }
        else
        {
            return GameHandler.Rank.Missed;
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

        // Prototype code; FUTURE ME, PLEASE OPTIMIZE
        foreach (ButtonClass altBtn in gameHandler.buttons2)
        {
            if (altBtn.btnClass.upNext == true && GetRank(altBtn.btnClass.beat, musicHandler.bpm, beat) != GameHandler.Rank.Missed)
            {
                if (track == 0)
                {
                    pair = altBtn.btnClass;
                }
                break;
            }
        }
;
        // Prototype code; FUTURE ME, PLEASE OPTIMIZE
        foreach (ButtonClass btn in gameHandler.buttons)
        {
            if (btn.btnClass.upNext == true && GetRank(btn.btnClass.beat, musicHandler.bpm, beat) != GameHandler.Rank.Missed)
            {
                if (track == 1)
                {
                    pair = btn.btnClass;
                }
                break;
            }
        }
    }
}
