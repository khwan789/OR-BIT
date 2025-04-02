using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectableItem
{
    public string itemName;      // 구분용 이름 (선택 사항)
    public GameObject prefab;    // 생성할 프리팹
    public float weight;         // 소환 weight
}

public class WeightedObjectSpawner : MonoBehaviour
{
    private ObjectPoolManager poolManager;
    private Transform player;
    public float spawnDistance = 10f;
    public bool isFirstObstacle = false;
    public bool isTutorialObstacle = false;
    public GameObject tutorialObject;

    private GameObject currentObject;
    private bool playerInRange = false;
    private List<GameObject> spawnedObjects = new List<GameObject>(); // 생성된 collectable 오브젝트들 저장

    // 기존 아이템 (예: 골드, 점수)와 추가 스킬 아이템들을 모두 포함하는 리스트
    public List<CollectableItem> collectableItems;

    private void Awake()
    {
        // 중심(0,0,0)에서 현재 위치까지의 방향과 각도를 계산해 외부로 바라보도록 회전
        Vector3 direction = transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void Start()
    {
        poolManager = GameObject.Find("ObjectPoolManager").GetComponent<ObjectPoolManager>();

        // Instead of GameObject.FindWithTag("Player"), use PlayerManager.
        if (PlayerManager.Instance != null)
            player = PlayerManager.Instance.transform;
        else
            Debug.LogWarning("Player not found at start. Waiting for player instantiation...");

        // Optionally, start a coroutine that waits for the player to become available.

        if (player == null)
            StartCoroutine(WaitForPlayerAndSpawn());

        if (!GameManager.Instance.isTutorialCleared)
        {
            if (!isFirstObstacle && !isTutorialObstacle)
                SpawnRandomObject();
            else if (!isFirstObstacle && isTutorialObstacle)
                SpawnTutorialObject();
        }
        else
        {
            if (!isFirstObstacle)
                SpawnRandomObject();
        }
    }
    private System.Collections.IEnumerator WaitForPlayerAndSpawn()
    {
        while (PlayerManager.Instance == null)
        {
            yield return null;
        }
        player = PlayerManager.Instance.transform;
    }

    private void Update()
    {
        if (player == null) return;

        bool isInRange = IsPlayerWithinDistance();

        if (isInRange && !playerInRange)
        {
            playerInRange = true;
        }
        else if (!isInRange && playerInRange)
        {
            playerInRange = false;
            if (currentObject != null)
            {
                poolManager.ReturnToPool(currentObject);
                currentObject = null;
            }
            // 생성된 모든 collectable 오브젝트를 풀로 반환
            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null)
                    poolManager.ReturnToPool(obj);
            }
            spawnedObjects.Clear();
            SpawnRandomObject();
        }
    }

    private bool IsPlayerWithinDistance()
    {
        if (player == null)
            return false;
        return Vector3.Distance(transform.position, player.position) <= spawnDistance;
    }

    private void SpawnRandomObject()
    {
        if (currentObject == null)
            currentObject = poolManager.GetPooledObject();

        if (currentObject != null)
        {
            currentObject.transform.position = transform.position;
            currentObject.transform.rotation = transform.rotation;

            // 만약 생성된 오브젝트가 Obstacle 태그라면 추가로 collectable 오브젝트 생성
            if (currentObject.CompareTag("Obstacle"))
            {
                SpawnCollectableObjects(currentObject);
            }
        }
    }

    private void SpawnTutorialObject()
    {
        if (currentObject == null)
            currentObject = poolManager.GetPooledObject(tutorialObject);

        if (currentObject != null)
        {
            currentObject.transform.position = transform.position;
            currentObject.transform.rotation = transform.rotation;
        }
    }

    private void SpawnCollectableObjects(GameObject obstacle)
    {
        Transform[] spawnPoints = obstacle.GetComponentsInChildren<Transform>();

        // 전체 weight 합계 계산
        float totalWeight = 0f;
        foreach (var item in collectableItems)
        {
            totalWeight += item.weight;
        }

        // 각 spawnPoint에서 weight에 따른 랜덤 선택으로 아이템 생성
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.CompareTag("PointSpawner"))
            {
                float randomWeight = Random.Range(0f, totalWeight);
                float cumulative = 0f;

                foreach (var item in collectableItems)
                {
                    cumulative += item.weight;
                    if (randomWeight <= cumulative)
                    {
                        SpawnItemAtPoint(item.prefab, spawnPoint.position);
                        break;
                    }
                }
            }
        }
    }

    private void SpawnItemAtPoint(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogWarning("SpawnItemAtPoint: Prefab is null");
            return;
        }
        GameObject item = poolManager.GetPooledObject(prefab);
        if (item != null)
        {
            item.transform.position = position;
            spawnedObjects.Add(item);
        }
    }
}
