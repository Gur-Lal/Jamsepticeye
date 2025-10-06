using UnityEngine;

public class FanDeath : MonoBehaviour
{
    [SerializeField] GameObject prompt;
    [SerializeField] ParticleSystem bloodEffect;   //effect prefab
    [SerializeField] Transform effectSpawnPoint;   //point to spawn the effect

    private bool playerIsNearby = false;
    private Collider2D playerCol;

    void Start()
    {
        if (prompt != null) prompt.SetActive(false);

        AudioSource src = GetComponent<AudioSource>();
        if (src != null) if(AudioManager.Instance != null)  src.volume *= AudioManager.Instance.sfxVolume;
    }

    void Update()
    {
        if (playerIsNearby)
        {
            if (prompt != null) prompt.SetActive(true);

            if (playerCol != null && PlayerController.input.Player.Interact.triggered)
            {
                playerCol.GetComponent<PlayerController>().Die();

                // Play blood effect
                PlayBloodEffect();
            }
        }
        else
        {
            if (prompt != null) prompt.SetActive(false);
        }
    }

    void PlayBloodEffect()
    {
        if (bloodEffect != null)
        {
            
            Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
            Instantiate(bloodEffect, spawnPos, Quaternion.identity);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCol = other;
            playerIsNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCol = other;
            playerIsNearby = false;            
        }
    }
}
