﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This manages the rank text when you hit a note
/// </summary>
public class RankText : MonoBehaviour
{
    // Reference the sprite itself and other vars
    public Sprite[] rankSprites;
    public GameObject spriteObj;
    public SpriteRenderer spriteRend;
    public Animator spriteAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Change sprite to match rank and start animation
    /// </summary>
    /// <param name="rank">Accuracy of the hit</param>
    public void Init(int rank)
    {
        spriteRend.sprite = rankSprites[rank];
    }

    // Update is called once per frame
    void Update()
    {
        // If the animation finishes, kill the object to save memory
        if (!spriteAnim.GetCurrentAnimatorStateInfo(0).IsName("RankFloat"))
        {
            Destroy(this.gameObject);
        }
    }
}
