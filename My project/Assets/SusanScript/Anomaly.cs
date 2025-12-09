using UnityEngine;

public class Anomaly : MonoBehaviour
{
    public AnomalyData data;
    private Transform player;
    public string zoneTag;        // 哪个区域生成的
    public AnomalyManager manager;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (data.type == AnomalyType.StealthLiving)
        {
            HandleStealthBehavior();
        }
    }

    private void HandleStealthBehavior()
    {
        // 玩家视野检查
        if (IsInPlayerView()) return;

        // 不在视野 → 靠近玩家随机刷新
        RespawnNearPlayer();
    }

    bool IsInPlayerView()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        float angle = Vector3.Angle(Camera.main.transform.forward, dir);

        // 视野角度 80° 可自己调
        return angle < 80f;
    }

    void RespawnNearPlayer()
    {
        float dist = Random.Range(
            data.minRespawnDistance,
            data.maxRespawnDistance
        );

        float randomAngle = Random.Range(-90f, 90f); // 玩家前方 180°
        Vector3 offset = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward * dist;

        transform.position = player.position + offset;
    }

    public void Remove()
    {
        manager.FreeZone(zoneTag, gameObject);
        Destroy(gameObject);
    }
}
