using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PooledObject
{
    public GameObject prefab;
    public float weight;
    public int poolSize;
}

public class ObjectPoolManager : MonoBehaviour
{
    [Header("Prefab | Weight | Pool Size")]
    public List<PooledObject> objectsToPool;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    private float totalWeight;

    [Header("Floating Text Settings")]
    public GameObject floatingText; // Assign prefab in Inspector
    [SerializeField] private int textPoolSize = 2; // Number of reusable text objects
    private Queue<GameObject> textPool = new Queue<GameObject>();
    public Canvas myCanvas;

    [Header("Destroy Effect Settings")]
    public GameObject destroyEffect;
    [SerializeField] private int effectPoolSize = 5;
    private Queue<GameObject> effectPool = new Queue<GameObject>();

    private void Awake()
    {
        InitializeObjectPools();
        InitializeTextPool();
        InitializeEffectPool();
    }

    private void InitializeObjectPools()
    {
        // �� PooledObject�� ���� Ǯ�� �ʱ�ȭ
        foreach (var item in objectsToPool)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < item.poolSize; i++)
            {
                GameObject obj = Instantiate(item.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary[item.prefab] = objectPool;
            totalWeight += item.weight;
        }
    }

    private void InitializeTextPool()
    {
        if (floatingText == null || myCanvas == null)
        {
            Debug.LogError("FloatingText prefab �Ǵ� Canvas�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }
        for (int i = 0; i < textPoolSize; i++)
        {
            // myCanvas�� �θ�� �����Ͽ� �ν��Ͻ� ����
            GameObject textObj = Instantiate(floatingText, myCanvas.transform);
            textObj.SetActive(false);
            textPool.Enqueue(textObj);
        }
    }

    private void InitializeEffectPool()
    {
        if (destroyEffect == null)
        {
            Debug.LogError("DestroyEffect prefab�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }
        for (int i = 0; i < effectPoolSize; i++)
        {
            GameObject effectObj = Instantiate(destroyEffect, transform);
            effectObj.SetActive(false);
            effectPool.Enqueue(effectObj);
        }
    }

    // ����ġ�� ���� �������� ������Ʈ�� �����ɴϴ�.
    public GameObject GetPooledObject()
    {
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var item in objectsToPool)
        {
            cumulativeWeight += item.weight;
            if (randomValue < cumulativeWeight)
            {
                if (poolDictionary.TryGetValue(item.prefab, out Queue<GameObject> poolQueue))
                {
                    while (poolQueue.Count > 0)
                    {
                        GameObject obj = poolQueue.Dequeue();
                        if (!obj.activeInHierarchy)
                        {
                            obj.SetActive(true);
                            return obj;
                        }
                    }
                }
                // Ǯ�� ����� ������Ʈ�� ���ٸ� ���� ����
                GameObject newObj = Instantiate(item.prefab, transform);
                newObj.SetActive(true);
                return newObj;
            }
        }
        return null;
    }

    // Ư�� prefab�� �������� ������Ʈ�� �����ɴϴ�.
    public GameObject GetPooledObject(GameObject prefab)
    {
        if (poolDictionary.TryGetValue(prefab, out Queue<GameObject> poolQueue))
        {
            while (poolQueue.Count > 0)
            {
                GameObject pooledObj = poolQueue.Dequeue();
                if (!pooledObj.activeInHierarchy)
                {
                    pooledObj.SetActive(true);
                    return pooledObj;
                }
            }
        }
        GameObject newObj = Instantiate(prefab, transform);
        newObj.SetActive(true);
        return newObj;
    }

    // ������Ʈ�� �ش� Ǯ�� ��ȯ�մϴ�.
    public void ReturnToPool(GameObject obj)
    {
        // �̸� �� ���, �ʿ� �� Ŀ���� ������Ʈ�� ����Ͽ� �� ȿ�������� ��ȯ�� �� �ֽ��ϴ�.
        foreach (var item in objectsToPool)
        {
            if (obj.name.Contains(item.prefab.name))
            {
                obj.SetActive(false);
                poolDictionary[item.prefab].Enqueue(obj);
                return;
            }
        }
        // �ش��ϴ� Ǯ�� ������ ������Ʈ�� �ı��մϴ�.
        Destroy(obj);
    }

    // Floating Text Ǯ ���� �޼���
    public GameObject GetFloatingText()
    {
        if (textPool.Count > 0)
        {
            GameObject textObj = textPool.Dequeue();
            textObj.SetActive(true);
            return textObj;
        }
        else
        {
            // Ǯ�� ���� ��� �� �ν��Ͻ��� ����
            GameObject newText = Instantiate(floatingText, myCanvas.transform);
            return newText;
        }
    }

    public void ReturnTextToPool(GameObject textObj)
    {
        textObj.SetActive(false);
        textPool.Enqueue(textObj);
    }

    // Destroy Effect Ǯ ���� �޼���
    public GameObject GetDestroyEffect()
    {
        if (effectPool.Count > 0)
        {
            GameObject effectObj = effectPool.Dequeue();
            effectObj.SetActive(true);
            return effectObj;
        }
        else
        {
            // Ǯ�� ���� ��� �� �ν��Ͻ��� ����
            GameObject newEffect = Instantiate(destroyEffect, transform);
            return newEffect;
        }
    }

    public void ReturnEffectToPool(GameObject effectObj)
    {
        effectObj.SetActive(false);
        effectPool.Enqueue(effectObj);
    }

    // ������ ��ġ���� �ı� ����Ʈ�� ������ ��, ���� �ð� �Ŀ� Ǯ�� ��ȯ�ϴ� Coroutine
    public IEnumerator GenerateDestroyEffect(Vector3 position)
    {
        GameObject effectObject = GetDestroyEffect();
        effectObject.transform.position = position;
        yield return new WaitForSeconds(0.5f);
        ReturnEffectToPool(effectObject);
    }
}
