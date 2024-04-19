﻿using System;
using System.Collections;
using System.Collections.Generic;

public static class PacketData
{
    public enum EPacketType
    {
        /* ServerToClient */
        UserLoginFail = 0, // DB에 유저 정보가 존재하지 않을 때
        UserLoginSuccess, // DB에 유저 정보가 존재할 때
        AllowCreateUserData, // 유저 데이터의 생성을 허용할 때
        CantCreateUserData, // 유저 데이터를 생성하지 못하게 할 때 (== 이미 존재할 때)
        UserCreateSuccess, // 유저(계정)를 생성에 성공하였을 때
        UserCreateFail, // 유저(계정)를 생성에 실패'하였을 때
        newUserCraeted, // 새로운 유저가 접속했음을 클라이언트에게 알림.

        /* ClientToServer */
        RequireUserLogin = 1000, // 16byte + 16byte 총 32byte를 보낸다.
        RequireCreateUser, // 16byte + 16byte 총 32byte를 보낸다.
        ConnectClient, // UDP는 ReceiveFrom을 하기 위해서 한번 클라이언트가 서버에게 데이터를 보내야만한다.
        DisconnectClient, // 클라이언트가 종료되었을때 서버에게 자기 빼달라고 하기.
        CreateUser, // 새로운 유저(계정)을 생성합니다.
    };
}