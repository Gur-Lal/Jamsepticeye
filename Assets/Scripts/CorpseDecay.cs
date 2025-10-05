using UnityEngine;

public class CorpseDecay : MonoBehaviour
{
    [SerializeField] Sprite state0;
    [SerializeField] Sprite state1;
    [SerializeField] Sprite state2;

    SpriteRenderer spr;
    bool ready = false;

    public void Prep()
    {
        if (ready) return;
        spr = GetComponent<SpriteRenderer>();
        ready = true;
    }

    public void Up(int pos, int max)
    {
        if (max == 1)
        {
            spr.sprite = state0;
        }
        else if (max == 2)
        {
            if (pos == 0) spr.sprite = state1;
            else if (pos == 1) spr.sprite = state0;
        }
        else if (max == 3)
        {
            if (pos == 0) spr.sprite = state2;
            else if (pos == 1) spr.sprite = state1;
            else if (pos == 2) spr.sprite = state0;
        }
    }
}
