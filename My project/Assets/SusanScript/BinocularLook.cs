using UnityEngine;

/// <summary>
/// 望远镜旋转控制：围绕固定点转动，上下左右限制角度。
/// 把脚本挂在 BinocularPivot 上。
/// </summary>
public class BinocularLook : MonoBehaviour
{
    [Header("旋转速度（度/秒）")]
    public float rotateSpeed = 60f;

    [Header("左右最大角度（总共 180° 就填 90）")]
    public float maxHorizontalAngle = 90f;   // 左右各 90° = 总 180°

    [Header("上下最大角度（总共 120° 就填 60）")]
    public float maxVerticalAngle = 60f;     // 上下各 60°

    private float currentYaw = 0f;   // 左右（绕 Y 轴）
    private float currentPitch = 0f; // 上下（绕 X 轴）

    // 记录初始位置，确保 pivot 完全不移动（保险用）
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // 确保旋转点位置不变（即使有别的东西想动它，也拉回来）
        transform.position = initialPosition;

        // A / D → Horizontal，W / S → Vertical
        float inputX = Input.GetAxisRaw("Horizontal"); // A/D
        float inputY = Input.GetAxisRaw("Vertical");   // W/S

        // 左右旋转（绕 Y 轴）
        currentYaw += inputX * rotateSpeed * Time.deltaTime;

        // 上下旋转（绕 X 轴）——W 往上看，所以减去
        currentPitch -= inputY * rotateSpeed * Time.deltaTime;

        // 限制角度
        currentYaw = Mathf.Clamp(currentYaw, -maxHorizontalAngle, maxHorizontalAngle);
        currentPitch = Mathf.Clamp(currentPitch, -maxVerticalAngle, maxVerticalAngle);

        // 应用旋转（本地坐标系）
        transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}
