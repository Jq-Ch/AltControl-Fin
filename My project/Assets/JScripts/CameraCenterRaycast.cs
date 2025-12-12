using UnityEngine;

public class CameraCenterRaycast : MonoBehaviour
{
    [Header("Ray 长度")]
    public float rayLength = 20f;

    [Header("Gizmo 颜色")]
    public Color gizmoColor = Color.red;

    private Ray _centerRay;

    void Update()
    {
        // 从相机中心点发射 Ray（屏幕中心）
        _centerRay = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)
        );

        // 进行射线检测
        if (Physics.Raycast(_centerRay, out RaycastHit hit, rayLength))
        {
            Debug.Log("Hit: " + hit.collider.name);
        }
    }

    // Scene 视图中绘制 Gizmo
    private void OnDrawGizmos()
    {
        if (Camera.main == null) return;

        Gizmos.color = gizmoColor;

        // 绘制一条从摄像机中心向前的可视化线
        Vector3 start = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        Gizmos.DrawLine(start, start + direction * rayLength);
        Gizmos.DrawSphere(start + direction * rayLength, 0.05f);
    }
}
