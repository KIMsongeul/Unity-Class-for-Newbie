using UnityEngine;

// 왜: 입력은 항상 읽되, "적용" 전에 허용 여부를 검사하기 위해.
public class AbilityManager : MonoBehaviour
{
    public bool MoveHorizontal { get; private set; }
    public bool Jump { get; private set; }

    public void UnlockMoveHorizontal() => MoveHorizontal = true;
    public void UnlockJump() => Jump = true;

    public void ResetAll()
    {
        MoveHorizontal = false;
        Jump = false;
    }
}