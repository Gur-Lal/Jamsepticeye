using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform transformToFollow;
    [SerializeField] float Zoffset = -1f;
    [SerializeField, Range(0f, 10f)] float followLag = 5f;

    //Applies after the transform has done its movement for this frame
    void LateUpdate()
    {
        if (transformToFollow == null) return;

        Vector2 targetPos = transformToFollow.position;
        Vector2 smoothedPos = Vector2.Lerp(transform.position, targetPos, followLag * Time.deltaTime); //slow lerp to pos
        transform.position = new Vector3(smoothedPos.x, smoothedPos.y, Zoffset);
    }
}
