using UnityEngine;
using TMPro;

public class WorkClockTimer : MonoBehaviour
{
    public TextMeshProUGUI clockText;

    [Header("Start Time (Displayed)")]
    public int startHour = 4;
    public int startMinute = 55;
    public int startSecond = 0;

    [Header("Win Time (Displayed)")]
    public int winHour = 5;
    public int winMinute = 0;
    public int winSecond = 0;

    [Tooltip("Run duration in REAL seconds. 300 = 5 minutes real-time.")]
    public float realSecondsToWin = 300f;

    private float elapsed;
    private bool ended;

    private int startSeconds;
    private int winSeconds;
    private int spanSeconds;

    void Start()
    {
        ended = false;
        elapsed = 0f;

        // 计算起止秒数
        startSeconds = startHour * 3600 + startMinute * 60 + startSecond;
        winSeconds = winHour * 3600 + winMinute * 60 + winSecond;
        spanSeconds = Mathf.Max(1, winSeconds - startSeconds);

        UpdateClockUI(startSeconds);

        // 统计清零建议只放一个地方（这里 or 别处二选一）
        if (StatsManager.Instance != null)
            StatsManager.Instance.ResetStats();
    }

    void Update()
    {
        if (ended) return;
        if (GameEndUI.Instance != null && GameEndUI.Instance.IsEnded) { ended = true; return; }

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, realSecondsToWin));

        int cur = startSeconds + Mathf.RoundToInt(spanSeconds * t);
        UpdateClockUI(cur);

        if (t >= 1f)
        {
            ended = true;
            if (GameEndUI.Instance != null) GameEndUI.Instance.ShowWin();
            else Time.timeScale = 0f;
        }
    }

    void UpdateClockUI(int totalSeconds)
    {
        int h = (totalSeconds / 3600) % 24;
        int m = (totalSeconds / 60) % 60;
        int s = totalSeconds % 60;

        if (clockText != null)
            clockText.text = $"{h:00}:{m:00}:{s:00}";
    }
}
