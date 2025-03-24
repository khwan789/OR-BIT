using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private ObjectPoolManager poolManager;
    public TextMeshProUGUI textMesh;
    public float floatSpeed = 1f;
    public float lifetime = 1.5f;

    private Coroutine returnCoroutine;  // Coroutine ������ ������ �ʿ� �� �ߴ� ����

    private void Awake()
    {
        poolManager = FindObjectOfType<ObjectPoolManager>();
    }

    public void SetText(string text, Color textColor)
    {
        textMesh.text = text;
        textMesh.color = textColor;

        // ������ ���� ���� Coroutine�� �ִٸ� �ߴ��ϰ� ���� ����
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
        // Vector3.up�� ����� �� ������ ��ġ ������Ʈ (���ʿ��� ��ü ���� ����)
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    private void ReturnToPool()
    {
        poolManager.ReturnTextToPool(gameObject);
    }
}
