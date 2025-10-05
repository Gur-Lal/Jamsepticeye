using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
public class StartMusicCollider : MonoBehaviour
{
    [SerializeField] public MusicPlayer musicPlayerToStart;
    bool Lock = false;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && (!Lock))
        {
            Lock = true;
            if (musicPlayerToStart != null) musicPlayerToStart.PlayMusic();
        }
    }
}