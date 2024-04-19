using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Util;

public class InGameHandler : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject car_prefab;
    [SerializeField] private GameObject loginPanel;
    private List<PlayerData> playerList;
    private NetworkManager networkManager;
    private GameDirector gameDirector;
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(networkManager);
        gameDirector = FindObjectOfType<GameDirector>();
        Debug.Assert(gameDirector);
    }

    public void StartGame(byte[] data)
    {
        // 나중에 data에서 클라이언트의 정보를 얻어와 이곳에서 초기화를 진행해줘도 좋을 것 같다.

        byte[] idBytes = new byte[16];
        byte[] nickNameBytes = new byte[data.Length - 16];

        int offset = 0;
        Array.Copy(data, 0, idBytes, 0, idBytes.Length);
        offset += idBytes.Length;
        Array.Copy(data, offset, nickNameBytes, 0, nickNameBytes.Length);
        string id = idBytes.ChangeToString();
        string nickName = nickNameBytes.ChangeToString();

        // host id 세팅
        networkManager.hostId = id;
        
        // 플레이어 데이터 초기화
        PlayerData pd = new PlayerData
        {
            id = id,
            nickName = nickName
        };
        
        // 플레이어 데이터로 캐릭터 생성
        GameObject hostCar = CreateNewPlayer(pd);
        loginPanel.SetActive(false);
        gameDirector.car = hostCar;
        gameDirector.gameType = GameDirector.EGameType.InGame;
    }

    private GameObject CreateNewPlayer(PlayerData pd)
    {
        // 자동차 생성 및 playerData(id) 초기화 진행
        GameObject car = Instantiate(car_prefab, spawnPos);
        car.GetComponent<CarController>().SetPlayerData(pd);
        return car;
    }
    
    public void SetUserScore()
    {
        
    }
}
