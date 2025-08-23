using UnityEngine;

// 왜: 정답 제출 시점에만 게임 규칙 변화(해금/속도)를 반영해 학습 흐름을 통제
public class LessonManager : MonoBehaviour
{
    public AbilityManager abilities;
    public Player2DController player;

    // 콘솔에서 호출: 명령 → 결과 메시지
    public string Submit(string cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd)) return "명령을 입력하세요.";
        string t = cmd.Trim().ToLowerInvariant();

        if (t == "unlock move")
        {
            abilities.UnlockMoveHorizontal();
            return "이동 해금! A/D로 움직여 보세요.";
        }
        if (t == "unlock jump")
        {
            abilities.UnlockJump();
            return "점프 해금! Space로 점프해 보세요.";
        }
        if (t.StartsWith("set speed="))
        {
            var val = t.Substring("set speed=".Length);
            if (float.TryParse(val, out var s))
            {
                player.SetSpeed(s);
                return $"속도 = {player.name}:{s}";
            }
            return "속도 값이 잘못되었습니다. 예: set speed=8";
        }
        if (t == "reset")
        {
            abilities.ResetAll();
            player.SetSpeed(6f);
            return "모든 능력 잠금 + 속도 초기화";
        }

        return "알 수 없는 명령어. 예: unlock move / unlock jump / set speed=8 / reset";
    }
}