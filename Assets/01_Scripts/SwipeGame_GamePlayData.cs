using System;
using System.Text;

public class SwipeGame_GamePlayData
{
    public static readonly int NICKNAME_SIZE = 16;
    public static readonly int DATETIME_TO_STRING_SIZE = 32;
    public string NickName { get; set; }
    public float Length { get; set; }
    public DateTime DateTime { get; set; }

    public SwipeGame_GamePlayData(string id, float length, DateTime dt)
    {
        NickName = id;
        Length = length;
        DateTime = dt;
    }
    public SwipeGame_GamePlayData(string id, float length, string dt)
    {
        NickName = id;
        Length = length;
        DateTime = DateTime.Parse(dt);
    }
    public static int GetByteSize()
    {
        int size = NICKNAME_SIZE + sizeof(float) + DATETIME_TO_STRING_SIZE;

        return size;
    }
}
public static class GamePlayDataExtension
{
    public static byte[] ChangeToBytes(this SwipeGame_GamePlayData gamePlayData)
    {
        int size = SwipeGame_PlayerData.GetByteSize();
        byte[] data = new byte[size];

        // offset을 사용하여 각 필드의 시작 위치를 조정하여 데이터를 바이트 배열에 채웁니다.
        int offset = 0;
        Encoding.UTF8.GetBytes(gamePlayData.NickName, 0, Math.Min(SwipeGame_GamePlayData.NICKNAME_SIZE, gamePlayData.NickName.Length), data, offset);
        offset += SwipeGame_PlayerData.ID_SIZE;
        BitConverter.GetBytes(gamePlayData.Length).CopyTo(data, offset);
        offset += sizeof(float);
        string dateTime = gamePlayData.DateTime.ToString();
        Encoding.UTF8.GetBytes(dateTime, 0, Math.Min(SwipeGame_GamePlayData.DATETIME_TO_STRING_SIZE, dateTime.Length), data, offset);

        return data;
    }
}
