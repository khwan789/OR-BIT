using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InGameUIManager : MonoBehaviour
{
    private GameManager gameManager;

    // ���� �÷��� UI
    public GameObject gamePlayPopup;
    public TextMeshProUGUI currentScore_Text;
    public TextMeshProUGUI highScore_Text;
    public TextMeshProUGUI gold_Text;
    public TextMeshProUGUI roundUpText;
    public TextMeshProUGUI multiplierText;

    // ���� ���� UI
    public GameObject gameOverPopup;
    public TextMeshProUGUI bestScoreOver_Text;
    public TextMeshProUGUI currentScoreOver_Text;
    public TextMeshProUGUI roundOver_Text;

    private int golds;
    private int highScore;
    private bool isHighScore = false;
    private bool isGameOver = false;

    // UI ������Ʈ �ֱ⸦ ���� (��: 0.1�ʸ��� ������Ʈ)
    private float updateInterval = 0.1f;
    private float updateTimer = 0f;

    // These are references to the UI buttons' RectTransforms
    public UnityEngine.UI.Button jumpButton;
    public UnityEngine.UI.Button slideButton;

    private RectTransform jumpButtonRect;
    private RectTransform slideButtonRect;

    // Optionally, define the default positions (set in the Inspector)
    public Vector2 defaultJumpPos;
    public Vector2 defaultSlidePos;

    // The swapped positions can simply be the reverse of the defaults.
    private Vector2 swappedJumpPos;
    private Vector2 swappedSlidePos;

    private void Start()
    {
        gameManager = GameManager.Instance;
        golds = gameManager.GetTotalGold();
        highScore = gameManager.GetHighScore();
        UpdateTotalGoldText(golds);
        UpdateHighScoreText(highScore);
        UpdateMultiplierStat();
        UpdateButtonPosition();
    }

    private void Update()
    {
        // ���� ���ݸ��� UI ������Ʈ
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateScoreText((int)gameManager.currentScore);
            // ���̽��ھ� ������Ʈ: ���� ������ ���� ���̽��ھ�� ũ�� ������Ʈ
            if (gameManager.currentScore >= gameManager.GetHighScore())
            {
                UpdateHighScoreText((int)gameManager.currentScore);
                if (!isHighScore)
                {
                    StartCoroutine(HighScoreUp());
                    isHighScore = true;
                }
            }
            // ��尡 ���ŵǾ��� ���� ������Ʈ
            int currentGold = gameManager.GetTotalGold();
            if (currentGold > golds)
            {
                golds = currentGold;
                UpdateTotalGoldText(golds);
            }
        }

        // ���� ���� üũ�� �� ������ Ȯ���ص� ū �δ��� �����Ƿ� �״�� ���
        if (!gameManager.isPlaying && !isGameOver)
        {
            isGameOver = true;
            GameOverUI();
        }
    }

    private void UpdateScoreText(int score)
    {
        currentScore_Text.text = score.ToString();
    }

    private void UpdateTotalGoldText(int gold)
    {
        gold_Text.text = gold.ToString();
    }

    private void UpdateHighScoreText(int highScore)
    {
        highScore_Text.text = "High Score " + highScore.ToString();
    }
    private void UpdateButtonPosition()
    {
        jumpButtonRect = jumpButton.GetComponent<RectTransform>();
        slideButtonRect = slideButton.GetComponent<RectTransform>();

        // Apply the swap if needed
        if (GameManager.Instance.SwapButtons)
        {
            // Swap anchored positions
            Vector2 tempAnchoredPos = jumpButtonRect.anchoredPosition;
            jumpButtonRect.anchoredPosition = slideButtonRect.anchoredPosition;
            slideButtonRect.anchoredPosition = tempAnchoredPos;

            // Swap anchorMin values
            Vector2 tempAnchorMin = jumpButtonRect.anchorMin;
            jumpButtonRect.anchorMin = slideButtonRect.anchorMin;
            slideButtonRect.anchorMin = tempAnchorMin;

            // Swap anchorMax values
            Vector2 tempAnchorMax = jumpButtonRect.anchorMax;
            jumpButtonRect.anchorMax = slideButtonRect.anchorMax;
            slideButtonRect.anchorMax = tempAnchorMax;

            // Swap pivot values
            Vector2 tempPivot = jumpButtonRect.pivot;
            jumpButtonRect.pivot = slideButtonRect.pivot;
            slideButtonRect.pivot = tempPivot;
        }
    }

    private void GameOverUI()
    {
        gamePlayPopup.SetActive(false);
        gameOverPopup.SetActive(true);
        currentScoreOver_Text.text = ((int)gameManager.currentScore).ToString();
        bestScoreOver_Text.text = gameManager.GetHighScore().ToString();
        roundOver_Text.text = gameManager.currentRound.ToString();
    }

    public void Replay()
    {
        gamePlayPopup.SetActive(true);
        gameOverPopup.SetActive(false);
        gameManager.StartGame();
        SceneManager.LoadScene("Main");
        AudioManager.Instance.PlaySFX(SFXType.Button);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
        AudioManager.Instance.PlaySFX(SFXType.Button);
        AudioManager.Instance.PlayMainMenuBGM();
    }

    public void UpdateMultiplierStat()
    {
        roundUpText.text = gameManager.currentRound + " Round";
        multiplierText.text = "Boost x" + gameManager.speedMultiplier;
    }

    // ��Ʈ ũ��� ���� �ִϸ��̼��� �ڷ�ƾ���� �� ó���Ǿ� �����Ƿ� �ʿ信 ���� Tweening ���̺귯���� ����ϴ� �͵� ����� �� �ֽ��ϴ�.
    public IEnumerator MultiplierStatUp()
    {
        UpdateMultiplierStat();

        float roundSize = roundUpText.fontSize;
        float multiplierSize = multiplierText.fontSize;

        float startRoundSize = roundSize * 1.5f;
        float startMultiplierSize = multiplierSize * 1.5f;

        roundUpText.fontSize = startRoundSize;
        multiplierText.fontSize = startMultiplierSize;
        roundUpText.color = Color.yellow;
        multiplierText.color = Color.yellow;

        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            roundUpText.fontSize = Mathf.Lerp(startRoundSize, roundSize, t);
            multiplierText.fontSize = Mathf.Lerp(startMultiplierSize, multiplierSize, t);
            roundUpText.color = Color.Lerp(Color.yellow, Color.white, t);
            multiplierText.color = Color.Lerp(Color.yellow, Color.white, t);

            yield return null;
        }

        roundUpText.fontSize = roundSize;
        multiplierText.fontSize = multiplierSize;
        roundUpText.color = Color.white;
        multiplierText.color = Color.white;
    }

    public IEnumerator HighScoreUp()
    {
        float currentSize = currentScore_Text.fontSize;
        float highSize = highScore_Text.fontSize;

        float startCurrentSize = currentSize * 2f;
        float startHighSize = highSize * 2f;

        currentScore_Text.fontSize = startCurrentSize;
        highScore_Text.fontSize = startHighSize;
        currentScore_Text.color = Color.yellow;
        highScore_Text.color = Color.yellow;

        float duration = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            currentScore_Text.fontSize = Mathf.Lerp(startCurrentSize, currentSize, t);
            highScore_Text.fontSize = Mathf.Lerp(startHighSize, highSize, t);
            currentScore_Text.color = Color.Lerp(Color.yellow, Color.white, t);
            highScore_Text.color = Color.Lerp(Color.yellow, Color.white, t);

            yield return null;
        }

        currentScore_Text.fontSize = currentSize;
        highScore_Text.fontSize = highSize;
        currentScore_Text.color = Color.white;
        highScore_Text.color = Color.white;
    }
}
