using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static GameObject Instance { get; private set; }

    public static void SetPlayer(GameObject player)
    {
        Instance = player;
    }
}
