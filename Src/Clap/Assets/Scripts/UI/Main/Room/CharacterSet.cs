using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSet : MonoBehaviour
{
    public TMP_InputField Count;
    public TMP_InputField HP;
    public TMP_InputField Bullets;
    public TMP_InputField Swords;


    [SerializeField] private int minCount = 1;
    [SerializeField] private int maxCount = 10;
    public int CurrentCount
    {
        get => _currentCount;
        set
        {
            _currentCount = Mathf.Clamp(value, minCount, maxCount);
            Count.text = _currentCount.ToString();
        }
    }
    private int _currentCount;

    void Start()
    {
    }
}
