using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPTextDisplay : MonoBehaviour
{
    public TMP_Text text;
    public Status status;
    public void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
    public void Update()
    {
        text.text = status.HP.ToString();
    }
}
