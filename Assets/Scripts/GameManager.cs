using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance;

    public bool isPlaying = false;

    public float currentScore = 0f;
    private float highScore = 0f;
    private int totalGold = 0;
    public int currentRound = 0;
    private int totalRound = 0;
    public int totalGamePlay = 0;
    public bool isTutorialCleared = false;
    // Speed multiplier: ���� �� �ӵ� ������ ���
    public float speedMultiplier = 1f;

    // PlayerPrefs Ű�� ����� ����
    private const string HighScoreKey = "HighScore";
    private const string TotalGoldKey = "TotalGold";
    private const string TotalRoundKey = "TotalRound";
    private const string TotalGamePlayKey = "TotalGamePlay";
    private const string TutorialClearedKey = "TutorialCleared";

    public AdObserver adObserver;
    public GoogleObserver googleObserver;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        // �̱��� �ʱ�ȭ
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadGame();
    }

    // ���ʿ��� Update() ����

    public void StartGame()
    {
        if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        ResetInGameStats();
        IncreaseGamePlay();
        AudioManager.Instance.PlayGamePlayBGM();
        isPlaying = true;
    }

    public void GameOver()
    {
        isPlaying = false;
        Time.timeScale = 0;
        // ���� ���� �� ���� ����
        if (currentScore > highScore)
        {
            highScore = currentScore;
            googleObserver.SetLeaderboardScore((int)highScore);
        }
        totalRound += currentRound;
        SaveGame();
        CheckAchievementsCleared();
        
        if(currentRound >= 1 && totalGamePlay % 2 == 0)
        {
            adObserver.ShowAd();
        }
    }

    public void IncreaseScore(float amount)
    {
        currentScore += amount;
        AudioManager.Instance.PlaySFX(SFXType.Collect);
    }

    public void IncreaseSpeed()
    {
        speedMultiplier += 0.1f;
        Debug.Log("speed " + speedMultiplier);
    }

    public void IncreaseGold(float amount)
    {
        totalGold += (int)amount;
        AudioManager.Instance.PlaySFX(SFXType.Collect);
    }

    public void IncreaseGamePlay()
    {
        if(isTutorialCleared)
            totalGamePlay += 1;
    }

    public void ResetInGameStats()
    {
        currentScore = 0f;
        currentRound = 0;
        speedMultiplier = 1f;
    }

    public int GetHighScore() => (int)highScore;
    public int GetTotalGold() => totalGold;
    public int GetTotalRound() => totalRound;
    public int GetTotalGamePlay() => totalGamePlay;

    private void SaveGame()
    {
        PlayerPrefs.SetFloat(HighScoreKey, highScore);
        PlayerPrefs.SetInt(TotalGoldKey, totalGold);
        PlayerPrefs.SetInt(TotalRoundKey, totalRound);
        PlayerPrefs.SetInt(TotalGamePlayKey, totalGamePlay);
        PlayerPrefs.Save();
    }

    private void LoadGame()
    {
        highScore = PlayerPrefs.GetFloat(HighScoreKey, 0f);
        totalGold = PlayerPrefs.GetInt(TotalGoldKey, 0);
        totalRound = PlayerPrefs.GetInt(TotalRoundKey, 0);
        totalGamePlay = PlayerPrefs.GetInt(TotalGamePlayKey, 0);
        if(PlayerPrefs.GetInt(TutorialClearedKey,0) == 1)
        {
            isTutorialCleared = true;
        }
    }

    public void ClearTutorial()
    {
        PlayerPrefs.SetInt(TutorialClearedKey, 1);
        PlayerPrefs.Save();
    }


    public void SetRotation(Transform pos)
    {
        Vector3 downDirection = pos.position - Vector3.zero;
        pos.rotation = Quaternion.FromToRotation(transform.up, downDirection);
    }

    public bool CanSpendGold(int amount) => totalGold >= amount;

    public void SpendGold(int amount)
    {
        if (CanSpendGold(amount))
        {
            totalGold -= amount;
            PlayerPrefs.SetInt(TotalGoldKey, totalGold);
            PlayerPrefs.Save();
        }
    }
    private void CheckAchievementsCleared()
    {
        AchievementManager.Instance.CheckAllAchievements();
    }
    public bool SwapButtons
    {
        get { return PlayerPrefs.GetInt("SwapButtons", 0) == 1; }
        set
        {
            PlayerPrefs.SetInt("SwapButtons", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
