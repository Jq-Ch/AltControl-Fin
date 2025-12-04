using UnityEngine;
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
        transform.position = initialPosition;

        // A / D → Horizontal，W / S → Vertical
        float inputX = Input.GetAxisRaw("Horizontal"); // A/D
        float inputY = Input.GetAxisRaw("Vertical");   // W/S

        currentYaw += inputX * rotateSpeed * Time.deltaTime;

        currentPitch -= inputY * rotateSpeed * Time.deltaTime;

        // 限制角度
        currentYaw = Mathf.Clamp(currentYaw, -maxHorizontalAngle, maxHorizontalAngle);
        currentPitch = Mathf.Clamp(currentPitch, -maxVerticalAngle, maxVerticalAngle);

        // 应用旋转（本地坐标系）
        transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}
