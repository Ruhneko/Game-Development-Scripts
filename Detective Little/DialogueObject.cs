using UnityEngine;
[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private bool[] person;
    public string[] Dialogue => dialogue;
    public bool[] Person => person;
}
