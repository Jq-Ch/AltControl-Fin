using UnityEngine;

public class LightingToggle : MonoBehaviour
{
    public Light spotlight;
    public BatterySystem batterySystem;

    private bool isOn = false;

    void Start()
    {
        spotlight.enabled = false;
    }

    void Update()
    {
        //电量 = 0 → 完全禁用 ToggleLight() 的功能
        if (batterySystem.currentBattery <= 0)
        {
            isOn = false;             // 强制内部状态为关闭
            spotlight.enabled = false; // 强制灯为关闭
            return;                   // 玩家按 1 时不会触发任何逻辑
        }

        // 电量 > 0 → 可以正常按 1 开 / 关灯
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isOn = !isOn;
            spotlight.enabled = isOn;
        }
    }
}
