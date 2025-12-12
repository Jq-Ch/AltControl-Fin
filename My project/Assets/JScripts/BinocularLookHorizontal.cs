using UnityEngine;

public class BinocularLookHorizontal : MonoBehaviour
{
    [Header("水平旋转速度（度/秒）")]
    public float rotateSpeed = 60f;

    private float currentYaw = 0f;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // 保持位置固定（可选）
        transform.position = initialPosition;

        // A / D 控制水平
        float inputX = Input.GetAxisRaw("Horizontal");

        // 累积旋转角度（不做 Clamp → 无限制旋转）
        currentYaw += inputX * rotateSpeed * Time.deltaTime;

        // 应用水平旋转
        transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
    }
}
