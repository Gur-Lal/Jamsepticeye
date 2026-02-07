using UnityEngine;

public class HarmObject : MonoBehaviour
{
    Collider2D trigCol;

    [Header("Effect")]
    [SerializeField] private ParticleSystem bloodEffect;   //effect prefab
    [SerializeField] private Transform effectSpawnPoint;   //point to spawn the effect

    void Start()
    {
        trigCol = GetComponent<Collider2D>();
        trigCol.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerController>().Die();

        //play blood effect
        PlayBloodEffect();
    }

    void PlayBloodEffect()
    {
        if (bloodEffect != null)
        {
            //instantiate the effect
            Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
            Instantiate(bloodEffect, spawnPos, Quaternion.identity);
        }
    }
}
