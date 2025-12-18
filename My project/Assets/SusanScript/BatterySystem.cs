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

    private int sequenceStep = 0; // 0=等待J, 1=等待K, 2=等待L

    void Start()
    {
        currentBattery = maxBattery;
        if (spotlight != null) spotlight.enabled = false;
        UpdateUI();
    }

    void Update()
    {
        CheckRechargeSequence();

        // 灯开着 → 扣电（灯的真实状态只看 spotlight.enabled）
        if (spotlight != null && spotlight.enabled)
        {
            currentBattery -= drainPerSecond * Time.deltaTime;

            if (currentBattery <= 0f)
            {
                currentBattery = 0f;
                spotlight.enabled = false; // 强制断电
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (batteryFill != null)
            batteryFill.fillAmount = Mathf.Clamp01(currentBattery / maxBattery);
    }

    // J → K → L 连续按下恢复电量
    // 按一次 J 恢复电量（Makey Makey 友好版）
    void CheckRechargeSequence()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            currentBattery = Mathf.Min(maxBattery, currentBattery + rechargeAmount);
        }
    }

}
