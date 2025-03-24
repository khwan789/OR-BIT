using System;
using System.Linq;
using UnityEngine;

public static class CharacterManager
{
    private const string EquippedCharacterKey = "EquippedCharacter";
    private const string OwnedCharactersKey = "OwnedCharacters"; // Comma-separated list

    // Call this method at game start (for example, in CharacterDatabase or a dedicated initializer)
    public static void InitializeDefaultCharacter(CharacterData defaultCharacter)
    {
        // Check if no characters are owned yet.
        string owned = PlayerPrefs.GetString(OwnedCharactersKey, "");
        if (string.IsNullOrEmpty(owned) && defaultCharacter != null)
        {
            PlayerPrefs.SetString(OwnedCharactersKey, defaultCharacter.characterName);
            PlayerPrefs.SetString(EquippedCharacterKey, defaultCharacter.characterName);
            PlayerPrefs.Save();
            Debug.Log("Default character unlocked: " + defaultCharacter.characterName);
        }
    }

    public static bool IsCharacterOwned(string characterName)
    {
        string owned = PlayerPrefs.GetString(OwnedCharactersKey, "");
        if (string.IsNullOrEmpty(owned)) return false;
        string[] ownedArray = owned.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        return ownedArray.Contains(characterName);
    }

    public static void UnlockCharacter(string characterName)
    {
        if (!IsCharacterOwned(characterName))
        {
            string owned = PlayerPrefs.GetString(OwnedCharactersKey, "");
            owned = string.IsNullOrEmpty(owned) ? characterName : owned + "," + characterName;
            PlayerPrefs.SetString(OwnedCharactersKey, owned);
            PlayerPrefs.Save();
            Debug.Log("Character Unlocked: " + characterName);
            // You can trigger a notification (e.g. red dot) here.
        }
    }

    public static void EquipCharacter(string characterName)
    {
        if (!IsCharacterOwned(characterName))
        {
            Debug.LogWarning("Attempt to equip a character that is not owned: " + characterName);
            return;
        }
        PlayerPrefs.SetString(EquippedCharacterKey, characterName);
        PlayerPrefs.Save();
        Debug.Log("Character Equipped: " + characterName);
    }

    public static string GetEquippedCharacter()
    {
        return PlayerPrefs.GetString(EquippedCharacterKey, "");
    }
}
