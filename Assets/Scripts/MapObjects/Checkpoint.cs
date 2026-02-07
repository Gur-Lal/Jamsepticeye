using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    static Checkpoint activeCheckpoint;
    bool amActiveCheckpoint = false;
    [SerializeField] float cooldownSeconds = 3f;
    Collider2D col;
    SpriteRenderer spr;
    bool onCooldown = false;
    void Start()
    {
        col = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (amActiveCheckpoint && this != activeCheckpoint) { amActiveCheckpoint = false; spr.color = Color.white; }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (onCooldown) return;

        if (other.CompareTag("Player")) Activate(other);
    }

    void Activate(Collider2D playerCol)
    {
        PlayerController pcontr = playerCol.GetComponent<PlayerController>();
        pcontr.SetSpawnPoint(transform.position);

        StartCoroutine(Warmup());

        onCooldown = true;
        activeCheckpoint = this;
        amActiveCheckpoint = true;
        StartCoroutine(EndCooldown());
    }

    IEnumerator Warmup()
    {
        float targetRed = 0.6f;

        spr.color = Color.black;

        while (spr.color.r < targetRed)
        {
            float step = Time.deltaTime / 5f;
            spr.color = new Color(spr.color.r + step, spr.color.g, spr.color.b);
            yield return null;
        }

    }

    IEnumerator EndCooldown()
    {
        yield return new WaitForSeconds(cooldownSeconds);

        onCooldown = false;
        if (activeCheckpoint == this)
        {
            spr.color = Color.softRed; //'active' checkpoint
        }
        else
        {
            spr.color = Color.white; //'empty' checkpoint
        }

    }
}
