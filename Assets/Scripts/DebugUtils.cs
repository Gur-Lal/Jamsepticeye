using UnityEngine;

public static class DebugUtils
{
    public static void DebugDrawBox2D(Vector2 center, Vector2 size, Color color, float duration = 0f)
    {
        Vector2 half = size * 0.5f;

        Vector2 bl = center + new Vector2(-half.x, -half.y);
        Vector2 br = center + new Vector2( half.x, -half.y);
        Vector2 tr = center + new Vector2( half.x,  half.y);
        Vector2 tl = center + new Vector2(-half.x,  half.y);

        Debug.DrawLine(bl, br, color, duration);
        Debug.DrawLine(br, tr, color, duration);
        Debug.DrawLine(tr, tl, color, duration);
        Debug.DrawLine(tl, bl, color, duration);
    }
}
