using UnityEngine;
using System.Collections;

public class CommunicationManager : MonoBehaviour
{
    public enum CommsState { Idle, AskAlive, AskZone, AwaitLightCheck, ShowResult }

    private CommsState state = CommsState.Idle;

    [Header("References")]
    public Light spotlight;
    public UICommsController ui;

    [Header("Runtime Target")]
    public Transform targetAnomaly;

    // Answers
    private bool correctIsAlive;
    private string correctZoneTag;

    private bool playerIsAliveAnswer;
    private string playerZoneAnswer;

    private bool isCommunicating = false;


    // ================================================================
    void Update()
    {
        if (!isCommunicating)
        {
            if (Input.GetKeyDown(KeyCode.Y))
                StartCommunication();
            return;
        }

        if (state == CommsState.AskAlive) HandleAliveInput();
        if (state == CommsState.AskZone) HandleZoneInput();
    }

    // ================================================================
    // START COMMUNICATION
    // ================================================================
    public void StartCommunication()
    {
        isCommunicating = true;
        ui.SetUIVisible(true);

        ui.ShowSystemMessage("An anomaly has been detected.\nPlease answer the following questions.");
        Invoke(nameof(AskAliveQuestion), 3f);
    }

    void AskAliveQuestion()
    {
        state = CommsState.AskAlive;
        ui.ShowSystemMessage("Is the object alive?\n(R = Yes / B = No / G = Unsure)");
    }

    // ================================================================
    // ALIVE QUESTION INPUT
    // ================================================================
    void HandleAliveInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
            AnswerAlive(true, "Subject is alive.");
        else if (Input.GetKeyDown(KeyCode.B))
            AnswerAlive(false, "Subject is not alive.");
        else if (Input.GetKeyDown(KeyCode.G))
            AnswerAlive(false, "Unsure.");
    }

    void AnswerAlive(bool value, string text)
    {
        playerIsAliveAnswer = value;

        ui.ShowPlayerMessage(text, () =>
        {
            StartCoroutine(NextAfterAlive());
        });
    }

    IEnumerator NextAfterAlive()
    {
        yield return new WaitForSeconds(1f);
        AskZoneQuestion();
    }

    // ================================================================
    // ZONE QUESTION
    // ================================================================
    void AskZoneQuestion()
    {
        state = CommsState.AskZone;
        ui.ShowSystemMessage(
            "Which zone are you locating at?\n" +
            "RB=A, RG=B, BG=C, BR=D, GR=E, GB=F"
        );
    }

    KeyCode firstZoneKey = KeyCode.None;

    void HandleZoneInput()
    {
        if (firstZoneKey == KeyCode.None)
        {
            if (Input.GetKeyDown(KeyCode.R)) firstZoneKey = KeyCode.R;
            else if (Input.GetKeyDown(KeyCode.B)) firstZoneKey = KeyCode.B;
            else if (Input.GetKeyDown(KeyCode.G)) firstZoneKey = KeyCode.G;
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) ||
            Input.GetKeyDown(KeyCode.B) ||
            Input.GetKeyDown(KeyCode.G))
        {
            KeyCode second =
                Input.GetKeyDown(KeyCode.R) ? KeyCode.R :
                Input.GetKeyDown(KeyCode.B) ? KeyCode.B :
                KeyCode.G;

            playerZoneAnswer = ConvertKeysToZone(firstZoneKey, second);

            firstZoneKey = KeyCode.None;

            ui.ShowPlayerMessage("Reporting Zone: " + playerZoneAnswer, () =>
            {
                StartCoroutine(NextAfterZone());
            });
        }
    }

    IEnumerator NextAfterZone()
    {
        yield return new WaitForSeconds(1f);
        BeginLightPhase();
    }

    // ================================================================
    // LIGHT PHASE
    // ================================================================
    void BeginLightPhase()
    {
        state = CommsState.AwaitLightCheck;
        ui.ShowSystemMessage("Please keep your lights on.");

        Invoke(nameof(DispatchTeam), 2f);
    }

    void DispatchTeam()
    {
        ui.HidePlayerMessage();
        ui.ShowSystemMessage("Containment team dispatched.");

        StartCoroutine(ScanForAnomaly());
    }

    IEnumerator ScanForAnomaly()
    {
        float timer = 0f;
        float duration = 5f;
        Transform found = null;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            found = RaycastForAnomaly();
            if (found != null)
            {
                EvaluateAll(found);
                yield break;
            }

            yield return null;
        }

        // timeout
        yield return StartCoroutine(ShowFinalReport(false, false, false));
    }

    Transform RaycastForAnomaly()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        int ignoreZoneMask = ~(1 << LayerMask.NameToLayer("ZoneCollider"));

        if (Physics.Raycast(ray, out hit, 200f, ignoreZoneMask))
        {
            return hit.transform.GetComponentInParent<Anomaly>()?.transform;
        }
        return null;
    }


    // ================================================================
    // FINAL EVALUATION
    // ================================================================
    void EvaluateAll(Transform anomaly)
    {
        state = CommsState.ShowResult;

        targetAnomaly = anomaly;

        Anomaly a = anomaly.GetComponent<Anomaly>();

        correctIsAlive = (a.data.type == AnomalyType.StealthLiving);
        correctZoneTag = a.zoneTag;

        string correctZone = NormalizeZone(correctZoneTag);

        bool aliveOK = (playerIsAliveAnswer == correctIsAlive);
        bool zoneOK = (playerZoneAnswer == correctZone);
        bool lightOK = SpotlightHits();

        // ================= DEBUG OUTPUT =================
        Debug.Log("========== FINAL CHECK ==========");

        Debug.Log(
            $"Type  : {(aliveOK ? "Correct" : "Wrong")}   " +
            $"(Player: {(playerIsAliveAnswer ? "Alive" : "Not Alive")} | " +
            $"Target: {(correctIsAlive ? "Alive" : "Not Alive")})"
        );

        Debug.Log(
            $"Zone  : {(zoneOK ? "Correct" : "Wrong")}   " +
            $"(Player: {playerZoneAnswer} | Target: {correctZone})"
        );

        Debug.Log(
            $"Light : {(lightOK ? "Detected" : "Failed")}"
        );

        Debug.Log("=================================");

        StartCoroutine(ShowFinalReport(aliveOK, zoneOK, lightOK));
    }


    bool SpotlightHits_Debug(Transform expectedAnomaly)
    {
        Ray ray = new Ray(spotlight.transform.position, spotlight.transform.forward);
        RaycastHit hit;

        int ignoreZoneMask = ~(1 << LayerMask.NameToLayer("ZoneCollider"));

        Debug.DrawRay(ray.origin, ray.direction * 300f, Color.cyan, 1.5f);

        if (Physics.Raycast(ray, out hit, 300f, ignoreZoneMask))
        {
            Debug.Log($"[LIGHT RAY] Hit: {hit.transform.name}");

            var hitAnomaly = hit.transform.GetComponentInParent<Anomaly>();
            if (hitAnomaly == null)
            {
                Debug.Log("[LIGHT RAY] ❌ Hit object has NO Anomaly in parent");
                return false;
            }

            Debug.Log($"[LIGHT RAY] ✅ Hit Anomaly: {hitAnomaly.name}");
            Debug.Log($"[LIGHT RAY] Expected Anomaly: {expectedAnomaly.name}");

            return hitAnomaly.transform == expectedAnomaly;
        }

        Debug.Log("[LIGHT RAY] ❌ Raycast hit NOTHING");
        return false;
    }



    bool SpotlightHits()
    {
        Ray ray = new Ray(spotlight.transform.position, spotlight.transform.forward);
        RaycastHit hit;

        int ignoreZoneMask = ~(1 << LayerMask.NameToLayer("ZoneCollider"));

        if (Physics.Raycast(ray, out hit, 300f, ignoreZoneMask))
        {
            return hit.transform.GetComponentInParent<Anomaly>() != null;
        }

        return false;
    }


    // ================================================================
    // FINAL REPORT — 包含 … 闪烁 与 隐藏检查
    // ================================================================
    IEnumerator ShowFinalReport(bool aliveOK, bool zoneOK, bool lightOK)
    {
        // STEP 1: 显示省略号闪烁
        ui.ForceDots("...");
        bool blinking = true;
        StartCoroutine(ui.DotBlink(() => blinking));

        // STEP 2: 隐藏系统提示，后台执行检查（不可见）
        ui.SetSystemAlpha(0f);

        yield return StartCoroutine(ui.TypeSilently($"Alive Check: {(aliveOK ? "Correct" : "Incorrect")}"));
        yield return StartCoroutine(ui.TypeSilently($"Zone Check: {(zoneOK ? "Correct" : "Incorrect")}"));
        yield return StartCoroutine(ui.TypeSilently($"Light Check: {(lightOK ? "Confirmed" : "Failed")}"));

        // STEP 3: 停止省略号闪烁
        blinking = false;
        ui.ForceDots("");
        yield return new WaitForSeconds(0.2f);

        // STEP 4: 显示最终结果
        ui.RestoreSystemAlpha();

        if (aliveOK && zoneOK && lightOK)
        {
            ui.ShowSystemMessage("Containment successful.");

            if (targetAnomaly != null)
                Destroy(targetAnomaly.gameObject);
        }
        else
        {
            ui.ShowSystemMessage("Containment failed.");
        }

        yield return new WaitForSeconds(2f);
        ResetAll();
    }

    // ================================================================
    void ResetAll()
    {
        isCommunicating = false;
        state = CommsState.Idle;

        ui.ClearAll();
        ui.HideAll();
    }

    // ================================================================
    string ConvertKeysToZone(KeyCode a, KeyCode b)
    {
        string combo = a.ToString() + b.ToString();
        return combo switch
        {
            "RB" => "A",
            "RG" => "B",
            "BG" => "C",
            "BR" => "D",
            "GR" => "E",
            "GB" => "F",
            _ => "?"
        };
    }

    string NormalizeZone(string zoneTag)
    {
        // "ZoneA" -> "A"
        if (zoneTag.StartsWith("Zone"))
            return zoneTag.Replace("Zone", "");

        return zoneTag;
    }

    // ================================================================
    // Voice API
    // ================================================================
    public void ReceiveAliveAnswerFromVoice(bool isAlive, string displayText)
    {
        if (!isCommunicating || state != CommsState.AskAlive)
            return;

        playerIsAliveAnswer = isAlive;

        ui.ShowPlayerMessage(displayText, () =>
        {
            StartCoroutine(NextAfterAlive());
        });
    }

    public void ReceiveZoneAnswerFromVoice(string zoneLetter)
    {
        if (!isCommunicating || state != CommsState.AskZone)
            return;

        string upper = zoneLetter.ToUpper();

        if ("ABCDEF".IndexOf(upper) < 0)
            return;

        playerZoneAnswer = upper;

        ui.ShowPlayerMessage("Reporting Zone: " + upper, () =>
        {
            StartCoroutine(NextAfterZone());
        });
    }
}
