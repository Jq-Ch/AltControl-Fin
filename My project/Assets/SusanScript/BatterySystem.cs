using UnityEngine;
using UnityEngine.UI;

public class BatterySystem : MonoBehaviour
{
    [Header("Battery Settings")]
    public float maxBattery = 100f;         // 最大电量
    public float currentBattery = 100f;     // 当前电量
    public float drainPerSecond = 5f;       // 每秒消耗多少电

    [Header("UI")]
    public Image batteryFill;               // 拖入 BatteryBarFill

    [Header("Spotlight")]
    public Light spotlight;                 // 拖入你的 SpotlightLight

    private bool lightOn = false;

    void Start()
    {
        currentBattery = maxBattery;
        spotlight.enabled = false;
    }

    void Update()
    {
        // 按1 开关灯
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleLight();
        }

        // 灯打开 → 每秒消耗电量
        if (lightOn)
        {
            currentBattery -= drainPerSecond * Time.deltaTime;

            // 电量耗尽 → 强制关灯
            if (currentBattery <= 0)
            {
                currentBattery = 0;
                lightOn = false;
                spotlight.enabled = false;
            }
        }

        // 更新 UI
        batteryFill.fillAmount = currentBattery / maxBattery;
    }

    void ToggleLight()
    {
        // 如果没电 → 不准开灯
        if (currentBattery <= 0)
            return;

        lightOn = !lightOn;
        spotlight.enabled = lightOn;
    }
}
