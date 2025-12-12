using UnityEngine;
using UnityEngine.UI;

public class BatterySystem : MonoBehaviour
{
    [Header("Battery Settings")]
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float drainPerSecond = 5f;

    [Header("Recharge Settings")]
    public float rechargeAmount = 10f; 
    [Header("UI")]
    public Image batteryFill;

    [Header("Spotlight")]
    public Light spotlight;

    private bool lightOn = false;
    private int sequenceStep = 0;   // 0=等待J, 1=等待K, 2=等待L

    void Start()
    {
        currentBattery = maxBattery;
        spotlight.enabled = false;
    }

    void Update()
    {
        // 组合按键检测：J → K → L

        CheckRechargeSequence();

        // 按1 开关灯
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleLight();
        }

        // 开灯 → 电量下降
        if (lightOn)
        {
            currentBattery -= drainPerSecond * Time.deltaTime;

            if (currentBattery <= 0)
            {
                currentBattery = 0;
                lightOn = false;
                spotlight.enabled = false;
            }
        }

        batteryFill.fillAmount = currentBattery / maxBattery;
    }

    // J → K → L 连续按下恢复电量
    void CheckRechargeSequence()
    {
        if (sequenceStep == 0 && Input.GetKeyDown(KeyCode.J))
        {
            sequenceStep = 1; // 进入下一步（等待 K）
        }
        else if (sequenceStep == 1 && Input.GetKeyDown(KeyCode.K))
        {
            sequenceStep = 2; // 进入下一步（等待 L）
        }
        else if (sequenceStep == 2 && Input.GetKeyDown(KeyCode.L))
        {
            //输入 J-K-L
            currentBattery = Mathf.Min(maxBattery, currentBattery + rechargeAmount);

            // 重置输入步骤
            sequenceStep = 0;
        }
        else
        {
            //输入错误 → 重置
            if (Input.anyKeyDown &&
                !Input.GetKeyDown(KeyCode.J) &&
                !Input.GetKeyDown(KeyCode.K) &&
                !Input.GetKeyDown(KeyCode.L))
            {
                sequenceStep = 0;
            }
        }
    }


    // 灯光开关
    void ToggleLight()
    {
        if (currentBattery <= 0)
            return;

        lightOn = !lightOn;
        spotlight.enabled = lightOn;
    }
}
