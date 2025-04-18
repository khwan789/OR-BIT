using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public Animator animator;
    private bool isInput = false;
    private GameManager gameManager;

    public TextMeshProUGUI highScore_Text;
    public TextMeshProUGUI totalGold_Text;
    public TextMeshProUGUI totalRound_Text;

    public Button characterSelectButton;
    public GameObject redDotNotification;      // UI element for the red dot
    public GameObject characterSelectionPopup;   // The popup panel (with CharacterSelectionPopup script)

    private CharacterSelectionPopup characterSelection;
    
    public GameObject characterSelectionContent;
    public GameObject playerStats;

    private Vector3 playerStatStartPos;
    private Vector3 playerStatTargetPos;
    private float charSelectContentY;
    public GameObject bgmOff;
    public GameObject sfxOff;
    //public GameObject swapOff;

    //menu
    public GameObject menuPopup;

    // These are references to the UI buttons' RectTransforms
    public UnityEngine.UI.Button jumpButtonEx;
    public UnityEngine.UI.Button slideButtonEx;

    private RectTransform jRect, sRect;

    // Default values
    private bool defaultsCaptured = false;
    private Vector2 jPosDef, sPosDef;
    private Vector2 jMinDef, sMinDef;
    private Vector2 jMaxDef, sMaxDef;
    private Vector2 jPivotDef, sPivotDef;

    private void Start()
    {
        Time.timeScale = 1.0f;
        gameManager = GameManager.Instance;
        characterSelection = characterSelectionPopup.GetComponent<CharacterSelectionPopup>();
        LoadStats();
        playerStatStartPos = playerStats.GetComponent<RectTransform>().anchoredPosition;
        charSelectContentY = characterSelectionContent.GetComponent<RectTransform>().rect.height;
        playerStatTargetPos = new Vector3(playerStatStartPos.x, charSelectContentY, 0);
        // Optionally check for any new unlocked character and show red dot.
        CheckForNewUnlock();
        menuUpdate();
        UpdateButtonPositionEx();
    }

    // Update()는 현재 필요 없으므로 제거 가능

    public void StartGame()
    {   
        if (!isInput)
        {
            isInput = true;
            // UI 버튼을 비활성화하거나 애니메이터로 피드백을 주는 것도 고려해보세요.
            StartCoroutine(StartGameRoutine());
        }
        AudioManager.Instance.PlaySFX(SFXType.Button);
    }

    private void LoadStats()
    {
        highScore_Text.text = gameManager.GetHighScore().ToString();
        totalGold_Text.text = gameManager.GetTotalGold().ToString();
        totalRound_Text.text = gameManager.GetTotalRound().ToString();
    }

    private IEnumerator StartGameRoutine()
    {
        animator.enabled = true;
        yield return new WaitForSeconds(1.5f);
        gameManager.StartGame();
        SceneManager.LoadScene("Main");
    }


    public void ToggleCharacterSelection()
    {
        RectTransform statRect = playerStats.GetComponent<RectTransform>();
        if (characterSelectionPopup.activeSelf)
        {
            characterSelection.Hide();
            statRect.anchoredPosition = playerStatStartPos;
        }
        else
        {
            characterSelection.Show();
            // Hide the red dot immediately in the UI...
            redDotNotification.SetActive(false);
            // And clear the PlayerPrefs flag so it doesn't show on next launch.
            PlayerPrefs.SetInt("NewUnlockNotification", 0);
            PlayerPrefs.Save();

            statRect.anchoredPosition = playerStatTargetPos;
        }
        AudioManager.Instance.PlaySFX(SFXType.Button);
    }


    private void CheckForNewUnlock()
    {
        // Get the flag value; default is 0 (no new unlock)
        bool newUnlockAvailable = PlayerPrefs.GetInt("NewUnlockNotification", 0) == 1;
        redDotNotification.SetActive(newUnlockAvailable);
    }


    public void ToggleMenu()
    {
        if (menuPopup.activeSelf)
        {
            menuPopup.SetActive(false);
        }
        else
        {
            menuPopup.SetActive(true);
        }
        AudioManager.Instance.PlaySFX(SFXType.Button);
    }

    public void ToggleBGM()
    {
        AudioManager.Instance.ToggleBGM();
        AudioManager.Instance.PlaySFX(SFXType.Button);
        menuUpdate();
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
        AudioManager.Instance.PlaySFX(SFXType.Button);
        menuUpdate();
    }

    private void menuUpdate()
    {
        if (AudioManager.Instance.isBgmOn)
        {
            bgmOff.SetActive(false);
        }
        else
        {
            bgmOff.SetActive(true);
        }

        if (AudioManager.Instance.isSfxOn)
        {
            sfxOff.SetActive(false);
        }
        else
        {
            sfxOff.SetActive(true);
        }

        
    }
    public void ToggleButtonSwap()
    {
        // Toggle the setting
        GameManager.Instance.SwapButtons = !GameManager.Instance.SwapButtons;

        // Optionally, update a UI indicator (e.g., change text or icon)
        // e.g., swapIndicator.SetActive(GameManager.Instance.SwapButtons);

        // Play a sound to confirm the action
        AudioManager.Instance.PlaySFX(SFXType.Button);
        UpdateButtonPositionEx();
    }

    public void UpdateButtonPositionEx()
    {
        // Cache RectTransforms
        if (jRect == null) jRect = jumpButtonEx.GetComponent<RectTransform>();
        if (sRect == null) sRect = slideButtonEx.GetComponent<RectTransform>();

        // On first run, capture all the defaults
        if (!defaultsCaptured)
        {
            jPosDef = jRect.anchoredPosition;
            sPosDef = sRect.anchoredPosition;
            jMinDef = jRect.anchorMin;
            sMinDef = sRect.anchorMin;
            jMaxDef = jRect.anchorMax;
            sMaxDef = sRect.anchorMax;
            jPivotDef = jRect.pivot;
            sPivotDef = sRect.pivot;
            defaultsCaptured = true;
        }

        // Apply swap or restore
        if (GameManager.Instance.SwapButtons)
        {
            // Swap positions
            Vector2 tmpPos = jRect.anchoredPosition;
            jRect.anchoredPosition = sRect.anchoredPosition;
            sRect.anchoredPosition = tmpPos;

            // Swap anchorMin
            Vector2 tmpMin = jRect.anchorMin;
            jRect.anchorMin = sRect.anchorMin;
            sRect.anchorMin = tmpMin;

            // Swap anchorMax
            Vector2 tmpMax = jRect.anchorMax;
            jRect.anchorMax = sRect.anchorMax;
            sRect.anchorMax = tmpMax;

            // Swap pivot
            Vector2 tmpPivot = jRect.pivot;
            jRect.pivot = sRect.pivot;
            sRect.pivot = tmpPivot;
        }
        else
        {
            // Restore defaults
            jRect.anchoredPosition = jPosDef;
            sRect.anchoredPosition = sPosDef;
            jRect.anchorMin = jMinDef;
            sRect.anchorMin = sMinDef;
            jRect.anchorMax = jMaxDef;
            sRect.anchorMax = sMaxDef;
            jRect.pivot = jPivotDef;
            sRect.pivot = sPivotDef;
        }
    }
}
