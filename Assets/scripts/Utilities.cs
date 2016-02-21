using UnityEngine;
using System.Collections;

public static class Utilities {
    public static void drawPointer2D(Vector2 center, float length = 1.0f, float duration = 5.0f) {
        Debug.DrawLine(new Vector3(center.x - (length/2), center.y, 0), new Vector3(center.x + (length/2), center.y, 0), Color.green, duration);
        Debug.DrawLine(new Vector3(center.x, center.y - (length / 2), 0), new Vector3(center.x, center.y + (length / 2), 0), Color.green, duration);
    }
}
