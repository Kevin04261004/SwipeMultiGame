using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class LoginHandler : MonoBehaviour
{
    private static readonly string ERRORCODE_SHORT_ID_OR_PW = "아이디와 비밀번호가 너무 짧습니다.";
    private static readonly string ERRORCODE_LONG_ID_OR_PW = "아이디와 비밀번호가 너무 깁니다.";
    private static readonly Color redFadeInColor = new Color(1, 0, 0, 0);
    
    private NetworkManager networkManager;
    [SerializeField] private TextMeshProUGUI id;
    [SerializeField] private TextMeshProUGUI pw;
    [SerializeField] private TextMeshProUGUI errorCode;
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(networkManager != null);
    }

    public void TryLogin()
    {
        if (id.text.Length < 2 || pw.text.Length < 2)
        {
            SetErrorCode(ERRORCODE_SHORT_ID_OR_PW);
            return;
        }
        if (id.text.Length < 16 || pw.text.Length < 16)
        {
            SetErrorCode(ERRORCODE_LONG_ID_OR_PW);
            return;
        }

        byte[] IdBytes = new byte[16];
        byte[] PasswordBytes = new byte[16];

        IdBytes = Encoding.UTF8.GetBytes(id.text.PadRight(16, '\0'));
        PasswordBytes = Encoding.UTF8.GetBytes(pw.text.PadRight(16, '\0'));

        byte[] loginPacket = PacketData.PackPacket(PacketData.EPacketType.RequireCreateUser, IdBytes, PasswordBytes);
    }

    public void CreateUser()
    {
        if (id.text.Length < 2 || pw.text.Length < 2)
        {
            SetErrorCode(ERRORCODE_SHORT_ID_OR_PW);
            return;
        }
        if (id.text.Length < 16 || pw.text.Length < 16)
        {
            SetErrorCode(ERRORCODE_LONG_ID_OR_PW);
            return;
        }
        
    }

    private void SetErrorCode(string str)
    {
        errorCode.text = str;
        StartCoroutine(ErrorCodeFadeIn(1f));
    }
    private IEnumerator ErrorCodeFadeIn(float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            // Calculate the interpolation factor
            float t = elapsedTime / time;
            
            // Lerp between the start color and the target color
            errorCode.color = Color.Lerp(Color.red, redFadeInColor, t);
            
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;
            
            // Wait for the next frame
            yield return null;
        }
        
        // Ensure the final color is exactly the target color
        errorCode.color = Color.clear;
    }
}
