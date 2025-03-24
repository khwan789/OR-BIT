using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private ObjectPoolManager objectPoolManager;
    private GameManager gameManager;

    private void Awake()
    {
        objectPoolManager = FindObjectOfType<ObjectPoolManager>();
        gameManager = GameManager.Instance;
    }

    public void ReturnToPool()
    {
        if (objectPoolManager != null)
        {
            objectPoolManager.ReturnToPool(gameObject);
        }
    }
}
    