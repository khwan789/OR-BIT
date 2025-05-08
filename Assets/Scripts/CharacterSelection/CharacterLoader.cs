using System;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    private GameObject character;
    private Vector3 startPos;

    private void Awake()
    {
        startPos = this.transform.position;
    }

    private void Start()
    {
	    ChangeToEquippedCharacter(startPos);
    }

    public void ChangeToEquippedCharacter()
    {
        // Get the equipped character's name.
        string equippedName = CharacterManager.GetEquippedCharacter();
        if (string.IsNullOrEmpty(equippedName))
        {
            Debug.LogWarning("No character is equipped! Loading default character.");
            return;
        }

        // Look up the corresponding CharacterData.
        CharacterData data = CharacterDatabase.Instance.GetCharacterDataByName(equippedName);
        if (data != null && data.characterPrefab != null)
        {
            if (character == null)
            {
                character = Instantiate(data.characterPrefab, startPos, Quaternion.identity);
            }
            else
            {
                Destroy(character);
                character = Instantiate(data.characterPrefab, startPos, Quaternion.identity);
            }
            PlayerManager.SetPlayer(character);
        }
        else
        {
            Debug.LogWarning("Equipped character prefab not found. Check your CharacterDatabase.");
        }
    }

    public void ChangeToEquippedCharacter(Vector3 position)
    {
        // Get the equipped character's name.
        string equippedName = CharacterManager.GetEquippedCharacter();
        if (string.IsNullOrEmpty(equippedName))
        {
            Debug.LogWarning("No character is equipped! Loading default character.");
            return;
        }

        // Look up the corresponding CharacterData.
        CharacterData data = CharacterDatabase.Instance.GetCharacterDataByName(equippedName);
        if (data != null && data.characterPrefab != null)
        {
            if (character == null)
            {
                character = Instantiate(data.characterPrefab, position, Quaternion.identity);
            }
            else
            {
                Destroy(character);
                character = Instantiate(data.characterPrefab, position, Quaternion.identity);
            }
            PlayerManager.SetPlayer(character);
        }
        else
        {
            Debug.LogWarning("Equipped character prefab not found. Check your CharacterDatabase.");
        }
    }
}
