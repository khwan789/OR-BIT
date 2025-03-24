using UnityEngine;

public class PlayerInputButton : MonoBehaviour
{
    // UI �̺�Ʈ(��: Pointer Down/Up)���� ȣ���Ͽ�, PlayerInputManager�� static �޼��带 ȣ���մϴ�.
    public void OnJumpButtonDown() => PlayerInputManager.JumpButtonDown();
    public void OnJumpButtonUp() => PlayerInputManager.JumpButtonUp();
    public void OnSlideButtonDown() => PlayerInputManager.SlideButtonDown();
    public void OnSlideButtonUp() => PlayerInputManager.SlideButtonUp();
}
