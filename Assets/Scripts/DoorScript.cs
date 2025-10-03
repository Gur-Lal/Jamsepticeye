using System.Linq.Expressions;
using UnityEngine;
using System.Collections;

public class DoorScript : IButtonActivated
{
    [SerializeField] bool StartOpen = false;
    [SerializeField, Range(0f, 1f)] float delay = 0.25f;
    Collider2D col;
    SpriteRenderer spr;
    bool open;
    void Start()
    {
        open = StartOpen;
        col = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
        setCorrectState();
    }

    IEnumerator ToggleAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Toggle();
    }

    public override void OnButtonTrigger()
    {
        Toggle(); //toggle immediately
    }

    public override void OnButtonDisable()
    {
        StartCoroutine(ToggleAfterDelay()); //toggle after delay
    }

    void Toggle()
    {
        open = !open; //flip bool
        setCorrectState();
    }

    void setCorrectState()
    {
        if (open)
        {
            col.enabled = true;
            spr.color = Color.white;
        }
        else
        {
            col.enabled = false;
            spr.color = Color.grey;
        }
    }
}
