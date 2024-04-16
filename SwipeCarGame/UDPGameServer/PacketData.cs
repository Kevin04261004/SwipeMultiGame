using System;
using System.Collections;
using System.Collections.Generic;

public static class PacketData
{
    public enum EPacketType
    {
        /* ServerToClient */
        UserLoginFail = 0,
        UserLoginSuccess,
        ConnectUDP, // UDP는 ReceiveFrom을 하기 위해서 한번 클라이언트가 서버에게 데이터를 보내야만한다.

        /* ClientToServer */
        RequireUserLogin = 1000, // 16byte + 16byte 총 32byte를 보낸다.
        RequireCreateUser, // 16byte + 16byte 총 32byte를 보낸다.
    };
}
