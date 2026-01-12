// This script is for the buttons the answers will go on

using UnityEngine;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    private bool isCorrect;
    [SerializeField] private TextMeshProUGUI answerText;

    // To make it ask a new question after the first question
    [SerializeField] private QuestionSetup questionSetup;

    public void SetAnswerText(string newText)
    {
        answerText.text = newText;
    }

    public void SetIsCorrect(bool newBool)
    {
        isCorrect = newBool;
    }

    public void OnClick()
    {
        if(isCorrect)
        {
            Debug.Log("CORRECT ANSWER");
            if (questionSetup.questions.Count > 0)
            {
                // Generate a new question
                questionSetup.Start();
            }
        }
        else
        {
            Debug.Log("WRONG ANSWER");
            questionSetup.wrongAnswersText.enabled = true;
        }

    }
}