using UnityEngine;
using System.Collections.Generic;

public class SpriteLoopScript : MonoBehaviour
{
    [SerializeField] int FPS = 12;
    [SerializeField] List<Sprite> SpriteList;
    int sprIndex = 0;
    int sprMaxIndex = 0;
    SpriteRenderer spr;
    float lastSpriteChangeTime = 0;
    float secondsPerFrame;//derived
    void Start()
    {
        if (SpriteList == null || SpriteList.Count == 0) { this.enabled = false; return; }
        sprMaxIndex = SpriteList.Count - 1;
        spr = GetComponent<SpriteRenderer>();
        secondsPerFrame = 1f/FPS;
    }

    void Update()
    {
        if (Time.time - lastSpriteChangeTime > secondsPerFrame)
        {
            sprIndex++;
            if (sprIndex > sprMaxIndex) sprIndex = 0;
            spr.sprite = SpriteList[sprIndex];
            lastSpriteChangeTime = Time.time;
        }
    }
}
