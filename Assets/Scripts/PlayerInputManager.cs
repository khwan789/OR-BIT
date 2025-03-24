using System;
using UnityEngine;

public static class PlayerInputManager
{
    // �÷��̾ �̺�Ʈ�� �����Ͽ� �Է��� �����մϴ�.
    public static Action<bool> OnJumpInput;
    public static Action<bool> OnSlideInput;

    public static void JumpButtonDown() => OnJumpInput?.Invoke(true);
    public static void JumpButtonUp() => OnJumpInput?.Invoke(false);
    public static void SlideButtonDown() => OnSlideInput?.Invoke(true);
    public static void SlideButtonUp() => OnSlideInput?.Invoke(false);
}
