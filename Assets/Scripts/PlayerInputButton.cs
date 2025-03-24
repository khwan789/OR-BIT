using UnityEngine;

public class PlayerInputButton : MonoBehaviour
{
    // UI 이벤트(예: Pointer Down/Up)에서 호출하여, PlayerInputManager의 static 메서드를 호출합니다.
    public void OnJumpButtonDown() => PlayerInputManager.JumpButtonDown();
    public void OnJumpButtonUp() => PlayerInputManager.JumpButtonUp();
    public void OnSlideButtonDown() => PlayerInputManager.SlideButtonDown();
    public void OnSlideButtonUp() => PlayerInputManager.SlideButtonUp();
}
