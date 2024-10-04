using UnityEngine;

namespace Common
{
    public class GizmoDrawer
    {
        public static void DrawCross(Vector3 point, Color color)
        {
#if UNITY_EDITOR
            Debug.DrawRay(point, Vector3.right, color, 5f);
            Debug.DrawRay(point, Vector3.left, color, 5f);
            Debug.DrawRay(point, Vector3.forward, color, 5f);
            Debug.DrawRay(point, Vector3.back, color, 5f);
#endif
        }
    }
}