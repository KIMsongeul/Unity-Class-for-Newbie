using TMPro;
using UnityEngine;
using System.Collections.Generic;

// 왜: UI는 '문자열만' 주고받는다. 로직은 전부 LessonManager로 분리.
public class ConsoleUI : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    [SerializeField] TextMeshProUGUI log;
    [SerializeField] LessonManager lesson;

    readonly Queue<string> lines = new();
    const int MaxLines = 12;

    void Start()
    {
        if (!input || !log || !lesson)
            Debug.LogError("ConsoleUI 참조가 비었습니다. input/log/lesson을 연결하세요.");

        Append("예: unlock move / unlock jump / set speed=8 / reset");
        input.onSubmit.AddListener(OnSubmit);
        EnsureFocus();
    }

    void OnDestroy() => input.onSubmit.RemoveListener(OnSubmit);

    void OnSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Append("명령을 입력하세요. 예: unlock move");
            ResetLine();
            return;
        }
        Append($"> {text}");
        string result = lesson.Submit(text);
        Append(result);
        ResetLine();
    }

    void ResetLine()
    {
        input.text = "";
        EnsureFocus();
    }

    void EnsureFocus()
    {
        input.caretPosition = input.text.Length;
        input.selectionStringAnchorPosition = input.selectionStringFocusPosition = input.caretPosition;
        input.ActivateInputField();
    }

    void Append(string s)
    {
        lines.Enqueue(s);
        while (lines.Count > MaxLines) lines.Dequeue();
        log.text = string.Join("\n", lines);
    }
}