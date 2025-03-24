using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private ObjectPoolManager poolManager;
    public TextMeshProUGUI textMesh;
    public float floatSpeed = 1f;
    public float lifetime = 1.5f;

    private Coroutine returnCoroutine;  // Coroutine 참조를 보관해 필요 시 중단 가능

    private void Awake()
    {
        poolManager = FindObjectOfType<ObjectPoolManager>();
    }

    public void SetText(string text, Color textColor)
    {
        textMesh.text = text;
        textMesh.color = textColor;

        // 이전에 실행 중인 Coroutine이 있다면 중단하고 새로 시작
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }
        returnCoroutine = StartCoroutine(ReturnToPoolAfterDelay());
    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(lifetime);
        ReturnToPool();
    }

    private void Update()
    {
        // Vector3.up를 사용해 매 프레임 위치 업데이트 (불필요한 객체 생성 줄임)
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    private void ReturnToPool()
    {
        poolManager.ReturnTextToPool(gameObject);
    }
}
