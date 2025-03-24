using System.Collections.Generic;
using UnityEngine;

public class CharacterDatabase : MonoBehaviour
{
    public static CharacterDatabase Instance;
    public List<CharacterData> allCharacters;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally: DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize default character if none are owned.
        CharacterData defaultChar = allCharacters.Find(data => data.isDefault);
        if (defaultChar != null)
        {
            CharacterManager.InitializeDefaultCharacter(defaultChar);
        }
    }

    public CharacterData GetCharacterDataByName(string name)
    {
        foreach (CharacterData data in allCharacters)
        {
            if (data.characterName == name)
                return data;
        }
        return null;
    }
}
