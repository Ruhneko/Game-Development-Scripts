using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    [SerializeField] private Text dialog;
    [SerializeField] private PlayerState state;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform notepad;
    private ArrayList notes;
    private GameObject prefab;

    void Awake()
    {
        EventBroadcaster.Instance.AddObserver(
            EventNames.CHANGE_DIALOG,
            ChangeDialog
        );
    }

    void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveObserver(
            EventNames.CHANGE_DIALOG
        );
    }

    void Start()
    {
        gameObject.SetActive(false);
        notes = new ArrayList();
    }

    // item: name of the item
    // msg: meesage to display
    // code: what code is added
    private void ChangeDialog(Parameters param)
    {
        string item = param.GetStringExtra("item", "");
        string msg = param.GetStringExtra("msg", "");
        string code = param.GetStringExtra("code", "");

        dialog.text = msg;

        if (code != "")
        {
            prefab = GameObject.Instantiate(notePrefab, notepad);
            Notes n = prefab.GetComponent<Notes>();
            n.SetText(code);
            // n.SetIcon();
            notes.Add(prefab);
            state.AddCode(code);
            
        }
        gameObject.SetActive(true);
        StartCoroutine(clearDialogue());
    }
    private IEnumerator clearDialogue(){
        yield return new WaitForSeconds(5);
        gameObject.SetActive(false);
    }
}
