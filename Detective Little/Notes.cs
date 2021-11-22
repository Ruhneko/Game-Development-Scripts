using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notes : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Image icon;

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}
