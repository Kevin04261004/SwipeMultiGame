using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    private GameObject car;
    private GameObject flag;
    private GameObject distance;

    private TextMeshProUGUI distanceTMP;

    private void Start()
    {
        this.car = GameObject.Find("car");
        this.flag = GameObject.Find("flag");
        this.distance = GameObject.Find("Distance");
        this.distanceTMP = this.distance.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float length = this.flag.transform.position.x - this.car.transform.position.x;
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
