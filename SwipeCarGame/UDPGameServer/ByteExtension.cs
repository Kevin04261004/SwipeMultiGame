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

            // offset을 사용하여 바이트 배열에서 각 필드의 데이터를 추출합니다.
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
    }
}
