using UnityEngine;
using TMPro;

public class WorldText : MonoBehaviour
{
    public Transform worldTarget;      // 텍스트가 따라다닐 월드 오브젝트
    public RectTransform uiText;         // 캔버스 상의 UI 텍스트
    public Camera mainCamera;            // 메인 카메라

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (worldTarget == null)
            Debug.LogError("WorldTarget이 할당되지 않았습니다!");
        if (uiText == null)
            Debug.LogError("UI Text RectTransform이 할당되지 않았습니다!");
    }

    private void Update()
    {
        if (worldTarget != null && uiText != null && mainCamera != null)
        {
            // worldTarget 위쪽 (예: 5 유닛) 위치를 스크린 좌표로 변환
            Vector3 worldPos = worldTarget.position + Vector3.up * 5f;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPos);
            uiText.position = screenPosition;
        }
    }
}
