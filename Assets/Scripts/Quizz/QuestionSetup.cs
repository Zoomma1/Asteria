using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Interface.ConstellationInfo;

public class QuestionSetup : MonoBehaviour
{
    [SerializeField]
    public List<QuestionData> questions;
    private QuestionData currentQuestion;

    // Stocke la liste d'answers actuellement affichées (après randomize)
    private List<string> currentAnswers;

    // Marqueur visuel utilisé pour pointer la constellation correcte (fallback)
    private GameObject pointerMarker;

    // Constellation actuellement highlightée par le quiz
    private ConstellationData highlightedConstellation;

    [SerializeField]
    private TextMeshProUGUI questionText;
    [SerializeField]
    private TextMeshProUGUI categoryText;
    [SerializeField]
    private AnswerButton[] answerButtons;
    [SerializeField]
    public TextMeshProUGUI wrongAnswersText;

    // Temps max (en secondes) à attendre pour que les constellations soient chargées
    [SerializeField]
    private float pointSearchTimeout = 5f;

    [SerializeField]
    private int correctAnswerChoice;

    // Camera control (optionnel)
    [Header("Camera pointing")]
    [SerializeField]
    [Tooltip("Transform de la caméra ou de l'objet à orienter vers la constellation. Si null, on utilisera Camera.main.transform")] 
    private Transform cameraToControl;

    [SerializeField]
    [Tooltip("Si vrai, on fera une rotation lissée de la caméra vers la constellation")] 
    private bool smoothCamera = true;

    [SerializeField]
    [Tooltip("Vitesse de rotation (plus grand = plus rapide)")]
    private float cameraRotateSpeed = 2f;

    [SerializeField]
    [Tooltip("Si vrai, on déplacera aussi la caméra pour avoir la constellation à une distance donnée")]
    private bool moveCameraToView = false;

    [SerializeField]
    [Tooltip("Distance à garder entre la caméra et la constellation si 'moveCameraToView' est activé")]
    private float cameraMoveDistance = 10f;

    [SerializeField]
    [Tooltip("Vitesse de déplacement de la caméra")]
    private float cameraMoveSpeed = 2f;

    private void Awake()
    {
        GetQuestionAssets();
    }

    public void Start()
    {
        // Nettoyer tout highlight/marker précédent si présent
        if (highlightedConstellation != null)
        {
            highlightedConstellation.Unhighlight();
            highlightedConstellation = null;
        }
        if (pointerMarker != null)
        {
            Destroy(pointerMarker);
            pointerMarker = null;
        }

        // Si cameraToControl non assignée, on tente de trouver automatiquement le bon root
        if (cameraToControl == null)
        {
            cameraToControl = FindBestCameraControlRoot();
            Debug.Log("QuestionSetup.Start: cameraToControl auto-assignée à: " + (cameraToControl != null ? cameraToControl.name : "null"));
        }

        SelectNewQuestion();
        SetQuestionValues();
        SetAnswerValues();
        // Lancer la coroutine qui attend la création des constellations si nécessaire
        StartCoroutine(PointToConstellationCoroutine(pointSearchTimeout));
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

        // Sauvegarder les réponses actuelles pour les réutiliser
        currentAnswers = answers;

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
        if (originalList == null || originalList.Count == 0)
            return new List<string>();

        // La première entrée de QuestionData.answers est la réponse correcte (convention).
        string correctAnswerValue = originalList[0];
        bool correctAnswerChosen = false;

        List<string> newList = new List<string>();

        int choices = Mathf.Min(answerButtons.Length, originalList.Count);

        for (int i = 0; i < choices; i++)
        {
            int random = Random.Range(0, originalList.Count);

            // Si l'élément sélectionné correspond à la valeur de la bonne réponse, on marque l'index i comme correct
            if (!correctAnswerChosen && originalList[random] == correctAnswerValue)
            {
                correctAnswerChoice = i;
                correctAnswerChosen = true;
            }

            newList.Add(originalList[random]);
            originalList.RemoveAt(random);
        }

        // Si pour une raison quelconque la bonne réponse n'a pas été choisie (peu probable), la placer en première position
        if (!correctAnswerChosen)
        {
            // tenter d'ajouter la bonne réponse à l'indice 0 et la marquer comme correcte
            newList.Insert(0, correctAnswerValue);
            correctAnswerChoice = 0;
        }

        return newList;
    }

    private void PointToConstellation()
    {
        // Deprecated: we now use the coroutine version. Keep for compatibility.
        StartCoroutine(PointToConstellationCoroutine(pointSearchTimeout));
    }

    private IEnumerator PointToConstellationCoroutine(float timeout)
    {
        // Vérifier que nous avons bien les réponses et un index valide
        if (currentAnswers == null || currentAnswers.Count == 0)
        {
            Debug.LogWarning("PointToConstellation: currentAnswers is empty");
            yield break;
        }

        if (correctAnswerChoice < 0 || correctAnswerChoice >= currentAnswers.Count)
        {
            Debug.LogWarningFormat(this, "PointToConstellation: correctAnswerChoice out of range: {0}", correctAnswerChoice);
            yield break;
        }

        string correctAnswer = currentAnswers[correctAnswerChoice]?.Trim();
        if (string.IsNullOrEmpty(correctAnswer))
        {
            Debug.LogWarning("PointToConstellation: correctAnswer is empty or null");
            yield break;
        }

        float elapsed = 0f;
        ConstellationData target = null;

        // Retry loop: attend que les constellations soient créées par ConstellationFieldFromApi
        while (elapsed < timeout)
        {
            // chercher les constellations actuelles
            ConstellationData[] allConstellations = GameObject.FindObjectsOfType<ConstellationData>();

            foreach (var c in allConstellations)
            {
                if (c.Constellation != null && string.Equals(c.Constellation.name?.Trim(), correctAnswer, System.StringComparison.OrdinalIgnoreCase))
                {
                    target = c;
                    break;
                }
            }

            if (target != null)
                break;

            // Attendre la prochaine frame et incrémenter le temps écoulé
            yield return null;
            elapsed += Time.deltaTime;
        }

        if (target == null)
        {
            Debug.LogWarningFormat(this, "PointToConstellation: no ConstellationData found matching '{0}' after {1} seconds", correctAnswer, timeout);

            // fallback : créer un marker global à la position (0,0,0) ou aucun
            if (pointerMarker != null) Destroy(pointerMarker);
            pointerMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pointerMarker.name = "QuestionPointerMarker_Fallback";
            pointerMarker.transform.position = Vector3.zero;
            pointerMarker.transform.localScale = Vector3.one * 0.25f;
            var rendF = pointerMarker.GetComponent<Renderer>();
            if (rendF != null)
            {
                rendF.material = new Material(Shader.Find("Standard"));
                rendF.material.color = Color.yellow;
            }
            var colF = pointerMarker.GetComponent<Collider>(); if (colF != null) Destroy(colF);

            yield break;
        }

        // Si on a trouvé la constellation : highlight
        // Restaurer l'ancien highlight si différent
        if (highlightedConstellation != null && highlightedConstellation != target)
        {
            highlightedConstellation.Unhighlight();
            highlightedConstellation = null;
        }

        // Highlight la constellation cible
        target.Highlight(Color.yellow);
        highlightedConstellation = target;

        // Orienter la caméra vers la constellation trouvée (optionnel)
        StartCoroutine(OrientCameraToTarget(target.transform));

        Debug.LogFormat(this, "PointToConstellation: highlighted constellation '{0}'", correctAnswer);

        yield break;
    }

    // Cherche un ancestor utile pour contrôler la caméra (ex: XR Origin / XR Rig), sinon retourne Camera.main.transform
    private Transform FindBestCameraControlRoot()
    {
        if (cameraToControl != null) return cameraToControl;
        if (Camera.main == null) return null;

        // Commencer par la caméra principale
        Transform cam = Camera.main.transform;

        // Si la caméra a un parent, chercher un ancestor dont le nom contient des indices 'XR' ou qui possède des composants XRRig/XROrigin
        Transform current = cam.parent;
        while (current != null)
        {
            string nameLower = current.name.ToLowerInvariant();
            if (nameLower.Contains("xr") || nameLower.Contains("rig") || nameLower.Contains("origin"))
            {
                Debug.Log("FindBestCameraControlRoot: selected ancestor by name: " + current.name);
                return current;
            }

            // Rechercher par type (XROrigin, XRRig) via reflection
            var typesToFind = new string[] { "Unity.XR.CoreUtils.XROrigin", "UnityEngine.XR.Interaction.Toolkit.XRRig", "XROrigin", "XRRig" };
            foreach (var tname in typesToFind)
            {
                var t = System.Type.GetType(tname);
                if (t != null)
                {
                    var comp = current.GetComponent(t);
                    if (comp != null)
                    {
                        Debug.Log("FindBestCameraControlRoot: selected ancestor by component: " + current.name + " (" + tname + ")");
                        return current;
                    }
                }
                else
                {
                    // fallback: chercher par nom si le type n'existe pas
                    if (tname == "XROrigin" || tname == "XRRig")
                    {
                        // rien ici, la vérification de nom est déjà effectuée
                    }
                }
            }

            current = current.parent;
        }

        // Aucun ancestor trouvé : retourner la transform de la caméra
        return cam;
    }

    private IEnumerator OrientCameraToTarget(Transform target)
    {
        if (target == null) yield break;

        Transform camTransform = FindBestCameraControlRoot();

        if (camTransform == null)
        {
            Debug.LogWarning("OrientCameraToTarget: no camera found (Camera.main is null and cameraToControl not assigned)");
            yield break;
        }

        Debug.Log("OrientCameraToTarget: controlling transform = " + camTransform.name + " (cameraToControl=" + (cameraToControl != null) + ")");

        // Détecter un TrackedPoseDriver / dispositifs de tracking qui pourraient écraser la position de la caméra
        System.Type trackedPoseType = System.Type.GetType("UnityEngine.SpatialTracking.TrackedPoseDriver, UnityEngine.SpatialTracking");
        if (trackedPoseType == null)
        {
            trackedPoseType = System.Type.GetType("UnityEngine.SpatialTracking.TrackedPoseDriver");
        }

        Component trackedComp = null;
        Behaviour trackedBehaviour = null;
        if (trackedPoseType != null)
        {
            // Chercher le composant dans l'arbre parent (camTransform inclus)
            trackedComp = camTransform.GetComponentInParent(trackedPoseType);
            if (trackedComp != null)
            {
                trackedBehaviour = trackedComp as Behaviour;
                if (trackedBehaviour != null)
                {
                    Debug.Log("OrientCameraToTarget: disabling tracked behaviour: " + trackedBehaviour.GetType().Name + " on " + trackedBehaviour.gameObject.name);
                    trackedBehaviour.enabled = false;
                    // attend une courte durée pour s'assurer que le tracking ne réapplique pas la position
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        Vector3 targetPos = target.position;

        // Compute target rotation once so we can apply it both to the rig transform and to Camera.main if needed
        Quaternion endRot = Quaternion.LookRotation(targetPos - camTransform.position);

        Debug.LogFormat("OrientCameraToTarget: moving camera '{0}' (pos={1}) to look at target '{2}' (pos={3})", camTransform.name, camTransform.position, target.name, targetPos);

        // Optionnel : déplacer la caméra vers une position à distance donnée
        if (moveCameraToView)
        {
            // Determine a safe distance: prefer using the target's SphereCollider radius when available
            float desiredDistance = cameraMoveDistance;
            var targetCollider = target.GetComponent<SphereCollider>();
            if (targetCollider != null)
            {
                // use collider radius plus margin
                desiredDistance = targetCollider.radius + cameraMoveDistance;
                Debug.Log("OrientCameraToTarget: using target SphereCollider.radius = " + targetCollider.radius + ", desiredDistance = " + desiredDistance);
            }

            Vector3 dirToCam = (camTransform.position - targetPos).normalized;
            Vector3 desiredPos = targetPos + dirToCam * desiredDistance;

            float t = 0f;
            while (Vector3.Distance(camTransform.position, desiredPos) > 0.05f)
            {
                camTransform.position = Vector3.Lerp(camTransform.position, desiredPos, Time.deltaTime * cameraMoveSpeed);
                camTransform.LookAt(targetPos);
                // Debug visual: dessiner la ligne entre la caméra et la cible
                Debug.DrawLine(camTransform.position, targetPos, Color.cyan, 0.1f);
                yield return null;
                t += Time.deltaTime;
                if (t > 10f) break; // safe-guard
            }
        }

        if (smoothCamera)
        {
            Quaternion startRot = camTransform.rotation;
            float progress = 0f;
            while (progress < 1f)
            {
                progress += Time.deltaTime * cameraRotateSpeed;
                camTransform.rotation = Quaternion.Slerp(startRot, endRot, Mathf.SmoothStep(0f, 1f, progress));
                // Debug visual: dessiner la ligne pendant la rotation
                Debug.DrawLine(camTransform.position, targetPos, Color.yellow, 0.1f);
                yield return null;
            }
            camTransform.rotation = endRot;
        }
        else
        {
            camTransform.LookAt(targetPos);
        }

        // Force the camera (and the actual Camera.main.transform) to look at the target for a few frames
        int forceFrames = 5;
        for (int i = 0; i < forceFrames; i++)
        {
            if (camTransform != null)
                camTransform.LookAt(targetPos);
            // Also force the actual Camera.main rotation to the computed endRot so the camera component aligns
            if (Camera.main != null && Camera.main.transform != camTransform)
            {
                Camera.main.transform.rotation = endRot;
                Camera.main.transform.LookAt(targetPos);
            }
             // draw for debug
             Debug.DrawLine((Camera.main != null ? Camera.main.transform.position : camTransform.position), targetPos, Color.magenta, 0.1f);
             yield return null;
         }

         // Log final orientation vectors for debugging
         if (camTransform != null)
             Debug.LogFormat("OrientCameraToTarget: final forward = {0}, toTarget = {1}", camTransform.forward, (targetPos - camTransform.position).normalized);
         if (Camera.main != null)
             Debug.LogFormat("OrientCameraToTarget: Camera.main.forward = {0}", Camera.main.transform.forward);

        // réactiver le tracked behaviour si on l'avait désactivé
        if (trackedBehaviour != null)
        {
            // attendre une frame pour s'assurer que la caméra a stabilisé sa nouvelle position
            yield return new WaitForSeconds(0.05f);
            trackedBehaviour.enabled = true;
            Debug.Log("OrientCameraToTarget: re-enabled tracked behaviour: " + trackedBehaviour.GetType().Name + " on " + trackedBehaviour.gameObject.name);
        }

        Debug.Log("OrientCameraToTarget: done");

        yield break;
    }
}
