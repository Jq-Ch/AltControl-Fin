using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    public List<AnomalyData> anomalyPool;
    public Transform player;

    [Header("Spawn Area")]
    public BoxCollider spawnArea;   // 生成区域限制（你拖进去）

    [Header("Spawn Settings")]
    public float minSeparation = 5f;

    [Header("Limit")]
    public int maxAnomalies = 10;


    [Header("Ground Raycast")]
    public LayerMask groundMask;        // 地面所在 Layer


    private List<GameObject> activeAnomalies = new List<GameObject>();



    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (activeAnomalies.Count < maxAnomalies)
            {
                SpawnAnAnomaly();
            }

            AnomalyData data = anomalyPool[Random.Range(0, anomalyPool.Count)];
            float wait = Random.Range(data.minSpawnInterval, data.maxSpawnInterval);
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnAnAnomaly()
    {
        // 1. 随机选一个类型
        AnomalyData data = anomalyPool[Random.Range(0, anomalyPool.Count)];

        // 2. 用这个 data 去找生成位置（带它自己的 heightOffset）
        Vector3 pos = GetValidSpawnPosition(data);

        // 3. 生成
        GameObject obj = Instantiate(data.prefab, pos, Quaternion.identity);

        Anomaly anomaly = obj.AddComponent<Anomaly>();
        anomaly.data = data;

        activeAnomalies.Add(obj);

        if (activeAnomalies.Count >= maxAnomalies)
        {
            Debug.Log("Game Over: Too many anomalies!");
            // TODO: 在这里调用你的 GameOver UI / 场景切换
        }
    }


    // ---- NEW: 限制生成在地板区域内 ----
    Vector3 GetValidSpawnPosition(AnomalyData data)
    {
        // 最多试 30 次找一个不和其他异常太近的位置
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPosXZ = GetRandomPointOnGroundInsideBox();

            // 防止和现有异常重叠太近
            if (!OverlapWithOthers(randomPosXZ))
            {
                // 加上每种异常物自己的高度偏移
                randomPosXZ.y += data.heightOffset;
                return randomPosXZ;
            }
        }

        // 实在找不到就随便给一个（至少在地上）
        Vector3 fallback = GetRandomPointOnGroundInsideBox();
        fallback.y += data.heightOffset;
        return fallback;
    }

    /// <summary>
    /// 在 BoxCollider 范围内随机一个点，然后用射线从上往下找到真正的地面 Y。
    /// </summary>
    Vector3 GetRandomPointOnGroundInsideBox()
    {
        if (spawnArea == null)
        {
            Debug.LogError("SpawnArea (BoxCollider) 没有设置！");
            return player.position;
        }

        // 在 Box 的 local 空间里随机一个点，再转成世界坐标
        Vector3 boxCenter = spawnArea.center;
        Vector3 boxSize = spawnArea.size;

        Vector3 localRandom = new Vector3(
            Random.Range(-boxSize.x * 0.5f, boxSize.x * 0.5f),
            boxSize.y * 0.5f, // 取 Box 顶部
            Random.Range(-boxSize.z * 0.5f, boxSize.z * 0.5f)
        );

        Vector3 worldFrom = spawnArea.transform.TransformPoint(boxCenter + localRandom);

        // 从上往下打射线找到地面
        Ray ray = new Ray(worldFrom + Vector3.up * 10f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, groundMask))
        {
            return hit.point;   // 正好在地面上
        }

        // 万一没打到地，就退回原来的随机点
        return worldFrom;
    }


    Vector3 GetRandomPointInsideBox(BoxCollider box)
    {
        Vector3 center = box.transform.position + box.center;
        Vector3 size = box.size;

        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = center.y;
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, y, z);
    }

    bool OverlapWithOthers(Vector3 pos)
    {
        foreach (var a in activeAnomalies)
        {
            if (Vector3.Distance(a.transform.position, pos) < minSeparation)
                return true;
        }
        return false;
    }
}
