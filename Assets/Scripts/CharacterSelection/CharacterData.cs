using UnityEngine;

public enum UnlockType
{
    Achievement,
    Purchase
}

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public GameObject previewImage;        // For UI preview in selection popup
    public GameObject unownedImage;
    public GameObject characterPrefab; // Prefab to use in gameplay
    public UnlockType unlockType;
    public string unlockConditionText; // E.g. "Reach 1000 points" or "Price: 500 Gold"
    public int price;                  // Price (if Purchase type)
    public bool isDefault;             // Set this true for your main/default character
}
