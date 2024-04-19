using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Util;

public class LoginHandler : MonoBehaviour
{
    private static readonly string ERRORCODE_SHORT_ID_OR_PW = "아이디와 비밀번호가 너무 짧습니다.";
    private static readonly string ERRORCODE_LONG_ID_OR_PW = "아이디와 비밀번호가 너무 깁니다.";
    private static readonly string ERRORCODE_LOGIN_FAIL = "로그인이 실패하였습니다. ID와 PASSWORD가 존재하지 않습니다.";
    private static readonly string ERRORCODE_CREATE_USER_NON_ALLOWED = "ID가 이미 존재합니다. (또는 어떠한 이유로 계정을 만들 수 없습니다.)";
    private static readonly string GOODCODE_CREATE_USER_ALLOWED = "계정을 생성할 수 있습니다. 닉네임을 입력하세요.";
    private static readonly string ERRORCODE_USER_NICKNAME_LENGTH = "닉네임이 너무 짧거나 깁니다.";
    private static readonly string ERRORCODE_CREATE_USER_FAIL = "계정 생성에 실패하였습니다.";
    private static readonly Color redFadeInColor = new Color(1, 0, 0, 0);

    private InGameHandler inGameHandler;
    private NetworkManager networkManager;
    [SerializeField] private TextMeshProUGUI id;
    [SerializeField] private TextMeshProUGUI pw;
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private TextMeshProUGUI errorCode;
    [SerializeField] private GameObject CreateNickNamePanel;
    private Coroutine fadeInCoroutine;
    
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(networkManager != null);

        inGameHandler = FindObjectOfType<InGameHandler>();
        
        PacketHandler.SetHandler(PacketData.EPacketType.UserLoginSuccess, Login);
        PacketHandler.SetHandler(PacketData.EPacketType.UserLoginFail, LoginFail);
        PacketHandler.SetHandler(PacketData.EPacketType.AllowCreateUserData, CreateUserDataAllowed);
        PacketHandler.SetHandler(PacketData.EPacketType.CantCreateUserData, CreateUserDataNonAllowed);
        PacketHandler.SetHandler(PacketData.EPacketType.UserCreateFail, UserCreateSuccess);
        PacketHandler.SetHandler(PacketData.EPacketType.UserCreateSuccess, UserCreateFail);
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
        print($"idByte:{IdBytes.Length}, PasswordByte:{PasswordBytes.Length}");

        // 인코딩을 통해 byte로 변환
        IdBytes = id.text.PadRight(16, '\0').ChangeToByte();
        PasswordBytes = pw.text.PadRight(16, '\0').ChangeToByte();
        
        // id와 password의 바이트 배열을 16바이트로 자릅니다.
        Array.Resize(ref IdBytes, 16);
        Array.Resize(ref PasswordBytes, 16);
        print($"idByte:{IdBytes.Length}, PasswordByte:{PasswordBytes.Length}");
        
        // 패킷으로 패킹해준다.
        byte[] loginPacket = PacketHandler.PackPacket(PacketData.EPacketType.RequireUserLogin, IdBytes, PasswordBytes);
        networkManager.SendToServer(loginPacket);
    }

    private void LoginFail(byte[] data = null)
    {
        Debug.Log("로그인에 실패하였습니다.");
        MainThreadWorker.Instance.EnqueueJob(()=>SetErrorCode(ERRORCODE_LOGIN_FAIL, 3));
    }

    private void Login(byte[] data) // data = id(16), nickName
    {
        Debug.Log("로그인에 성공하였습니다.");
        
        MainThreadWorker.Instance.EnqueueJob(() => inGameHandler.StartGame(data));
    }

    private void CreateUserDataAllowed(byte[] data = null)
    {
        Debug.Log("유저가 새로운 데이터를 만들 수 있습니다.");
        MainThreadWorker.Instance.EnqueueJob(()=>SetErrorCode(GOODCODE_CREATE_USER_ALLOWED, 3, Color.green));
        MainThreadWorker.Instance.EnqueueJob(()=>CreateNickNamePanel.SetActive(true));
    }

    private void CreateUserDataNonAllowed(byte[] data = null)
    {
        Debug.Log("유저가 새로운 데이터를 만들 수 없습니다.");
        MainThreadWorker.Instance.EnqueueJob(()=>SetErrorCode(ERRORCODE_CREATE_USER_NON_ALLOWED, 3));
    }

    public void RequireCreateUser()
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
        IdBytes = id.text.PadRight(16, '\0').ChangeToByte();
        PasswordBytes = pw.text.PadRight(16, '\0').ChangeToByte();
            
        // id와 password의 바이트 배열을 16바이트로 자릅니다.
        Array.Resize(ref IdBytes, 16);
        Array.Resize(ref PasswordBytes, 16);
        
        // 패킷으로 패킹해준다.
        byte[] loginPacket = PacketHandler.PackPacket(PacketData.EPacketType.RequireCreateUser, IdBytes, PasswordBytes);
        networkManager.SendToServer(loginPacket);
    }

    public void CreateUser()
    {
        if (id.text.Length < 2 || pw.text.Length < 2)
        {
            SetErrorCode(ERRORCODE_SHORT_ID_OR_PW);
            return;
        }
        if (id.text.Length > 16 || pw.text.Length > 16 )
        {
            SetErrorCode(ERRORCODE_LONG_ID_OR_PW);
            return;
        }

        if (nickName.text.Length < 2 || nickName.text.Length > 16)
        {
            SetErrorCode(ERRORCODE_USER_NICKNAME_LENGTH, 3);
            return;
        }
        // 16 + 16 byte의 id와 password byte를 만들고
        byte[] IdBytes = new byte[16];
        byte[] PasswordBytes = new byte[16];
        byte[] NickNameBytes = new byte[16];

        // 인코딩을 통해 byte로 변환
        IdBytes = id.text.PadRight(16, '\0').ChangeToByte();
        PasswordBytes = pw.text.PadRight(16, '\0').ChangeToByte();
        NickNameBytes = nickName.text.PadRight(16, '\0').ChangeToByte();
        
        // id와 password, nickName의 바이트 배열을 16바이트로 자릅니다.
        Array.Resize(ref IdBytes, 16);
        Array.Resize(ref PasswordBytes, 16);
        Array.Resize(ref NickNameBytes, 16);
        
        // 패킷으로 패킹해준다.
        byte[] newUserBytes = PacketHandler.PackPacket(PacketData.EPacketType.CreateUser, IdBytes, PasswordBytes, NickNameBytes);
        networkManager.SendToServer(newUserBytes);
    }
    private void UserCreateSuccess(byte[] data) // data = id(16), nickName
    {
        Debug.Log("유저가 계정을 생성하였습니다.");
        
        MainThreadWorker.Instance.EnqueueJob(() => inGameHandler.StartGame(data));
    }
    private void UserCreateFail(byte[] data = null)
    {
        Debug.Log("유저가 계정을 생성에 실패하였습니다.");
        MainThreadWorker.Instance.EnqueueJob(()=>SetErrorCode(ERRORCODE_CREATE_USER_FAIL, 3));
    }

    private void SetErrorCode(string str, float time = 1, Color color = default(Color))
    {
        errorCode.text = str;
        errorCode.color = (color == default(Color)) ? Color.red : color;
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = null;
        }
        fadeInCoroutine = StartCoroutine(ErrorCodeFadeIn(time, color));
    }
    private IEnumerator ErrorCodeFadeIn(float time, Color color = default(Color))
    {
        yield return new WaitForSeconds(time - 1);
        
        float elapsedTime = 0f;
        while (elapsedTime < 1)
        {
            float t = elapsedTime / 1;

            if (color == default(Color))
            {
                errorCode.color = Color.Lerp(Color.red, redFadeInColor, t);
            }
            else
            {
                Color fadeInColor = new Color(color.r, color.g, color.b, 0);
                errorCode.color = Color.Lerp(color, fadeInColor, t);   
            }
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        errorCode.color = Color.clear;
    }
}
