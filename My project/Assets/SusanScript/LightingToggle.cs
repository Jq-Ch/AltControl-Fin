using UnityEngine;

public class LightingToggle : MonoBehaviour
{
    [Header("Spotlight to toggle")]
    public Light spotlight;  
    private bool isOn = false;

    void Start()
    {
        // 初始化关闭灯
        spotlight.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isOn = !isOn;
            spotlight.enabled = isOn;
        }
    }
}
