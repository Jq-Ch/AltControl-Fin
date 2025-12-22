using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Collections.Generic;
using System.Linq;

public class VoiceInputController : MonoBehaviour
{
    [Header("Target Communication Manager")]
    public CommunicationManager comms;

    [Header("Recognizer Settings")]
    [Tooltip("Low = easier to trigger (more false positives). Medium/High = safer.")]
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    [Tooltip("If true, will log all recognized phrases and confidence.")]
    public bool verboseLog = true;

    [Header("Zone Phrases")]
    [Tooltip("If true, also enable 'zone <alias>' variants (recommended).")]
    public bool enableZonePrefixAliases = true;

    private KeywordRecognizer recognizer;
    private Dictionary<string, Action> keywordActions;

    // -------------------------
    // Unity Lifecycle
    // -------------------------
    private void Start()
    {
        // Auto-find comms if not assigned
        if (comms == null)
            comms = FindObjectOfType<CommunicationManager>();

        BuildKeywordActions();

        // Make sure we have something to listen to
        if (keywordActions == null || keywordActions.Count == 0)
        {
            Debug.LogError("[VoiceInput] No keywords registered. Recognizer will not start.");
            return;
        }

        // Create + start recognizer
        recognizer = new KeywordRecognizer(keywordActions.Keys.ToArray(), confidence);
        recognizer.OnPhraseRecognized += OnPhraseRecognized;
        recognizer.Start();

        Debug.Log("[VoiceInput] Started.");
        Debug.Log("[VoiceInput] Confidence: " + confidence);
        Debug.Log("[VoiceInput] Keywords count: " + keywordActions.Count);
        Debug.Log("[VoiceInput] Listening for: " + string.Join(", ", keywordActions.Keys));

        // Sanity check (common debugging)
        Debug.Log("[VoiceInput] Has YES? " + keywordActions.ContainsKey("yes"));
        Debug.Log("[VoiceInput] Has NO? " + keywordActions.ContainsKey("no"));
    }

    private void OnDisable()
    {
        StopRecognizer();
    }

    private void OnDestroy()
    {
        StopRecognizer();
    }

    // -------------------------
    // Recognizer handling
    // -------------------------
    private void StopRecognizer()
    {
        if (recognizer == null) return;

        try
        {
            recognizer.OnPhraseRecognized -= OnPhraseRecognized;

            if (recognizer.IsRunning)
                recognizer.Stop();

            recognizer.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogWarning("[VoiceInput] StopRecognizer exception: " + e.Message);
        }
        finally
        {
            recognizer = null;
        }
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        string text = (args.text ?? string.Empty).Trim().ToLowerInvariant();

        if (verboseLog)
            Debug.Log($"[VoiceInput] Heard: '{text}' | Confidence: {args.confidence} | Time: {args.phraseDuration.TotalSeconds:F2}s");


        if (string.IsNullOrEmpty(text))
            return;

        if (keywordActions != null && keywordActions.TryGetValue(text, out var action))
        {
            action?.Invoke();
        }
        else
        {
            if (verboseLog)
                Debug.Log("[VoiceInput] Unhandled phrase: " + text);
        }
    }

    // -------------------------
    // Keywords map
    // -------------------------
    private void BuildKeywordActions()
    {
        keywordActions = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);

        // ---- Alive answers ----
        AddKeyword("yes", () => OnAliveAnswer(AliveAnswer.Yes));
        AddKeyword("yeah", () => OnAliveAnswer(AliveAnswer.Yes));
        AddKeyword("yep", () => OnAliveAnswer(AliveAnswer.Yes));
        AddKeyword("affirmative", () => OnAliveAnswer(AliveAnswer.Yes));

        AddKeyword("no", () => OnAliveAnswer(AliveAnswer.No));
        AddKeyword("nope", () => OnAliveAnswer(AliveAnswer.No));
        AddKeyword("negative", () => OnAliveAnswer(AliveAnswer.No));

        AddKeyword("unsure", () => OnAliveAnswer(AliveAnswer.Unsure));
        AddKeyword("not sure", () => OnAliveAnswer(AliveAnswer.Unsure));
        AddKeyword("unknown", () => OnAliveAnswer(AliveAnswer.Unsure));

        // ---- Zones (use distinct phrases; avoid single-word aliases) ----
        // A
        AddKeyword("black cliff", () => OnZoneAnswer("A"));
        if (enableZonePrefixAliases)
        {
            AddKeyword("zone black cliff", () => OnZoneAnswer("A"));
            AddKeyword("zone cliff", () => OnZoneAnswer("A"));
        }

        // B
        AddKeyword("mushroom circle", () => OnZoneAnswer("B"));
        if (enableZonePrefixAliases)
        {
            AddKeyword("fairy circle", () => OnZoneAnswer("B"));
            AddKeyword("zone mushroom", () => OnZoneAnswer("B"));
        }

        // C
        AddKeyword("raven mile", () => OnZoneAnswer("C"));
        if (enableZonePrefixAliases)
        {
            AddKeyword("zone raven mile", () => OnZoneAnswer("C"));
            AddKeyword("zone mile", () => OnZoneAnswer("C"));
        }

        // D
        AddKeyword("cinder creek", () => OnZoneAnswer("D"));
        if (enableZonePrefixAliases)
        {
            AddKeyword("zone cinder creek", () => OnZoneAnswer("D"));
            AddKeyword("zone creek", () => OnZoneAnswer("D"));
        }

        // E
        AddKeyword("mine cave", () => OnZoneAnswer("E"));
        if (enableZonePrefixAliases)
        {
            AddKeyword("zone mine cave", () => OnZoneAnswer("E"));
            AddKeyword("zone cave", () => OnZoneAnswer("E"));
        }

        // F
        AddKeyword("raccoon cabin", () => OnZoneAnswer("F"));
        if (enableZonePrefixAliases)
        {
            AddKeyword("zone raccoon cabin", () => OnZoneAnswer("F"));
            AddKeyword("zone cabin", () => OnZoneAnswer("F"));
        }
    }

    private void AddKeyword(string key, Action action)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        key = key.Trim().ToLowerInvariant();

        // Avoid duplicates silently (or warn)
        if (keywordActions.ContainsKey(key))
        {
            Debug.LogWarning("[VoiceInput] Duplicate keyword ignored: " + key);
            return;
        }

        keywordActions.Add(key, action);
    }

    // -------------------------
    // Alive / Zone dispatch
    // -------------------------
    public enum AliveAnswer { Yes, No, Unsure }

    private void OnAliveAnswer(AliveAnswer answer)
    {
        if (comms == null)
        {
            if (verboseLog) Debug.LogWarning("[VoiceInput] CommunicationManager is null.");
            return;
        }

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

    private void OnZoneAnswer(string zoneLetter)
    {
        if (comms == null)
        {
            if (verboseLog) Debug.LogWarning("[VoiceInput] CommunicationManager is null.");
            return;
        }

        comms.ReceiveZoneAnswerFromVoice(zoneLetter);
    }
}
