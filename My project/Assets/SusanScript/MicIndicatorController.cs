using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MicIndicatorController : MonoBehaviour
{
    [Header("Mic UI")]
    public Image micImage;

    [Header("Alpha Settings")]
    public float idleAlpha = 0.5f;
    public float activeAlpha = 1f;
    public float blinkInterval = 0.25f;

    Coroutine blinkRoutine;

    void Awake()
    {
        micImage.gameObject.SetActive(false);
    }

    // ===============================
    // 通信开始（按 Y）
    // ===============================
    public void OnCommunicationStart()
    {
        micImage.gameObject.SetActive(true);
        SetAlpha(idleAlpha);
        StopBlink();
    }

    // ===============================
    // 检测到玩家正在说话
    // ===============================
    public void OnVoiceDetected()
    {
        if (blinkRoutine == null)
            blinkRoutine = StartCoroutine(BlinkMic());
    }

    // ===============================
    // 玩家停止说话
    // ===============================
    public void OnVoiceEnded()
    {
        StopBlink();
        SetAlpha(idleAlpha);
    }

    // ===============================
    // 通信结束
    // ===============================
    public void OnCommunicationEnd()
    {
        StopBlink();
        micImage.gameObject.SetActive(false);
    }

    // ===============================
    IEnumerator BlinkMic()
    {
        while (true)
        {
            SetAlpha(activeAlpha);
            yield return new WaitForSeconds(blinkInterval);

            SetAlpha(idleAlpha);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    void StopBlink()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }
    }

    void SetAlpha(float a)
    {
        Color c = micImage.color;
        c.a = a;
        micImage.color = c;
    }
}
