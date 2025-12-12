using UnityEngine;

public class LightRayDebugger : MonoBehaviour
{
    public float rayDistance = 200f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))   // 鼠标左键
        {
            Vector3 origin = transform.position;
            Vector3 dir = transform.forward;

            Ray ray = new Ray(origin, dir);
            RaycastHit hit;

            Debug.DrawRay(origin, dir * rayDistance, Color.yellow, 1f);

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                Debug.Log("🔥 Light Ray Hit: " + hit.transform.name);
            }
            else
            {
                Debug.Log("⚠ Light Ray Hit NOTHING");
            }
        }
    }
}
