using UnityEngine;

public class ResetStatsOnMainScene : MonoBehaviour
{
    void Awake()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ResetStats();
        }
    }
}
