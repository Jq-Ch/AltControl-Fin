using UnityEngine;
using TMPro;
using System.Collections;

public class UICommsController : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject uiRoot;

    [Header("UI Elements")]
    public TextMeshProUGUI systemTitle;
    public TextMeshProUGUI systemText;
    public TextMeshProUGUI systemDots; // ⭐ 专门显示省略号
    public TextMeshProUGUI playerText;
    public GameObject systemCursor;

    [Header("Typing Speed")]
    public float typingSpeed = 0.03f;

    private Coroutine systemTypingRoutine;
    private Coroutine playerTypingRoutine;


    // ================================================================
    void Start()
    {
        uiRoot.SetActive(false);

        systemTitle.text = "<color=#6EC1FF>System:</color>";
        systemText.text = "";
        systemDots.text = ""; // 初始化
        playerText.text = "";

        StartCoroutine(CursorBlink());
    }

    // ================================================================
    // UI VISIBILITY
    // ================================================================
    public void SetUIVisible(bool visible)
    {
        uiRoot.SetActive(visible);
        if (!visible)
            ClearAll();
    }

    public void HidePlayerMessage()
    {
        playerText.text = "";
        playerText.alpha = 0f;
    }

    public void SetSystemAlpha(float a)
    {
        Color c = systemText.color;
        c.a = a;
        systemText.color = c;
    }

    public void RestoreSystemAlpha()
    {
        Color c = systemText.color;
        c.a = 1f;
        systemText.color = c;
    }

    // ================================================================
    // CURSOR BLINK
    // ================================================================
    IEnumerator CursorBlink()
    {
        while (true)
        {
            systemCursor.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            systemCursor.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    // ================================================================
    // SYSTEM MESSAGE
    // ================================================================
    public void ShowSystemMessage(string msg, System.Action onComplete = null)
    {
        if (systemTypingRoutine != null)
            StopCoroutine(systemTypingRoutine);

        string quoted = $"“{msg}”";
        systemTypingRoutine = StartCoroutine(TypeText(systemText, quoted, onComplete));
    }

    public void ForceSystemText(string text)
    {
        systemText.text = text;
    }

    // 用于 “...”
    public void ForceDots(string t)
    {
        systemDots.text = t;
    }

    // ================================================================
    // DOT BLINK（专门用于结算中“...”闪烁）
    // ================================================================
    public IEnumerator DotBlink(System.Func<bool> keepBlinking)
    {
        while (keepBlinking())
        {
            systemDots.alpha = 1f;
            yield return new WaitForSeconds(0.4f);

            systemDots.alpha = 0.25f;
            yield return new WaitForSeconds(0.4f);
        }
        systemDots.alpha = 1f;
    }

    // ================================================================
    // PLAYER MESSAGE
    // ================================================================
    public void ShowPlayerMessage(string msg, System.Action onComplete = null)
    {
        if (playerTypingRoutine != null)
            StopCoroutine(playerTypingRoutine);

        string quoted = $"“{msg}”";
        playerTypingRoutine = StartCoroutine(TypeText(playerText, quoted, onComplete));
    }

    // ================================================================
    // TYPING
    // ================================================================
    IEnumerator TypeText(TextMeshProUGUI ui, string full, System.Action onComplete)
    {
        ui.text = "";

        foreach (char c in full)
        {
            ui.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        onComplete?.Invoke();
    }

    // ================================================================
    // HIDDEN TYPE（用于后台“打字但不可见”）
    // ================================================================
    public IEnumerator TypeSilently(string msg)
    {
        string quoted = $"“{msg}”";

        systemText.text = "";
        float oldAlpha = systemText.color.a;

        SetSystemAlpha(0f);

        foreach (char c in quoted)
            yield return new WaitForSeconds(typingSpeed);

        // 不恢复透明度，由外部控制
    }

    // ================================================================
    // RESET
    // ================================================================
    public void HideAll()
    {
        StopAllCoroutines();
        uiRoot.SetActive(false);

        systemText.text = "";
        systemDots.text = "";
        playerText.text = "";
    }

    public void ClearAll()
    {
        systemText.text = "";
        systemDots.text = "";
        playerText.text = "";
    }
}
