using System.Collections;
using UnityEngine;

public class InternetConnectionMonitor : MonoBehaviour
{
    // Assign your popup panel (which should be initially disabled) via the Inspector.
    public GameObject noInternetPopupPrefab;

    // Check every 3 second (adjust if needed)
    public float checkInterval = 3f;

    private GameObject noInternetPopupInstance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Start monitoring the connection.
        StartCoroutine(MonitorConnection());
    }

    private IEnumerator MonitorConnection()
    {
        while (true)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                // Each check, find the current canvas in the scene.
                Canvas currentCanvas = FindObjectOfType<Canvas>();
                if (currentCanvas != null && noInternetPopupInstance == null)
                {
                    noInternetPopupInstance = Instantiate(noInternetPopupPrefab, currentCanvas.transform);
                    Debug.Log("No internet: Instantiated popup on canvas: " + currentCanvas.name);
                }
            }
            else
            {
                if (noInternetPopupInstance != null)
                {
                    Destroy(noInternetPopupInstance);
                    noInternetPopupInstance = null;
                    Debug.Log("Internet available: Destroyed popup.");
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
