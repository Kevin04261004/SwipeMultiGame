using System;
using System.Text;

namespace Util
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
            SwipeGame_PlayerData swipeGamePlayerData = new SwipeGame_PlayerData();

            // offset을 사용하여 바이트 배열에서 각 필드의 데이터를 추출합니다.
            int offset = 0;
            swipeGamePlayerData.id = Encoding.UTF8.GetString(data, offset, 16);
            offset += 16;
            swipeGamePlayerData.nickName = Encoding.UTF8.GetString(data, offset, 16);
            offset += 16;
            swipeGamePlayerData.level = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);
            swipeGamePlayerData.highScore = BitConverter.ToSingle(data, offset);
            offset += sizeof(float);
            swipeGamePlayerData.lastConnectTime = BitConverter.ToSingle(data, offset);

            return swipeGamePlayerData;
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
