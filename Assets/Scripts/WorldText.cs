using UnityEngine;
using TMPro;

public class WorldText : MonoBehaviour
{
    public Transform worldTarget;      // �ؽ�Ʈ�� ����ٴ� ���� ������Ʈ
    public RectTransform uiText;         // ĵ���� ���� UI �ؽ�Ʈ
    public Camera mainCamera;            // ���� ī�޶�

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (worldTarget == null)
            Debug.LogError("WorldTarget�� �Ҵ���� �ʾҽ��ϴ�!");
        if (uiText == null)
            Debug.LogError("UI Text RectTransform�� �Ҵ���� �ʾҽ��ϴ�!");
    }

    private void Update()
    {
        if (worldTarget != null && uiText != null && mainCamera != null)
        {
            // worldTarget ���� (��: 5 ����) ��ġ�� ��ũ�� ��ǥ�� ��ȯ
            Vector3 worldPos = worldTarget.position + Vector3.up * 5f;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPos);
            uiText.position = screenPosition;
        }
    }
}
