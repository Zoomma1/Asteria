using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "ScriptableObject/Question", order = 1)]
public class QuestionData : ScriptableObject
{
    public string question;
    [Tooltip("correct answer must be the first element, randomize later")]
    public string[] answers;
}
