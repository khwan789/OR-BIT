using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

public class CharacterItemUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject previewContainer;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI statusText;
    public Button actionButton;
    public GameObject goldIcon;
    public GameObject toolTip;
    private TextMeshProUGUI toolTipText;
    private Canvas canvas;
    private CharacterData characterData;
    private CharacterLoader characterLoader;
    private RectTransform selfRect;

    private void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        characterLoader = FindObjectOfType<CharacterLoader>();
        toolTipText = toolTip.GetComponentInChildren<TextMeshProUGUI>();   
    }

    public void Setup(CharacterData data)
    {
        characterData = data;
        if (characterNameText != null)
            characterNameText.text = data.characterName;
        UpdatePreview();
        UpdateStatus();
    }

    private void UpdatePreview()
    {
        bool owned = CharacterManager.IsCharacterOwned(characterData.characterName);
        // Clear any existing preview object in the container
        foreach (Transform child in previewContainer.transform)
        {
            Destroy(child.gameObject);
        }
        // Instantiate the new preview object as a child of the container
        if (characterData != null && characterData.previewImage != null)
        {
            if(owned) 
            { 
                GameObject newPreview = Instantiate(characterData.previewImage, previewContainer.transform);
            }
            else
            {
                if (characterData.unownedImage != null)
                {
                    GameObject newPreview = Instantiate(characterData.unownedImage, previewContainer.transform);
                }
            }

            // Optionally, adjust newPreview's transform (scale, position) as needed.
        }
    }

    public void UpdateStatus()
    {
        bool owned = CharacterManager.IsCharacterOwned(characterData.characterName);
        string equipped = CharacterManager.GetEquippedCharacter();

        if (!owned)
        {
            if (characterData.unlockType == UnlockType.Purchase && GameManager.Instance.GetTotalGold() >= characterData.price)
            {
                // Display the purchase price
                goldIcon.SetActive(true);
                statusText.text = characterData.price.ToString();
                actionButton.interactable = true; // Enable the button for purchase
            }
            else if (characterData.unlockType == UnlockType.Purchase && GameManager.Instance.GetTotalGold() < characterData.price)
            {
                // Display the purchase price
                goldIcon.SetActive(true);
                statusText.text = characterData.price.ToString();
                actionButton.interactable = false; // Enable the button for purchase
            }
            else
            {
                goldIcon.SetActive(false);
                // For achievements, display the unlock condition text
                statusText.text = characterData.unlockConditionText;
                // Optionally, disable the button if no direct purchase is allowed
                actionButton.interactable = false;
            }
        }
        else
        {
            goldIcon.SetActive(false);
            if (equipped == characterData.characterName)
            {
                statusText.text = "Equipped";
                actionButton.interactable = false;
            }
            else
            {
                statusText.text = "Equip";
                actionButton.interactable = true;
            }
        }
    }

    // This method is assigned to the Action Button's OnClick event.
    public void OnActionButtonClicked()
    {
        bool owned = CharacterManager.IsCharacterOwned(characterData.characterName);
        var text = "";
        if (!owned)
        {
            // For purchasable characters, check if the player has enough gold.
            if (characterData.unlockType == UnlockType.Purchase)
            {
                if (GameManager.Instance != null && GameManager.Instance.CanSpendGold(characterData.price))
                {
                    GameManager.Instance.SpendGold(characterData.price);
                    CharacterManager.UnlockCharacter(characterData.characterName);
                    UpdateStatus();
                }
                else
                {
                    Debug.Log("Not enough gold to purchase " + characterData.characterName);
                }
            }
            else
            {
                Debug.Log("Unlock this character by: " + characterData.unlockConditionText);
            }
        }
        else
        {
            // Equip the character
            CharacterManager.EquipCharacter(characterData.characterName);
            // Refresh the entire popup to update statuses.
            CharacterSelectionPopup.Instance.RefreshUI();
            text = characterData.characterName.ToString() + " Equipped";
            characterLoader.ChangeToEquippedCharacter();   
        }
        ActivateToolTip(text);
        AudioManager.Instance.PlaySFX(SFXType.Button);
    }

    void ActivateToolTip(string text)
    {
        toolTipText.text = text;
        Instantiate(toolTip, canvas.transform);
    }
}
