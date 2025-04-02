using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectableItem
{
    public string itemName;      // ���п� �̸� (���� ����)
    public GameObject prefab;    // ������ ������
    public float weight;         // ��ȯ weight
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
    private List<GameObject> spawnedObjects = new List<GameObject>(); // ������ collectable ������Ʈ�� ����

    // ���� ������ (��: ���, ����)�� �߰� ��ų �����۵��� ��� �����ϴ� ����Ʈ
    public List<CollectableItem> collectableItems;

    private void Awake()
    {
        // �߽�(0,0,0)���� ���� ��ġ������ ����� ������ ����� �ܺη� �ٶ󺸵��� ȸ��
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
            // ������ ��� collectable ������Ʈ�� Ǯ�� ��ȯ
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

            // ���� ������ ������Ʈ�� Obstacle �±׶�� �߰��� collectable ������Ʈ ����
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

        // ��ü weight �հ� ���
        float totalWeight = 0f;
        foreach (var item in collectableItems)
        {
            totalWeight += item.weight;
        }

        // �� spawnPoint���� weight�� ���� ���� �������� ������ ����
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
