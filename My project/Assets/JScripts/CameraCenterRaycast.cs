using UnityEngine;

public class CameraCenterRaycast : MonoBehaviour
{
    [Header("Ray ����")]
    public float rayLength = 20f;

    [Header("Gizmo ��ɫ")]
    public Color gizmoColor = Color.red;

    private Ray _centerRay;
	private int _maskWithoutZone;

	void Awake()
	{
		int zoneMask = LayerMask.GetMask("ZoneCollider");
		if (zoneMask == 0)
		{
			Debug.LogWarning("Layer 'ZoneCollider' not found; raycasts will not ignore it.");
		}
		_maskWithoutZone = ~zoneMask;
	}

    void Update()
    {
        // ��������ĵ㷢�� Ray����Ļ���ģ�
        _centerRay = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)
        );

        // �������߼��
		if (Physics.Raycast(_centerRay, out RaycastHit hit, rayLength, _maskWithoutZone))
        {
            Debug.Log("Hit: " + hit.collider.name);
        }
    }

    // Scene ��ͼ�л��� Gizmo
    private void OnDrawGizmos()
    {
        if (Camera.main == null) return;

        Gizmos.color = gizmoColor;

        // ����һ���������������ǰ�Ŀ��ӻ���
        Vector3 start = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        Gizmos.DrawLine(start, start + direction * rayLength);
        Gizmos.DrawSphere(start + direction * rayLength, 0.05f);
    }
}
