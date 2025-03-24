using System;
using UnityEngine;

public static class PlayerInputManager
{
    // 플레이어가 이벤트에 구독하여 입력을 수신합니다.
    public static Action<bool> OnJumpInput;
    public static Action<bool> OnSlideInput;

    public static void JumpButtonDown() => OnJumpInput?.Invoke(true);
    public static void JumpButtonUp() => OnJumpInput?.Invoke(false);
    public static void SlideButtonDown() => OnSlideInput?.Invoke(true);
    public static void SlideButtonUp() => OnSlideInput?.Invoke(false);
}
