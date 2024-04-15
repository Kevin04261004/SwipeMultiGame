using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PacketData
{
    public enum EPacketType
    {
        /* ServerToClient */


        /* ClientToServer */
        RequireUserLogin = 1000, // 16byte + 16byte 총 32byte를 보낸다.
        RequireCreateUser, // 16byte + 16byte 총 32byte를 보낸다.
    };
    
    public static byte[] PackPacket(EPacketType packetType, byte[] bytes)
    {
        // Byte Size (int - 4byte)
        // Packet Type (enum - 4byte)
        // + Byte (byte)
        int size = sizeof(int) + sizeof(EPacketType) + bytes.Length;
        byte[] sizeBytes = BitConverter.GetBytes(size);
        byte[] packetTypeBytes = BitConverter.GetBytes((int)packetType);

        // add all bytes
        byte[] result = new byte[size];

        int offset = 0;
        Array.Copy(sizeBytes, 0, result, offset, sizeof(int));
        offset += sizeof(int);
        Array.Copy(packetTypeBytes, 0, result, offset, sizeof(EPacketType));
        offset += sizeof(EPacketType);
        Array.Copy(bytes, 0, result, offset, bytes.Length);

        return result;
    }
    public static byte[] PackPacket(EPacketType packetType, byte[] idBytes, byte[] passwordBytes)
    {
        // Byte Size (int - 4byte)
        // Packet Type (enum - 4byte)
        // + ID (16byte)
        // + Password (16byte)
        int size = sizeof(int) + sizeof(int) + idBytes.Length + passwordBytes.Length;
        byte[] sizeBytes = BitConverter.GetBytes(size);
        byte[] packetTypeBytes = BitConverter.GetBytes((int)packetType);
        
        // add all bytes
        byte[] result = new byte[size];
        
        int offset = 0;
        Array.Copy(sizeBytes, 0, result, offset, sizeof(int));
        offset += sizeof(int);
        Array.Copy(packetTypeBytes, 0, result, offset, sizeof(int));
        offset += sizeof(int);
        Array.Copy(idBytes, 0, result, offset, idBytes.Length);
        offset += idBytes.Length;
        Array.Copy(passwordBytes, 0, result, offset, passwordBytes.Length);

        return result;
    }
}
