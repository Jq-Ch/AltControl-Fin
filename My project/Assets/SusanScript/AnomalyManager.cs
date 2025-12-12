using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    public List<AnomalyData> anomalyPool;   // 三类异常物的 ScriptableObject
    public List<ZoneSpawn> zones;           // 六个固定生成区

    public int maxAnomalies = 5;
    private List<GameObject> activeAnomalies = new List<GameObject>();
    public LayerMask groundMask;
    [Header("Spawn Timing")]
    public float minSpawnDelay = 3f;
    public float maxSpawnDelay = 6f;



    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (activeAnomalies.Count >= maxAnomalies)
            {
                Debug.Log("GAME OVER: Too many anomalies!");
                yield break;
            }

            // 调用正确的方法！
            TrySpawnAnomaly();

            float wait = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(wait);
        }
    }



    void TrySpawnAnomaly()
    {
        // 找所有未占用的 zone
        List<ZoneSpawn> freeZones = zones.FindAll(z => !z.isOccupied);

        if (freeZones.Count == 0)
        {
            Debug.Log("No free zones available.");
            return;
        }

        // 随机选一个空 zone
        ZoneSpawn chosenZone = freeZones[Random.Range(0, freeZones.Count)];

        // 随机选异常物类型
        AnomalyData data = anomalyPool[Random.Range(0, anomalyPool.Count)];

        // 在 spawnPoint 精准生成
        GameObject obj = Instantiate(data.prefab, chosenZone.spawnPoint.position, chosenZone.spawnPoint.rotation);

        // 设置异常物数据
        Anomaly anomaly = obj.AddComponent<Anomaly>();
        anomaly.data = data;
        anomaly.zoneTag = chosenZone.zoneTag;  // 记录它来自哪个 zone
        anomaly.manager = this;                // 回调可能需要

        chosenZone.isOccupied = true;          // 占用此区域
        activeAnomalies.Add(obj);

    }

    // 给异常物调用：当它被玩家清除时取消占用
    public void FreeZone(string zoneTag, GameObject obj)
    {
        ZoneSpawn zone = zones.Find(z => z.zoneTag == zoneTag);
        if (zone != null) zone.isOccupied = false;

        activeAnomalies.Remove(obj);
    }

    public ZoneController GetZone(string tag)
    {
        ZoneSpawn z = zones.Find(zone => zone.zoneTag == tag);
        if (z == null) return null;

        return z.spawnPoint.GetComponentInParent<ZoneController>();
    }

  
    }


