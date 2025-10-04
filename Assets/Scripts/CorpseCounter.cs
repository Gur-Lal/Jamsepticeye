using UnityEngine;
using System.Collections.Generic;

public class CorpseCounter : MonoBehaviour
{
    public static CorpseCounter Instance { get; private set; }

    [Header("Corpse Limit")]
    [SerializeField] private bool enableLimit = true;
    [SerializeField, Range(1, 100)] private int maxCorpses = 10;

    private List<GameObject> corpses = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool CanSpawnCorpse()
    {
        if (!enableLimit) return true;
        return corpses.Count < maxCorpses;
    }

    public void RegisterCorpse(GameObject corpse)
    {
        if (corpse == null)
        {
            return;
        }

        corpses.Add(corpse);

        // If limit exceeded, destroy oldest
        if (enableLimit && corpses.Count > maxCorpses)
        {
            GameObject oldest = corpses[0];
            corpses.RemoveAt(0);
            if (oldest != null)
            {
                Destroy(oldest);
            }
        }
    }

    public void UnregisterCorpse(GameObject corpse)
    {
        corpses.Remove(corpse);
    }

    public int GetCorpseCount()
    {
        // Clean up any destroyed corpses
        corpses.RemoveAll(c => c == null);
        return corpses.Count;
    }

    // Display info on screen (optional - for testing)
    private void OnGUI()
    {
        if (!Application.isPlaying) return;
        GUI.Label(new Rect(10, 10, 250, 30), $"Corpses: {GetCorpseCount()}/{maxCorpses}");
    }
}