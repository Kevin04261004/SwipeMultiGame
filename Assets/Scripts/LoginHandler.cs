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
    private static readonly string ERRORCODE_LOGIN_FAIL = "로그인이 실패하였습니다. ID와 PASSWORD가 존재하지 않습니다.";
    private static readonly Color redFadeInColor = new Color(1, 0, 0, 0);
    
    private NetworkManager networkManager;
    [SerializeField] private TextMeshProUGUI id;
    [SerializeField] private TextMeshProUGUI pw;
    [SerializeField] private TextMeshProUGUI errorCode;
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(networkManager != null);

        PacketHandler.SetHandler(PacketData.EPacketType.UserLoginSuccess, Login);
        PacketHandler.SetHandler(PacketData.EPacketType.UserLoginFail, LoginFail);
    }

    public void TryLogin()
    {
        if (id.text.Length < 2 || pw.text.Length < 2)
        {
            SetErrorCode(ERRORCODE_SHORT_ID_OR_PW);
            return;
        }
        if (id.text.Length > 16 || pw.text.Length > 16)
        {
            SetErrorCode(ERRORCODE_LONG_ID_OR_PW);
            return;
        }
        
        // 16 + 16 byte의 id와 password byte를 만들고
        byte[] IdBytes = new byte[16];
        byte[] PasswordBytes = new byte[16];
        
        // 인코딩을 통해 byte로 변환
        IdBytes = Encoding.UTF8.GetBytes(id.text.PadRight(16, '\0'));
        PasswordBytes = Encoding.UTF8.GetBytes(pw.text.PadRight(16, '\0'));

        // 패킷으로 패킹해준다.
        byte[] loginPacket = PacketHandler.PackPacket(PacketData.EPacketType.RequireUserLogin, IdBytes, PasswordBytes);
        networkManager.SendToServer(loginPacket);
    }

    private void LoginFail()
    {
        SetErrorCode(ERRORCODE_LOGIN_FAIL, 2);
    }

    private void Login()
    {
        Debug.Log("로그인에 성공하였습니다.");
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

    private void SetErrorCode(string str, float time = 1)
    {
        errorCode.text = str;
        StartCoroutine(ErrorCodeFadeIn(time));
    }
    private IEnumerator ErrorCodeFadeIn(float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            float t = elapsedTime / time;
            
            errorCode.color = Color.Lerp(Color.red, redFadeInColor, t);
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        errorCode.color = Color.clear;
    }
}
