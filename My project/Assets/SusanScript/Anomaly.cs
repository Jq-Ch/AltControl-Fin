using UnityEngine;

public class Anomaly : MonoBehaviour
{
    public AnomalyData data;        // ScriptableObject 数据
    public string zoneTag;          // 这个异常属于哪一个 Zone（由 Spawn 时分配）
    public AnomalyManager manager;  // manager 用于释放 zone

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (data.type == AnomalyType.StealthLiving)
        {
            HandleStealthBehavior();
        }
        // DeadObject 类型不需要 Update
    }

    // ===============================
    // 🟣 视野检测
    // ===============================
    private bool IsInPlayerView()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        float angle = Vector3.Angle(Camera.main.transform.forward, dir);

        // 视野角度可调
        return angle < 80f;
    }

    // ===============================
    // 🟣 StealthLiving 行为逻辑
    // ===============================
    private void HandleStealthBehavior()
    {
        if (IsInPlayerView())
            return; // 在玩家视野内 → 不动

        RespawnInsideZone(); // 不在视野内 → 刷新位置（但不换 Zone！）
    }

    // ===============================
    // 🟣 在 Zone 内随机位置刷新
    // ===============================
    private void RespawnInsideZone()
    {
        ZoneController zone = manager.GetZone(zoneTag);
        if (zone == null)
        {
            Debug.LogError("Zone not found: " + zoneTag);
            return;
        }

        // 随机 local 坐标（XZ 平面）
        Vector3 localRandom = new Vector3(
            Random.Range(-zone.range.x * 0.5f, zone.range.x * 0.5f),
            0,
            Random.Range(-zone.range.z * 0.5f, zone.range.z * 0.5f)
        );

        // 转为世界坐标
        Vector3 worldGuess = zone.transform.TransformPoint(localRandom);

        // 落地（Raycast）
        if (Physics.Raycast(worldGuess + Vector3.up * 5f, Vector3.down,
            out RaycastHit hit, 20f, manager.groundMask))
        {
            transform.position = hit.point + Vector3.up * data.heightOffset;
        }
    }

    // ===============================
    // 🟣 销毁异常物（上报正确时调用）
    // ===============================
    public void Remove()
    {
        manager.FreeZone(zoneTag, gameObject);
        Destroy(gameObject);
    }
}
