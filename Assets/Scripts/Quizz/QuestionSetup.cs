using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionSetup : MonoBehaviour
{
    [SerializeField]
    public List<QuestionData> questions;
    private QuestionData currentQuestion;

    [SerializeField]
    private TextMeshProUGUI questionText;
    [SerializeField]
    private TextMeshProUGUI categoryText;
    [SerializeField]
    private AnswerButton[] answerButtons;
    [SerializeField]
    public TextMeshProUGUI wrongAnswersText;

    [SerializeField]
    private int correctAnswerChoice;

    private void Awake()
    {
        GetQuestionAssets();
    }

    public void Start()
    {
        SelectNewQuestion();
        SetQuestionValues();
        SetAnswerValues();
        wrongAnswersText.enabled = false;
    }

    private void GetQuestionAssets()
    {
        questions = new List<QuestionData>(Resources.LoadAll<QuestionData>("Questions"));
    }

    private void SelectNewQuestion()
    {
        int randomQuestionIndex = Random.Range(0, questions.Count);
        currentQuestion = questions[randomQuestionIndex];
        questions.RemoveAt(randomQuestionIndex);
    }

    private void SetQuestionValues()
    {
        questionText.text = currentQuestion.question;
    }

    private void SetAnswerValues()
    {
        List<string> answers = RandomizeAnswers(new List<string>(currentQuestion.answers));

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool isCorrect = false;
            if(i == correctAnswerChoice)
            {
                isCorrect = true;
            }

            answerButtons[i].SetIsCorrect(isCorrect);
            answerButtons[i].SetAnswerText(answers[i]);
        }
    }

    private List<string> RandomizeAnswers(List<string> originalList)
    {
        bool correctAnswerChosen = false;

        List<string>  newList = new List<string>();

        for(int i = 0; i < answerButtons.Length; i++)
        {
            int random = Random.Range(0, originalList.Count);
            if(random == 0 && !correctAnswerChosen)
            {
                correctAnswerChoice = i;
                correctAnswerChosen = true;
            }
            newList.Add(originalList[random]);
            originalList.RemoveAt(random);  
        }

        return newList;
    }
}