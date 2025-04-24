using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class Collectable : MonoBehaviour
{
    private ObjectPoolManager objectPoolManager;
    private GameManager gameManager;
    private Camera mainCam;  // 메인 카메라를 캐싱할 변수

    public bool isObstacle;
    public bool isTextNeeded;
    public float scoreAmount = 1;
    public float goldAmount = 1;
    public float slowDown = 0.1f;

    private void Start()
    {
        objectPoolManager = FindObjectOfType<ObjectPoolManager>();
        gameManager = GameManager.Instance;
        mainCam = Camera.main;  // Start에서 Camera.main을 한 번만 캐싱합니다.
    }

    public void ReturnToPool(Vector3 pos)
    {
        HandleFloatingText(pos);
        objectPoolManager.ReturnToPool(gameObject);
    }

    public void ReturnToPool()
    {
        objectPoolManager.ReturnToPool(gameObject);
    }

    private void HandleFloatingText(Vector3 position)
    {
        if (isTextNeeded)
        {
            if (slowDown > 0)
            {
                ShowFloatingTextSpeed("Slow Down", position, Color.white);
            }
            else
            {
                float amount;
                Color textColor;
                if (scoreAmount > 0)
                {
                    textColor = Color.white;
                    amount = GetScoreAmount();
                }
                else
                {
                    textColor = Color.yellow;
                    amount = GetGoldAmount();
                }
                ShowFloatingText(amount, position, textColor);
            }
        }
    }

    void ShowFloatingText(float num, Vector3 position, Color color)
    {
        GameObject floatingText = objectPoolManager.GetFloatingText();
        // 매번 Camera.main을 호출하지 않고 캐싱된 mainCam 사용
        floatingText.transform.position = mainCam.WorldToScreenPoint(position);
        FloatingText ft = floatingText.GetComponent<FloatingText>();
        if (ft != null)
        {
            ft.SetText("+" + num, color);
        }
    }

    void ShowFloatingTextSpeed(string text, Vector3 position, Color color)
    {
        GameObject floatingText = objectPoolManager.GetFloatingText();
        // 매번 Camera.main을 호출하지 않고 캐싱된 mainCam 사용
        floatingText.transform.position = mainCam.WorldToScreenPoint(position);
        FloatingText ft = floatingText.GetComponent<FloatingText>();
        if (ft != null)
        {
            ft.SetText(text, color);
        }
    }

    public float GetScoreAmount() => scoreAmount * gameManager.speedMultiplier;
    public float GetGoldAmount() => goldAmount * gameManager.speedMultiplier;
}
