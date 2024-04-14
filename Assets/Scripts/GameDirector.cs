using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject flag;
    [SerializeField] private GameObject distance;

    private TextMeshProUGUI distanceTMP;

    private void Start()
    {
        distanceTMP = distance.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float length = flag.transform.position.x - car.transform.position.x;
        if (length >= 0)
        {
            distanceTMP.text = "Distance from flag is " + length.ToString("F2") + "m";
        }
        else
        {
            distanceTMP.text = "GameOver";
        }
    }
}
