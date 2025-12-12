using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

public class VoiceInputController : MonoBehaviour
{
    [Header("Target Communication Manager")]
    public CommunicationManager comms;   // 在 Inspector 里拖 CommunicationManager 进来

    private KeywordRecognizer recognizer;
    private Dictionary<string, System.Action> keywordActions;

    void Start()
    {
        // 如果没拖引用，自动找场景里第一个
        if (comms == null)
        {
            comms = FindObjectOfType<CommunicationManager>();
        }

        // 关键字 -> 对应执行的函数
        keywordActions = new Dictionary<string, System.Action>
        {
            // 活体判断
            { "yes",    () => OnAliveAnswer(AliveAnswer.Yes) },
            { "yeah",   () => OnAliveAnswer(AliveAnswer.Yes) },
            { "no",     () => OnAliveAnswer(AliveAnswer.No) },
            { "unsure", () => OnAliveAnswer(AliveAnswer.Unsure) },
            { "not sure", () => OnAliveAnswer(AliveAnswer.Unsure) },

            // 区域：A~F
            { "a",      () => OnZoneAnswer("A") },
            { "b",      () => OnZoneAnswer("B") },
            { "c",      () => OnZoneAnswer("C") },
            { "d",      () => OnZoneAnswer("D") },
            { "e",      () => OnZoneAnswer("E") },
            { "f",      () => OnZoneAnswer("F") },

            // 你也可以加更清楚的词
            { "zone a", () => OnZoneAnswer("A") },
            { "zone b", () => OnZoneAnswer("B") },
            { "zone c", () => OnZoneAnswer("C") },
            { "zone d", () => OnZoneAnswer("D") },
            { "zone e", () => OnZoneAnswer("E") },
            { "zone f", () => OnZoneAnswer("F") },
        };

        // 初始化识别器
        recognizer = new KeywordRecognizer(keywordActions.Keys.ToArray(), ConfidenceLevel.Low);
        recognizer.OnPhraseRecognized += OnPhraseRecognized;
        recognizer.Start();

        Debug.Log("[VoiceInput] Started. Listening for: " + string.Join(", ", keywordActions.Keys));
    }

    void OnDestroy()
    {
        if (recognizer != null)
        {
            recognizer.OnPhraseRecognized -= OnPhraseRecognized;
            recognizer.Stop();
            recognizer.Dispose();
        }
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        string text = args.text.ToLower();
        Debug.Log("[VoiceInput] Heard: " + text);

        if (keywordActions.TryGetValue(text, out var action))
        {
            action?.Invoke();
        }
    }

    // ---------- 活体回答 ----------
    public enum AliveAnswer { Yes, No, Unsure }

    void OnAliveAnswer(AliveAnswer answer)
    {
        if (comms == null) return;

        // 通知 CommunicationManager（下面第 3 部分会加这些接口）
        switch (answer)
        {
            case AliveAnswer.Yes:
                comms.ReceiveAliveAnswerFromVoice(true, "Subject is alive.");
                break;
            case AliveAnswer.No:
                comms.ReceiveAliveAnswerFromVoice(false, "Subject is not alive.");
                break;
            case AliveAnswer.Unsure:
                comms.ReceiveAliveAnswerFromVoice(false, "Unsure.");
                break;
        }
    }

    // ---------- 区域回答 ----------
    void OnZoneAnswer(string zoneLetter)
    {
        if (comms == null) return;

        comms.ReceiveZoneAnswerFromVoice(zoneLetter);
    }


}
