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

    public void DeleteOldestCorpseIfNeeded()
    {
        if (enableLimit && corpses.Count > maxCorpses)
        {
            if (corpses.Count == 0) return;
            GameObject oldest = corpses[0];
            corpses.RemoveAt(0);
            if (oldest != null)
            {
                Destroy(oldest);
            }
        }
    }

    public void RegisterCorpse(GameObject corpse)
    {
        if (corpse == null)
        {
            return;
        }

        corpses.Add(corpse);

        DeleteOldestCorpseIfNeeded();
        
        for (int i = 0; i < corpses.Count; i++) { CorpseDecay cd = corpses[i].GetComponent<CorpseDecay>(); cd.Prep(); cd.Up(i, corpses.Count); }
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
}