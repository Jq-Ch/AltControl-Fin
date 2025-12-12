using UnityEngine;

public class LightRayDebugger : MonoBehaviour
{
    public float rayDistance = 200f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 origin = transform.position;
            Vector3 dir = transform.forward;

            int ignoreZoneMask = ~(1 << LayerMask.NameToLayer("ZoneCollider"));

            Ray ray = new Ray(origin, dir);
            RaycastHit hit;

            Debug.DrawRay(origin, dir * rayDistance, Color.yellow, 1f);

            if (Physics.Raycast(ray, out hit, rayDistance, ignoreZoneMask))
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
