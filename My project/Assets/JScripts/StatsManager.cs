using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    public int SuccessCount { get; private set; }
    public int FailureCount { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RecordSuccess() => SuccessCount++;
    public void RecordFailure() => FailureCount++;

    public void ResetStats()
    {
        SuccessCount = 0;
        FailureCount = 0;
    }
}

