using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform transformToFollow;
    [SerializeField] public float Zoffset = -1f;
    [SerializeField] public float Yoffset = 3f;
    [SerializeField, Range(0f, 10f)] float followLagX = 5f;
    [SerializeField, Range(0f, 10f)] float followLagY = 2f;

    //Applies after the transform has done its movement for this frame
    void LateUpdate()
    {
        if (transformToFollow == null) return;

        Vector2 targetPos = transformToFollow.position;
        targetPos.y += Yoffset;

        float newX = Mathf.Lerp(transform.position.x, targetPos.x, followLagX * Time.deltaTime); //slow lerp to pos
        float newY = Mathf.Lerp(transform.position.y, targetPos.y, followLagY * Time.deltaTime); //slow lerp to pos

        transform.position = new Vector3(newX, newY, Zoffset);
    }
}
