using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using Random = UnityEngine.Random;

public class CVStoSO
{
    private static string questionsCSVPath = "/Resources/CSVs/QuizAsteria.csv";
    private static string questionsPath = "Assets/Resources/Questions/";
    private static int numberOfAnswers = 4;

    [MenuItem("Utilities/Generate Questions")]
    public static void GeneratePhrases()
    {

        string[] allLines = File.ReadAllLines(Application.dataPath + questionsCSVPath);

        foreach (string s in allLines)
        {
            string[] splitData = s.Split(';');

            // CSV (COMMA SEPARATED VALUE) DATA FORMAT
            // QUESTION, CATEGORY, CORRECT ANSWER, WRONG ANSWER 1, WRONG ANSWER 2, WRONG ANSWER 3

            QuestionData questionData = ScriptableObject.CreateInstance<QuestionData>();
            questionData.question = splitData[0];

            questionData.answers = new string[4];

            if (!Directory.Exists(questionsPath))
            {
                Directory.CreateDirectory(questionsPath);
            }

            for (int i = 0; i < numberOfAnswers; i++)
            {
                questionData.answers[i] = splitData[1 + i];
            }

            if (questionData.question.Contains("?"))
            {
                questionData.name = questionData.question.Remove(questionData.question.IndexOf("?"));
            }
            else
            {
                questionData.name = questionData.question;
            }
            AssetDatabase.CreateAsset(questionData, $"{questionsPath}/{Random.Range(0, Int32.MaxValue)}.asset");
        }
        AssetDatabase.SaveAssets();
    }
}
