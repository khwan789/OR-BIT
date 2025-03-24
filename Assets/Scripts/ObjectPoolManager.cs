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
        // 각 PooledObject에 대해 풀을 초기화
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
            Debug.LogError("FloatingText prefab 또는 Canvas가 할당되지 않았습니다!");
            return;
        }
        for (int i = 0; i < textPoolSize; i++)
        {
            // myCanvas를 부모로 지정하여 인스턴스 생성
            GameObject textObj = Instantiate(floatingText, myCanvas.transform);
            textObj.SetActive(false);
            textPool.Enqueue(textObj);
        }
    }

    private void InitializeEffectPool()
    {
        if (destroyEffect == null)
        {
            Debug.LogError("DestroyEffect prefab이 할당되지 않았습니다!");
            return;
        }
        for (int i = 0; i < effectPoolSize; i++)
        {
            GameObject effectObj = Instantiate(destroyEffect, transform);
            effectObj.SetActive(false);
            effectPool.Enqueue(effectObj);
        }
    }

    // 가중치에 따라 무작위로 오브젝트를 가져옵니다.
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
                // 풀에 사용할 오브젝트가 없다면 새로 생성
                GameObject newObj = Instantiate(item.prefab, transform);
                newObj.SetActive(true);
                return newObj;
            }
        }
        return null;
    }

    // 특정 prefab을 기준으로 오브젝트를 가져옵니다.
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

    // 오브젝트를 해당 풀로 반환합니다.
    public void ReturnToPool(GameObject obj)
    {
        // 이름 비교 대신, 필요 시 커스텀 컴포넌트를 사용하여 더 효율적으로 반환할 수 있습니다.
        foreach (var item in objectsToPool)
        {
            if (obj.name.Contains(item.prefab.name))
            {
                obj.SetActive(false);
                poolDictionary[item.prefab].Enqueue(obj);
                return;
            }
        }
        // 해당하는 풀이 없으면 오브젝트를 파괴합니다.
        Destroy(obj);
    }

    // Floating Text 풀 관련 메서드
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
            // 풀에 없을 경우 새 인스턴스를 생성
            GameObject newText = Instantiate(floatingText, myCanvas.transform);
            return newText;
        }
    }

    public void ReturnTextToPool(GameObject textObj)
    {
        textObj.SetActive(false);
        textPool.Enqueue(textObj);
    }

    // Destroy Effect 풀 관련 메서드
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
            // 풀에 없을 경우 새 인스턴스를 생성
            GameObject newEffect = Instantiate(destroyEffect, transform);
            return newEffect;
        }
    }

    public void ReturnEffectToPool(GameObject effectObj)
    {
        effectObj.SetActive(false);
        effectPool.Enqueue(effectObj);
    }

    // 지정된 위치에서 파괴 이펙트를 생성한 후, 일정 시간 후에 풀로 반환하는 Coroutine
    public IEnumerator GenerateDestroyEffect(Vector3 position)
    {
        GameObject effectObject = GetDestroyEffect();
        effectObject.transform.position = position;
        yield return new WaitForSeconds(0.5f);
        ReturnEffectToPool(effectObject);
    }
}
