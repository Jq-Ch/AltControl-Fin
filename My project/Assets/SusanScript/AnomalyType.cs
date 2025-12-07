using UnityEngine;

public enum AnomalyType
{
    StaticLiving,       // 活物（看着活，但不移动）
    StealthLiving,      // 潜行动物（不在视野时靠近玩家刷新）
    StaticDead          // 死物（台灯/电脑）
}

[CreateAssetMenu(fileName = "AnomalyData", menuName = "Game/Anomaly Data")]
public class AnomalyData : ScriptableObject
{
    [Header("Ground Offset")]
    public float heightOffset = 0.5f;   // 用来把模型往上抬一点，如果 pivot 在中间就设 0.5 之类

    public string anomalyName;
    public AnomalyType type;
    public GameObject prefab;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 10f;

    [Header("Stealth Settings (for StealthLiving only)")]
    public float minRespawnDistance = 5f;  // 和玩家的最小距离
    public float maxRespawnDistance = 20f; // 最大距离
}
