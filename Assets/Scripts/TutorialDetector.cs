using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialDetector : MonoBehaviour
{
    public bool isTutorial1;
    public bool isTutorial2;
    public bool isTutorialCleared;
    public GameObject tutorialJump;
    public GameObject tutorialSlide;

    public GameObject JumpButton;
    public GameObject SlideButton;

    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.Instance.isTutorialCleared)
        {
            JumpButton.SetActive(false);
            SlideButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !GameManager.Instance.isTutorialCleared)
        {
            if (isTutorial1)
            {
                Time.timeScale = 0;
                tutorialJump.SetActive(true);
                JumpButton.SetActive(true);

            }
            if (isTutorial2)
            {
                Time.timeScale = 0;
                tutorialSlide.SetActive(true);
                JumpButton.SetActive(false);
                SlideButton.SetActive(true);
            }
            if (isTutorialCleared)
            {
                GameManager.Instance.isTutorialCleared = true;
                GameManager.Instance.ClearTutorial();
            }
        }
    }

    public void TutorialContinue()
    {
        Time.timeScale = 1;
        if (isTutorial1)
        {
            tutorialJump.SetActive(false);
        }
        if (isTutorial2)
        {
           tutorialSlide.SetActive(false);
           JumpButton.SetActive(true);
        }
    }
}
