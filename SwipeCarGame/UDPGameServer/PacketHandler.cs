using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace UDPGameServer
{
    public static partial class PacketHandler
    {
        public delegate void PacketHandlerEvent(IPEndPoint endPoint, byte[] data);

        private static Dictionary<PacketData.EPacketType, PacketHandlerEvent> packetHandlerEvents =
            new Dictionary<PacketData.EPacketType, PacketHandlerEvent>();

        static PacketHandler()
        {
            // Enum의 각 값에 대해 델리게이트 추가
            foreach (PacketData.EPacketType packetType in Enum.GetValues(typeof(PacketData.EPacketType)))
            {
                packetHandlerEvents.Add(packetType, null); // 기본값은 null로 초기화
            }
            SetAllHandlers();
        }

        // Set
        public static void SetHandler(PacketData.EPacketType packetType, PacketHandlerEvent handler)
        {
            packetHandlerEvents[packetType] = handler;
        }

        public static void ProcessPacket(IPEndPoint endPoint, byte[] packetBuffer, int packetSize)
        {
            byte[] data = new byte[packetSize - sizeof(int) - sizeof(PacketData.EPacketType)];
            PacketData.EPacketType packetType = (PacketData.EPacketType)BitConverter.ToInt32(packetBuffer, sizeof(int));
            Array.Copy(packetBuffer, sizeof(int) + sizeof(PacketData.EPacketType), data, 0, data.Length);
            Console.WriteLine($"Received packet Size: {sizeof(int) + sizeof(PacketData.EPacketType) + data.Length}, PacketType = {packetType}");

            if (packetHandlerEvents.ContainsKey(packetType) && packetHandlerEvents[packetType] != null)
            {
                packetHandlerEvents[packetType](endPoint, data);
            }
            else
            {
                Console.WriteLine($"No handler set for packet type: {packetType}");
            }
        }

        public static byte[] PackPacket(PacketData.EPacketType packetType)
        {
            // Byte Size (int - 4byte)
            // Packet Type (enum - 4byte)
            int size = sizeof(int) + sizeof(PacketData.EPacketType);
            byte[] sizeBytes = BitConverter.GetBytes(size);
            byte[] packetTypeBytes = BitConverter.GetBytes((int)packetType);

            // add all bytes
            byte[] result = new byte[size];

            int offset = 0;
            Array.Copy(sizeBytes, 0, result, offset, sizeof(int));
            offset += sizeof(int);
            Array.Copy(packetTypeBytes, 0, result, offset, sizeof(PacketData.EPacketType));

            return result;
        }
        public static byte[] PackPacket(PacketData.EPacketType packetType, byte[] bytes)
        {
            // Byte Size (int - 4byte)
            // Packet Type (enum - 4byte)
            // + Byte (byte)
            int size = sizeof(int) + sizeof(PacketData.EPacketType) + bytes.Length;
            byte[] sizeBytes = BitConverter.GetBytes(size);
            byte[] packetTypeBytes = BitConverter.GetBytes((int)packetType);

            // add all bytes
            byte[] result = new byte[size];

            int offset = 0;
            Array.Copy(sizeBytes, 0, result, offset, sizeof(int));
            offset += sizeof(int);
            Array.Copy(packetTypeBytes, 0, result, offset, sizeof(PacketData.EPacketType));
            offset += sizeof(PacketData.EPacketType);
            Array.Copy(bytes, 0, result, offset, bytes.Length);

            return result;
        }
        public static byte[] PackPacket(PacketData.EPacketType packetType, byte[] byte1, byte[] byte2)
        {
            // Byte Size (int - 4byte)
            // Packet Type (enum - 4byte)
            // + ID (16byte)
            // + Password (16byte)
            int size = sizeof(int) + sizeof(int) + byte1.Length + byte2.Length;
            byte[] sizeBytes = BitConverter.GetBytes(size);
            byte[] packetTypeBytes = BitConverter.GetBytes((int)packetType);

            // add all bytes
            byte[] result = new byte[size];

            int offset = 0;
            Array.Copy(sizeBytes, 0, result, offset, sizeof(int));
            offset += sizeof(int);
            Array.Copy(packetTypeBytes, 0, result, offset, sizeof(int));
            offset += sizeof(int);
            Array.Copy(byte1, 0, result, offset, byte1.Length);
            offset += byte1.Length;
            Array.Copy(byte2, 0, result, offset, byte2.Length);

            return result;
        }
        public static byte[] PackPacket(PacketData.EPacketType packetType, byte[] byte1, byte[] byte2, byte[] byte3)
        {
            // Byte Size (int - 4byte)
            // Packet Type (enum - 4byte)
            // + ID (16byte)
            // + Password (16byte)
            int size = sizeof(int) + sizeof(PacketData.EPacketType) + byte1.Length + byte2.Length + byte3.Length;
            byte[] sizeBytes = BitConverter.GetBytes(size);
            byte[] packetTypeBytes = BitConverter.GetBytes((int)packetType);

            // add all bytes
            byte[] result = new byte[size];

            int offset = 0;
            Array.Copy(sizeBytes, 0, result, offset, sizeof(int));
            offset += sizeof(int);
            Array.Copy(packetTypeBytes, 0, result, offset, sizeof(int));
            offset += sizeof(int);
            Array.Copy(byte1, 0, result, offset, byte1.Length);
            offset += byte1.Length;
            Array.Copy(byte2, 0, result, offset, byte2.Length);
            offset += byte2.Length;
            Array.Copy(byte3, 0, result, offset, byte3.Length);

            return result;
        }
    }
}
