using UnityEngine;

public class BobAnimation : MonoBehaviour
{
    [SerializeField, Range(0.1f, 1f)] float amplitude = 0.5f;
    [SerializeField, Range(0.1f, 2f)] float frequency = 1f;

    Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
