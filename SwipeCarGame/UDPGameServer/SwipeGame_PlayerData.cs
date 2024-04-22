using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPGameServer
{
    public class SwipeGame_PlayerData
    {
        public static readonly int ID_SIZE = 16;
        public static readonly int NICKNAME_SIZE = 16;

        public string id { get; set; }
        public string nickName { get; set; }
        public int level { get; set; }
        public float highScore { get; set; }
        public float lastConnectTime { get; set; }

        public static int GetByteSize()
        {
            int size = ID_SIZE + NICKNAME_SIZE + sizeof(int) + sizeof(float) + sizeof(float);

            return size;
        }
    }

    public static class PlayerDataExtension
    {
        public static byte[] ChangeToBytes(this SwipeGame_PlayerData playerData)
        {
            int size = SwipeGame_PlayerData.GetByteSize();
            byte[] data = new byte[size];

            // offset을 사용하여 각 필드의 시작 위치를 조정하여 데이터를 바이트 배열에 채웁니다.
            int offset = 0;
            Encoding.UTF8.GetBytes(playerData.id, 0, Math.Min(SwipeGame_PlayerData.ID_SIZE, playerData.id.Length), data, offset);
            offset += SwipeGame_PlayerData.ID_SIZE;
            Encoding.UTF8.GetBytes(playerData.nickName, 0, Math.Min(SwipeGame_PlayerData.NICKNAME_SIZE, playerData.nickName.Length), data, offset);
            offset += SwipeGame_PlayerData.NICKNAME_SIZE;
            BitConverter.GetBytes(playerData.level).CopyTo(data, offset);
            offset += sizeof(int);
            BitConverter.GetBytes(playerData.highScore).CopyTo(data, offset);
            offset += sizeof(float);
            BitConverter.GetBytes(playerData.lastConnectTime).CopyTo(data, offset);

            return data;
        }
    }
}
