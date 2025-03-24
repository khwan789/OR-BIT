using System.Collections;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    private GameManager gameManager;
    private ObjectPlayer player; // Reference to the player
    public Transform planet; // Reference to the planet
    public float baseFollowDistance = 5f; // Default follow distance
    public float smoothSpeed = 0.125f; // Smoothing factor
    private bool isFollowing = false; // Determines if the camera is following the player
    private Vector3 playerPosition;

    public Color[] bgColors = new Color[6];
    private Camera myCam;
    private int currentColorIndex = 0;
    private bool isTransitioning;

    private void Start()
    {
        gameManager = GameManager.Instance;
        player = FindObjectOfType<ObjectPlayer>();
        myCam = GetComponent<Camera>();
    }

    public void AdjustCameraParameters(float multiplier)
    {
        baseFollowDistance *= multiplier;
        smoothSpeed *= multiplier;
    }

    public void CenterOnPlayer()
    {
        isFollowing = true; // Temporarily stop regular camera follow
    }

    void LateUpdate()
    {
        playerPosition = player.basePosition;

        if (gameManager.isPlaying)
        {
            // Regular follow logic
            Vector3 playerToPlanetDirection = (playerPosition - planet.position).normalized;
            Vector3 tangentialDirection = Vector3.Cross(playerToPlanetDirection, Vector3.forward).normalized;

            Vector3 targetPosition = playerPosition + tangentialDirection * baseFollowDistance * player.direction;
            targetPosition.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * gameManager.speedMultiplier * Time.deltaTime);
        }

        //CameraSize();
    }

    void CameraSize()
    {
        var size = myCam.orthographicSize;
        var targetSize = 10 * gameManager.speedMultiplier;
        if (size < targetSize)
        {
            this.GetComponent<Camera>().orthographicSize = Mathf.Lerp(size, targetSize, 10 * Time.deltaTime);
        }
    }

    public void TriggerColorChange()
    {
        if (!isTransitioning)
        {
            StartCoroutine(ChangeColor());
        }
    }

    IEnumerator ChangeColor()
    {
        isTransitioning = true;

        Color startColor = bgColors[currentColorIndex];
        currentColorIndex = (currentColorIndex + 1) % bgColors.Length; // Loop back after the last color
        Color targetColor = bgColors[currentColorIndex];

        float duration = 1f; // 1-second transition
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            myCam.backgroundColor = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        myCam.backgroundColor = targetColor; // Ensure final color is set
        isTransitioning = false;
    }
}
