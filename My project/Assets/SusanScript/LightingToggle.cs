using UnityEngine;

public class LightingToggle : MonoBehaviour
{
    public Light spotlight;
    public BatterySystem batterySystem;

    void Start()
    {
        spotlight.enabled = false;
    }

    void Update()
    {
        // 电量耗尽 → 强制关灯
        if (batterySystem.currentBattery <= 0)
        {
            spotlight.enabled = false;
            return;
        }

        // 1 = 开灯
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            spotlight.enabled = true;
        }

        // 2 = 关灯
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            spotlight.enabled = false;
        }
    }
}
