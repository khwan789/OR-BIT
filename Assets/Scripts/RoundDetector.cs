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
            Debug.LogError("GameManager�� ã�� �� �����ϴ�!");
        if (gameUIManager == null)
            Debug.LogError("InGameUIManager�� ã�� �� �����ϴ�!");
        if (cameraPosition == null)
            Debug.LogError("CameraPosition�� ã�� �� �����ϴ�!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾� �Ǵ� ���� �÷��̾�� �浹 ��
        if (collision.CompareTag("Player"))
        {
            if (isStart)
            {
                gameManager.IncreaseSpeed();
                // speedMultiplier�� 1 �ʰ���� ���� ���� �� UI, ī�޶� ���� ����
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
