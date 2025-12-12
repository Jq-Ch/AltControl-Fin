using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("键位设置")]
    public KeyCode startKey = KeyCode.Y;   // 开始对话
    public KeyCode keyR = KeyCode.R;
    public KeyCode keyG = KeyCode.G;
    public KeyCode keyB = KeyCode.B;

    [Header("对话文本（单句）")]
    public TextMeshProUGUI text1;      // 文本1
    public TextMeshProUGUI text2;      // 文本2
    public TextMeshProUGUI text4;      // 文本4
    public TextMeshProUGUI text5;      // 文本5（灯光一直开时）

    [Header("随机文本列表（灯光关闭时从这里随机抽一条）")]
    public List<TextMeshProUGUI> randomTexts = new List<TextMeshProUGUI>();

    [Header("灯光检测")]
    public Light lightToCheck;         // 要检测的灯光
    public float delayBetween1And2 = 4f;   // 文本1到文本2的间隔
    public float lightCheckDuration = 5f;  // 灯光检测时长（秒）

    private bool dialogueRunning = false;

    private void Start()
    {
        HideAllTexts();
    }

    private void Update()
    {
        // 开始对话
        if (!dialogueRunning && Input.GetKeyDown(startKey))
        {
            StartCoroutine(DialogueFlow());
        }
    }

    private IEnumerator DialogueFlow()
    {
        dialogueRunning = true;
        HideAllTexts();

        // 1. 播放文本1
        ShowText(text1);

        // 等4秒
        yield return new WaitForSeconds(delayBetween1And2);

        // 2. 播放文本2（这里默认保留文本1一起显示，如果不需要可以先 HideAllTexts 再显示）
        ShowText(text2);

        // 3. 等待玩家输入两次（R / G / B 任意）
        int inputCount = 0;
        while (inputCount < 2)
        {
            if (Input.GetKeyDown(keyR) || Input.GetKeyDown(keyG) || Input.GetKeyDown(keyB))
            {
                inputCount++;
            }
            yield return null;
        }

        // 清掉之前所有对话，只显示文本4（如果你想保留前面的就把 HideAllTexts() 注释掉）
        HideAllTexts();
        ShowText(text4);

        // 4. 同时开始5秒灯光检测
        bool lightTurnedOff = false;
        float timer = 0f;

        // 如果灯一开始就是关的，也视为“有关闭过”
        if (lightToCheck == null || !lightToCheck.enabled)
        {
            lightTurnedOff = true;
        }

        while (timer < lightCheckDuration)
        {
            timer += Time.deltaTime;

            if (lightToCheck != null && !lightToCheck.enabled)
            {
                lightTurnedOff = true;
            }

            yield return null;
        }

        // 检测结束，清掉文本4
        HideAllTexts();

        // 5. 根据灯光状态决定显示文本
        if (!lightTurnedOff)
        {
            // 灯在整个检测期间都保持开启 -> 文本5
            ShowText(text5);
        }
        else
        {
            // 灯在检测期间出现过关闭 -> 随机播放列表中的一条文本
            if (randomTexts != null && randomTexts.Count > 0)
            {
                int index = Random.Range(0, randomTexts.Count);
                ShowText(randomTexts[index]);
            }
            else
            {
                Debug.LogWarning("RandomTexts 列表为空，没有可随机的文本。");
            }
        }

        dialogueRunning = false;
    }

    /// <summary>
    /// 隐藏所有 TextMeshPro 对话对象
    /// </summary>
    private void HideAllTexts()
    {
        if (text1) text1.gameObject.SetActive(false);
        if (text2) text2.gameObject.SetActive(false);
        if (text4) text4.gameObject.SetActive(false);
        if (text5) text5.gameObject.SetActive(false);

        if (randomTexts != null)
        {
            foreach (var t in randomTexts)
            {
                if (t) t.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 显示一个文本
    /// </summary>
    private void ShowText(TextMeshProUGUI tmp)
    {
        if (tmp != null)
        {
            tmp.gameObject.SetActive(true);
        }
    }
}
