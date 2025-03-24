using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    private GameObject character;

    private void Start()
    {
        ChangeToEquippedCharacter();
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
                character = Instantiate(data.characterPrefab, this.transform.position, this.transform.rotation);
            }
            else
            {
                Destroy(character);
                character = Instantiate(data.characterPrefab, this.transform.position, this.transform.rotation);
            }
            PlayerManager.SetPlayer(character);
        }
        else
        {
            Debug.LogWarning("Equipped character prefab not found. Check your CharacterDatabase.");
        }
    }
}
