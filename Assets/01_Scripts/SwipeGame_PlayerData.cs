using System;
using System.Text;

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
    public static byte[] ChangeToBytes(this SwipeGame_PlayerData swipeGamePlayerData)
    {
        int size = 16 + 16 + sizeof(int) + sizeof(float) + sizeof(float);
        byte[] data = new byte[size];

        // offset을 사용하여 각 필드의 시작 위치를 조정하여 데이터를 바이트 배열에 채웁니다.
        int offset = 0;
        Encoding.UTF8.GetBytes(swipeGamePlayerData.id, 0, Math.Min(16, swipeGamePlayerData.id.Length), data, offset);
        offset += 16;
        Encoding.UTF8.GetBytes(swipeGamePlayerData.nickName, 0, Math.Min(16, swipeGamePlayerData.nickName.Length), data, offset);
        offset += 16;
        BitConverter.GetBytes(swipeGamePlayerData.level).CopyTo(data, offset);
        offset += sizeof(int);
        BitConverter.GetBytes(swipeGamePlayerData.highScore).CopyTo(data, offset);
        offset += sizeof(float);
        BitConverter.GetBytes(swipeGamePlayerData.lastConnectTime).CopyTo(data, offset);

        return data;
    }
}
