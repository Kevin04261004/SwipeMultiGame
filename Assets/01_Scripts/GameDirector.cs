using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public enum EGameType
    {
        Login,
        InGame,
        GameEnd,
        Pause,
    };


    public GameObject car { get; set; }
    [SerializeField] private GameObject flag;
    [SerializeField] private GameObject distance;
    private TextMeshProUGUI distanceTMP;
    [field:ReadOnly(false)] [field:SerializeField] public EGameType gameType { get; set; } = EGameType.Login;
    private void Awake()
    {
        distanceTMP = distance.GetComponent<TextMeshProUGUI>();
    }
    
    private void Update()
    {
        switch (gameType)
        {
            case EGameType.Login:
                break;
            case EGameType.InGame:
                float length = flag.transform.position.x - car.transform.position.x;
                if (length >= 0)
                {
                    distanceTMP.text = "Distance from flag is " + length.ToString("F2") + "m";
                }
                else
                {
                    distanceTMP.text = "GameOver";
                }
                break;
            case EGameType.GameEnd:
                distanceTMP.text = "GameEnd!!!\nPress Button to ReTry.";
                break;
            case EGameType.Pause:
                break;
            default:
                Debug.LogError("Switch Out Of Case");
                break;
        }
    }

    private void OnApplicationQuit()
    {
        
    }
}
