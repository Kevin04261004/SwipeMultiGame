using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPGameServer
{
    public static class ByteExtension
    {
        public static string ChangeToString(this byte[] bytes)
        {
            // 변환할때 Encoding을 맞춰주기 위함.
            return Encoding.UTF8.GetString(bytes);
        }
        public static SwipeGame_PlayerData ChangeToPlayerData(this byte[] data)
        {
            SwipeGame_PlayerData playerData = new SwipeGame_PlayerData();

            int offset = 0;
            playerData.id = Encoding.UTF8.GetString(data, offset, SwipeGame_PlayerData.ID_SIZE);
            offset += SwipeGame_PlayerData.ID_SIZE;
            playerData.nickName = Encoding.UTF8.GetString(data, offset, SwipeGame_PlayerData.NICKNAME_SIZE);
            offset += SwipeGame_PlayerData.NICKNAME_SIZE;
            playerData.level = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);
            playerData.highScore = BitConverter.ToSingle(data, offset);
            offset += sizeof(float);
            playerData.lastConnectTime = BitConverter.ToSingle(data, offset);

            return playerData;
        }
        public static SwipeGame_GamePlayData ChangeToGamePlayData(this byte[] data)
        {
            int offset = 0;
            string id = Encoding.UTF8.GetString(data, offset, SwipeGame_GamePlayData.NICKNAME_SIZE);
            offset += SwipeGame_GamePlayData.NICKNAME_SIZE;
            float length = BitConverter.ToSingle(data, offset);
            offset += sizeof(float);
            string dt = Encoding.UTF8.GetString(data, offset, SwipeGame_GamePlayData.DATETIME_TO_STRING_SIZE);
            SwipeGame_GamePlayData gameplayData = new SwipeGame_GamePlayData(id, length, dt);

            return gameplayData;
        }
    }
}
