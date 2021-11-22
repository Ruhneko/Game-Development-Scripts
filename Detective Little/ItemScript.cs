using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField] private string item;
    [SerializeField] private string msg;
    [SerializeField] private string code;

    public string getItem()
    {
        return item;
    }

    public string getMsg()
    {
        return msg;
    }

    public string getCode()
    {
        return code;
    }
}
