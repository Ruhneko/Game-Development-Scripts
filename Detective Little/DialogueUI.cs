using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private GameObject A;
    [SerializeField] private GameObject B;
    private int count = 0;
    public string sceneName;

    private TypewriterEffect typewriterEffect;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        CloseDialogueBox();
        ShowDialogue(testDialogue);
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {

        foreach (string dialogue in dialogueObject.Dialogue)
        {
            if (dialogueObject.Person[count] == true)
                {
                    A.SetActive(true);
                    B.SetActive(false);
                    count++;
                }else{
                    B.SetActive(true);
                    A.SetActive(false);
                    count++;
                }
            yield return typewriterEffect.Run(dialogue, textLabel);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }
        CloseDialogueBox();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }
}
