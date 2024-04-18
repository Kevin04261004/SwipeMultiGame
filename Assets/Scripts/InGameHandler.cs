using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class InGameHandler : MonoBehaviour
{
    private Player _player;
    private NetworkManager networkManager;
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(networkManager);
        
        
    }

    public void StartGame(byte[] data)
    {
        // 나중에 data에서 클라이언트의 정보를 얻어와 이곳에서 초기화를 진행해줘도 좋을 것 같다.

        string nickName = Encoding.UTF8.GetString(data);
        
    }

    public void SetUserScore()
    {
        
    }
}
