using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using Util;

public class InGameHandler : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject car_prefab;
    [SerializeField] private GameObject loginPanel;
    [field: SerializeField] public GameObject retryButton { get; private set; }
    [SerializeField] private TextMeshProUGUI[] scoreRankingArray;
    private Dictionary<GameObject, SwipeGame_PlayerData>
        playerDictionary = new Dictionary<GameObject, SwipeGame_PlayerData>();
    private NetworkManager networkManager;
    private GameDirector gameDirector;
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(networkManager);
        gameDirector = FindObjectOfType<GameDirector>();
        Debug.Assert(gameDirector);
        
        Debug.Assert(scoreRankingArray.Length == 10, "scoreRankingArray배열에 10개의 TextMeshPro를 넣어주세요.");
        
        PacketHandler.SetHandler(PacketData.EPacketType.UserEnterInGame, OtherUserEnterInGame);
        PacketHandler.SetHandler(PacketData.EPacketType.UserExitInGame, OtherUserExitInGame);
        PacketHandler.SetHandler(PacketData.EPacketType.LoadOtherClient, LoadOtherClients);
        PacketHandler.SetHandler(PacketData.EPacketType.CarMove, CarMoveReceived);
        PacketHandler.SetHandler(PacketData.EPacketType.ReTryGame, ReTryGame);
        PacketHandler.SetHandler(PacketData.EPacketType.SendUserRank, SetUserRank);
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
        SwipeGame_PlayerData pd = new SwipeGame_PlayerData
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
    private GameObject CreateNewPlayer(SwipeGame_PlayerData pd)
    {
        // 자동차 생성 및 playerData(id) 초기화 진행
        GameObject car = Instantiate(car_prefab, spawnPos);
        car.GetComponent<CarController>().SetPlayerData(pd);
        
        playerDictionary.Add(car, pd);
        
        return car;
    }
    private void OtherUserEnterInGame(byte[] data) // playerData
    {
        SwipeGame_PlayerData pd = data.ChangeToPlayerData();

        MainThreadWorker.Instance.EnqueueJob(()=>CreateNewPlayer(pd));
    }
    private void OtherUserExitInGame(byte[] data) // id
    {
        string id = data.ChangeToString();
        Debug.Log($"ID: {id}님이 접속을 종료하였습니다.");

        var found = playerDictionary
            .FirstOrDefault(x => x.Value.id == id).Key;
        if (found == null) return;

        playerDictionary.Remove(found);
        MainThreadWorker.Instance.EnqueueJob(()=>Destroy(found));
    }
    private void LoadOtherClients(byte[] data) // PlayerData[]
    {
        int size = SwipeGame_PlayerData.GetByteSize();
        if (data.Length % size != 0)
        {
            return;
        }
        
        int count = data.Length / size;
        for (int i = 0; i < count; ++i)
        {
            int offset = i * size;
            byte[] playerDataBytes = new byte[size];
            Array.Copy(data, offset, playerDataBytes, 0, size);
            SwipeGame_PlayerData pd = playerDataBytes.ChangeToPlayerData();
            MainThreadWorker.Instance.EnqueueJob(()=>CreateNewPlayer(pd));
        }
    }
    // 서버에게 유저 인풋(시작과 끝)을 보낸다.
    public void SendCarMove(Vector2 startPos, Vector2 endPos)
    {
        byte[] startByte = BitConverter.GetBytes(startPos.x);
        byte[] endByte = BitConverter.GetBytes(endPos.x);
        byte[] packetData = PacketHandler.PackPacket(PacketData.EPacketType.RequestCarMove,startByte, endByte);
        networkManager.SendToServer(packetData);
    }
    private void CarMoveReceived(byte[] data) // magnitude, id(string)
    {
        float magnitude = BitConverter.ToSingle(data, 0);
        byte[] idBytes = new byte[16];
        Array.Copy(data, sizeof(float), idBytes, 0, 16);
        string id = idBytes.ChangeToString();

        foreach (var playerPair in playerDictionary)
        {
            if (playerPair.Value.id != id)
            {
                continue;
            }
            MainThreadWorker.Instance.EnqueueJob(()=>playerPair.Key.GetComponent<CarController>().Move(magnitude));
        }
    }
    public void RequireReTry()
    {
        retryButton.SetActive(false);
        byte[] packetData = PacketHandler.PackPacket(PacketData.EPacketType.RequireReTryGame);
        networkManager.SendToServer(packetData);
    }
    private void ReTryGame(byte[] data)
    {
        string id = data.ChangeToString();
        foreach (var playerPair in playerDictionary)
        {
            if (playerPair.Value.id != id)
            {
                continue;
            }

            MainThreadWorker.Instance.EnqueueJob(()=>
            {
                CarController carController = playerPair.Key.GetComponent<CarController>();
                carController.RetryGame();
                if (carController.IsHost())
                {
                    gameDirector.gameType = GameDirector.EGameType.InGame;
                }
            });
        }
    }
    private void SetUserRank(byte[] data)
    {
        int size = SwipeGame_GamePlayData.GetByteSize();
        if (data.Length % size != 0)
        {
            return;
        }
        
        int count = data.Length / size;
        List<SwipeGame_GamePlayData> gpdList = new List<SwipeGame_GamePlayData>(count);
        for (int i = 0; i < count; ++i)
        {
            int offset = i * size;
            byte[] playerDataBytes = new byte[size];
            Array.Copy(data, offset, playerDataBytes, 0, size);
            gpdList.Add(playerDataBytes.ChangeToGamePlayData());
        }
                    
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            for (int i = 0; i < gpdList.Count; ++i)
            {
                string temp =
                    $"{i + 1,-1}.\t{gpdList[i].Id.TrimEnd('\0'),-16}\t{gpdList[i].Length:F2}m \t{gpdList[i].DateTime}";
                scoreRankingArray[i].text = temp;
            }
        });
    }
}
