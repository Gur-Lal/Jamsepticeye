using UnityEngine;

public class SeamlessScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;
    public float tilemapWidth = 34f;

    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x <= -tilemapWidth)
        {
            transform.position += Vector3.right * tilemapWidth;
        }
    }
}
