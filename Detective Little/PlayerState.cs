using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] private Hashtable eventTriggers = new Hashtable();

    // Start is called before the first frame update
    void Start()
    {
        eventTriggers.Clear();
    }

    public void AddCode(string code)
    {
        eventTriggers.Add(code, true);
    }

    public bool CodeTriggered(string code)
    {
        return eventTriggers.ContainsKey(code);
    }
}
