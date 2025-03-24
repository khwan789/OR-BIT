using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    [HideInInspector] public static AchievementManager Instance;
    GameManager gameManager;
    public bool isUnlocked;

    private void Awake()
    {
        // ½Ì±ÛÅæ ÃÊ±âÈ­
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
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;   
    }

    public void CheckAllAchievements()
    {
        CatAchievement();
        RocketAchievement();
    }

    private void CatAchievement()
    {
        if (gameManager.GetTotalGamePlay() >= 99 && !CharacterManager.IsCharacterOwned("Cat"))
        {
            //play 9 times
            CharacterManager.UnlockCharacter("Cat");
            isUnlocked = true;
            // Set flag to show red dot
            PlayerPrefs.SetInt("NewUnlockNotification", 1);
            PlayerPrefs.Save();
        }
    }
    private void RocketAchievement()
    {
        if (gameManager.GetTotalRound() >= 100 && !CharacterManager.IsCharacterOwned("Rocket"))
        {
            //total 100 rounds
            CharacterManager.UnlockCharacter("Rocket");
            isUnlocked = true;
            // Set flag to show red dot
            PlayerPrefs.SetInt("NewUnlockNotification", 1);
            PlayerPrefs.Save();
        }
    }
}



