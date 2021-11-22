using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerInteract : MonoBehaviour
{
    [SerializeField] private Text dialog;
    private ItemScript triggeringObj;
    private bool triggering;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && triggering)
        {
            Parameters p = new Parameters();
            p.PutExtra("item", triggeringObj.getItem());
            p.PutExtra("msg", triggeringObj.getMsg());
            p.PutExtra("code", triggeringObj.getCode());

            EventBroadcaster.Instance.PostEvent(EventNames.CHANGE_DIALOG, p);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            triggering = true;
            triggeringObj = other.gameObject.GetComponent<ItemScript>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item")
        {
            triggering = false;
            triggeringObj = null;
        }
    }
}
