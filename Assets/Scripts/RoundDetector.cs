using System.Collections;
using UnityEngine;

public class RoundDetector : MonoBehaviour
{
    private GameManager gameManager;
    private InGameUIManager gameUIManager;
    private CameraPosition cameraPosition;
    private bool isStart = false;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        gameUIManager = FindObjectOfType<InGameUIManager>();
        cameraPosition = FindObjectOfType<CameraPosition>();

        if (gameManager == null)
            Debug.LogError("GameManager를 찾을 수 없습니다!");
        if (gameUIManager == null)
            Debug.LogError("InGameUIManager를 찾을 수 없습니다!");
        if (cameraPosition == null)
            Debug.LogError("CameraPosition을 찾을 수 없습니다!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 또는 무적 플레이어와 충돌 시
        if (collision.CompareTag("Player"))
        {
            if (isStart)
            {
                gameManager.IncreaseSpeed();
                // speedMultiplier가 1 초과라면 라운드 증가 및 UI, 카메라 색상 변경
                if (gameManager.speedMultiplier > 1)
                {
                    gameManager.currentRound++;
                    StartCoroutine(gameUIManager.MultiplierStatUp());
                    cameraPosition.TriggerColorChange();
                    AudioManager.Instance.PlaySFX(SFXType.RoundUp);
                }
            }
            else
            {
                isStart = true;
            }
        }
    }
}
