using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndUI : MonoBehaviour
{
    public static GameEndUI Instance { get; private set; }

    [Header("Panel Root")]
    public GameObject endPanel;

    [Header("Texts")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI line1Text;
    public TextMeshProUGUI line2Text;
    public TextMeshProUGUI footerText;

    private bool ended;

    void Awake()
    {
        Instance = this; // 场景内只放一个
    }

    void Start()
    {
        SetGameRunningState(); // 🔑 开局就是“游戏进行中态”
    }

    public bool IsEnded => ended;

    // =========================
    // WIN
    // =========================
    public void ShowWin()
    {
        if (ended) return;
        ended = true;

        int success = StatsManager.Instance != null ? StatsManager.Instance.SuccessCount : 0;
        int fail = StatsManager.Instance != null ? StatsManager.Instance.FailureCount : 0;

        if (endPanel != null) endPanel.SetActive(true);

        if (titleText != null) titleText.text = "Today's work is completed.";
        if (line1Text != null) line1Text.text = $"You successfully contained {success} time(s).";
        if (line2Text != null) line2Text.text = $"You failed {fail} time(s).";
        if (footerText != null) footerText.text = "Please try harder next time.";

        Time.timeScale = 0f;
    }

    // =========================
    // DEATH
    // =========================
    public void ShowDeath()
    {
        if (ended) return;
        ended = true;

        int success = StatsManager.Instance != null ? StatsManager.Instance.SuccessCount : 0;
        int fail = StatsManager.Instance != null ? StatsManager.Instance.FailureCount : 0;

        if (endPanel != null) endPanel.SetActive(true);

        if (titleText != null) titleText.text = "YOU DIED";
        if (line1Text != null) line1Text.text = $"You successfully contained {success} time(s).";
        if (line2Text != null) line2Text.text = $"You failed {fail} time(s).";
        if (footerText != null) footerText.text = "Please try harder next time.";

        Time.timeScale = 0f;
    }

    // =========================
    // RESTART
    // =========================
    public void Restart()
    {
        Time.timeScale = 1f;

        // 防止闪屏
        SetGameRunningState();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =========================
    // RUNNING STATE
    // =========================
    private void SetGameRunningState()
    {
        ended = false;

        // 🔑 清空文字，避免 “New Text”
        if (titleText != null) titleText.text = "";
        if (line1Text != null) line1Text.text = "";
        if (line2Text != null) line2Text.text = "";
        if (footerText != null) footerText.text = "";

        if (endPanel != null) endPanel.SetActive(false);
    }
}
