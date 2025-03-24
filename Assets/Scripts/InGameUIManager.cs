using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InGameUIManager : MonoBehaviour
{
    private GameManager gameManager;

    // 게임 플레이 UI
    public GameObject gamePlayPopup;
    public TextMeshProUGUI currentScore_Text;
    public TextMeshProUGUI highScore_Text;
    public TextMeshProUGUI gold_Text;
    public TextMeshProUGUI roundUpText;
    public TextMeshProUGUI multiplierText;

    // 게임 오버 UI
    public GameObject gameOverPopup;
    public TextMeshProUGUI bestScoreOver_Text;
    public TextMeshProUGUI currentScoreOver_Text;
    public TextMeshProUGUI roundOver_Text;

    private int golds;
    private int highScore;
    private bool isHighScore = false;
    private bool isGameOver = false;

    // UI 업데이트 주기를 조절 (예: 0.1초마다 업데이트)
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
        // 일정 간격마다 UI 업데이트
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateScoreText((int)gameManager.currentScore);
            // 하이스코어 업데이트: 현재 점수가 기존 하이스코어보다 크면 업데이트
            if (gameManager.currentScore >= gameManager.GetHighScore())
            {
                UpdateHighScoreText((int)gameManager.currentScore);
                if (!isHighScore)
                {
                    StartCoroutine(HighScoreUp());
                    isHighScore = true;
                }
            }
            // 골드가 갱신되었을 때만 업데이트
            int currentGold = gameManager.GetTotalGold();
            if (currentGold > golds)
            {
                golds = currentGold;
                UpdateTotalGoldText(golds);
            }
        }

        // 게임 오버 체크는 매 프레임 확인해도 큰 부담은 없으므로 그대로 사용
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

    // 폰트 크기와 색상 애니메이션은 코루틴으로 잘 처리되어 있으므로 필요에 따라 Tweening 라이브러리를 사용하는 것도 고려할 수 있습니다.
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
