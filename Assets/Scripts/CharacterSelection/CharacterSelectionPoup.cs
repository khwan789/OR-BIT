using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionPopup : MonoBehaviour
{
    public static CharacterSelectionPopup Instance;

    [Header("UI References")]
    public GameObject characterItemPrefab; // Prefab for each character item
    public GameObject contentParent;          // Parent transform (content of the ScrollRect)
    public Canvas canvas;

    [Header("Character Data List")]
    public List<CharacterData> characterDataList; // List of all characters (assign in Inspector)

    public GameObject leftButton;
    public GameObject rightButton;
    private float itemWidth;
    private float minContentWidth;
    private float maxContentPosX;
    RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
        itemWidth = characterItemPrefab.GetComponent<RectTransform>().rect.width;
        minContentWidth = Screen.width;

        rectTransform = contentParent.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(minContentWidth,rectTransform.sizeDelta.y);
        //gameObject.SetActive(false); // Hide popup initially

        if (characterDataList.Count > 4)
        {
            int extraCount = characterDataList.Count - 4;
            float increaseWidth = extraCount * itemWidth;    
            rectTransform.sizeDelta = new Vector2(minContentWidth + increaseWidth, rectTransform.sizeDelta.y);
            maxContentPosX = minContentWidth - rectTransform.rect.width;
        }        
    }

    private void Update()
    {
       
    }

    public void Show()
    {
        gameObject.SetActive(true);
        RefreshUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void RefreshUI()
    {
        // Clear existing items
        foreach (Transform child in contentParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate a UI item for each character
        foreach (CharacterData data in characterDataList)
        {
            GameObject itemGO = Instantiate(characterItemPrefab, contentParent.transform);
            CharacterItemUI itemUI = itemGO.GetComponent<CharacterItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(data);
            }
        }
    }

    public void ContentLeft()
    {
        // Get the RectTransform for the content
        RectTransform contentRect = contentParent.GetComponent<RectTransform>();
        // Enable the right button since we are moving left
        rightButton.SetActive(true);

        // Calculate the effective move amount factoring in the UI scale.
        Vector2 moveAmount = new Vector2(itemWidth, 0);

        // Increase the anchoredPosition.x by moveAmount (moving content rightward relative to its parent)
        contentRect.anchoredPosition += moveAmount;

        // If the new anchored position is approximately at the starting position (i.e. x close to 0), disable the left button
        if (Mathf.Approximately(contentRect.anchoredPosition.x, 0))
        {
            leftButton.SetActive(false);
        }
    }


    public void ContentRight()
    {
        leftButton.SetActive(true);
        RectTransform contentRect = contentParent.GetComponent<RectTransform>();

        // Instead of using transform.position, use anchoredPosition.
        // The movement amount may need to be scaled by the canvas scale factor:
        Vector2 moveAmount = new Vector2(itemWidth, 0);

        contentRect.anchoredPosition -= moveAmount;

        if (Mathf.Approximately(contentRect.anchoredPosition.x, maxContentPosX))
        {
            rightButton.SetActive(false);
        }
    }

}
