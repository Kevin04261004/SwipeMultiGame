using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace UDPGameServer
{
    public static class ServerHandler
    {
        // 추후 잡 타이머 방식을 통해 리스트에 존재하는 모든 클라이언트들이 튕기지는 않았는지? 접속이 이상한지는 않은지 확인해보기.
        public static List<IPEndPoint> connectedClients = new List<IPEndPoint>();
        public static Dictionary<IPEndPoint, SwipeGame_PlayerData> inGameClients = new Dictionary<IPEndPoint, SwipeGame_PlayerData>();
        public static Socket serverSocket;
        public static void SendToClientList(byte[] bytes)
        {
            if (serverSocket == null)
            {
                return;
            }
            foreach (IPEndPoint client in connectedClients)
            {
                serverSocket.SendTo(bytes, client);
            }
        }
        public static void SendToInGameClientList(byte[] bytes)
        {
            if (serverSocket == null)
            {
                return;
            }
            foreach(var pair in inGameClients)
            {
                serverSocket.SendTo(bytes, pair.Key);
            }
        }
        public static void SendToInGameClientListExcludeEndPoint(byte[] bytes, IPEndPoint excludeIPEndPoint)
        {
            if (serverSocket == null)
            {
                return;
            }
            foreach (var pair in inGameClients)
            {
                if(pair.Key == excludeIPEndPoint)
                {
                    continue;
                }
                serverSocket.SendTo(bytes, pair.Key);
            }
        }
        public static void SendToClient(IPEndPoint client, byte[] bytes)
        {
            if(serverSocket == null)
            {
                return;
            }
            serverSocket.SendTo(bytes, client);
        }
        public static void ServerFunction(object obj_)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 10200);

            serverSocket.Bind(endPoint);

            byte[] partialBuffer = new byte[1024];
            int partialBytesReceived = 0;

            EndPoint clientEndPoint = new IPEndPoint(IPAddress.None, 0);
            DatabaseHandler.SetMySqlConnection();

            while (true)
            {
                // 데이터 수신
                int receivedBytes = serverSocket.ReceiveFrom(partialBuffer, ref clientEndPoint);

                // 클라이언트의 IPEndPoint 가져오기.
                IPEndPoint clientIPEndPoint = (IPEndPoint)clientEndPoint;
                // 수신된 데이터 처리
                if (partialBytesReceived == 0)
                {
                    // 처음 수신되는 데이터의 경우, 첫 4바이트를 읽어 패킷의 크기 찾습니다.
                    if (receivedBytes < 4)
                    {
                        continue; // 데이터를 추가로 기다립니다.
                    }

                    // 첫 4바이트를 사용하여 패킷의 크기를 파악합니다.
                    int packetSize = BitConverter.ToInt32(partialBuffer, 0);

                    // 수신된 데이터가 충분하지 않으면 추가로 데이터를 수신합니다.
                    if (receivedBytes < packetSize)
                    {
                        partialBytesReceived = receivedBytes; // 현재까지 수신된 데이터 크기 저장
                        continue; // 데이터를 추가로 기다립니다.
                    }

                    // 전체 패킷을 수신한 경우, 패킷을 처리합니다.
                    PacketHandler.ProcessPacket(clientIPEndPoint, partialBuffer, receivedBytes);

                    // 다음 패킷을 위해 초기화합니다.
                    partialBytesReceived = 0;
                }
                else
                {
                    // 이전에 일부만 수신된 데이터가 있는 경우, 추가로 데이터를 받습니다.
                    Array.Copy(partialBuffer, 0, partialBuffer, partialBytesReceived, receivedBytes);
                    partialBytesReceived += receivedBytes;

                    // 전체 패킷을 수신했는지 확인합니다.
                    if (partialBytesReceived >= 4)
                    {
                        int packetSize = BitConverter.ToInt32(partialBuffer, 0);
                        if (partialBytesReceived >= packetSize)
                        {
                            // 전체 패킷을 수신한 경우, 패킷을 처리합니다.
                            PacketHandler.ProcessPacket(clientIPEndPoint, partialBuffer, partialBytesReceived);

                            // 다음 패킷을 위해 초기화합니다.
                            partialBytesReceived = 0;
                        }
                    }
                }
            }  
        }
    }
}
