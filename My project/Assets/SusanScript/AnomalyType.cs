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
    public float heightOffset = 0f;   // 用来把模型往上抬一点，如果 pivot 在中间就设 0.5 之类

    public string anomalyName;
    public AnomalyType type;
    public GameObject prefab;

}
