using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginHandler : MonoBehaviour
{
    private NetworkManager networkManager;
    [SerializeField] private TextMeshProUGUI id;
    [SerializeField] private TextMeshProUGUI pw;
    
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(networkManager != null);
    }

    public void TryLogin()
    {
        if(id.text.)
    }

    public void CreateUser()
    {
        
    }
    
}
