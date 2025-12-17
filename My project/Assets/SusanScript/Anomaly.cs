using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Anomaly : MonoBehaviour
{
    public AnomalyData data;        // ScriptableObject 数据
    public string zoneTag;          // 这个异常属于哪一个 Zone（由 Spawn 时分配）
    public AnomalyManager manager;  // manager 用于释放 zone
                                    // === Stealth Living Watch Logic ===
    bool wasRayHit = false;      // 曾经被 ray 扫到过
    bool isCurrentlyHit = false;
    float lostSightTimer = 0f;

    public float teleportDelay = 2f;


    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

   void Update()
{
    if (data.type != AnomalyType.StealthLiving)
        return;

    HandleStealthLiving();
}

    void HandleStealthLiving()
    {
        isCurrentlyHit = IsHitByCenterRay();

        // 第一次被 ray 扫到
        if (isCurrentlyHit)
        {
            wasRayHit = true;
            lostSightTimer = 0f;
            return;
        }

        // 曾被看到，但现在没被看
        if (wasRayHit && !isCurrentlyHit)
        {
            lostSightTimer += Time.deltaTime;

            if (lostSightTimer >= teleportDelay)
            {
                TeleportToAnotherZone();
                ResetStealthState();
            }
        }
    }

    void TeleportToAnotherZone()
    {
        List<ZoneSpawn> freeZones = manager.zones
            .Where(z => !z.isOccupied && z.zoneTag != zoneTag)
            .ToList();

        if (freeZones.Count == 0)
            return;

        ZoneSpawn newZone = freeZones[Random.Range(0, freeZones.Count)];

        // 释放旧 zone
        manager.FreeZone(zoneTag, gameObject);

        // 占用新 zone
        zoneTag = newZone.zoneTag;
        newZone.isOccupied = true;

        // 瞬移
        transform.position = newZone.spawnPoint.position;
    }

    void ResetStealthState()
    {
        wasRayHit = false;
        isCurrentlyHit = false;
        lostSightTimer = 0f;
    }


    bool IsHitByCenterRay()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        float angle = Vector3.Angle(Camera.main.transform.forward, dir);

        if (angle > 2f) return false; // 2° 很“准心感”，你可以调

        float distance = dir.magnitude;
        return distance < 100f; // 望远镜可视距离
    }

    // ===============================
    // 视野检测
    // ===============================
    private bool IsInPlayerView()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        float angle = Vector3.Angle(Camera.main.transform.forward, dir);

        // 视野角度可调
        return angle < 80f;
    }

    // ===============================
    // StealthLiving 行为逻辑
    // ===============================
    private void HandleStealthBehavior()
    {
        if (IsInPlayerView())
            return; // 在玩家视野内 → 不动

        RespawnInsideZone(); // 不在视野内 → 刷新位置（但不换 Zone！）
    }

    // ===============================
    // 在 Zone 内随机位置刷新
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
    // 销毁异常物（上报正确时调用）
    // ===============================
    public void Remove()
    {
        manager.FreeZone(zoneTag, gameObject);
        Destroy(gameObject);
    }
}
