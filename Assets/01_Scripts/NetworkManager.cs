using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class NetworkManager : MonoBehaviour
{
    public static readonly int PORT_NUM = 10200;
    private static readonly string SERVER_IP = "192.168.110.194";
    private Socket clientSocket;
    private EndPoint serverEndPoint;
    private Thread receiveThread;
    private byte[] partialBuffer = new byte[1024];
    public string hostId { get; set; } = null;
    private int partialBytesReceived = 0;
    public void Awake()
    {
        Init();
        
    }

    private void Init()
    {
        // Client Socket 초기화 
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Debug.Assert(clientSocket != null);
        // 서버 EndPoint 세팅
        IPAddress ipAddress = IPAddress.Parse(SERVER_IP);
        serverEndPoint = new IPEndPoint(ipAddress, PORT_NUM);
        Debug.Assert(serverEndPoint != null);

        // 굳이 함수화를 안해도 되는데, 가독성을 위해...
        ConnectUDPAndReceiveThreadStart();
    }

    private void ConnectUDPAndReceiveThreadStart()
    {
        // UDP는 간접적으로 연결을 하기 위해서 클라이언트에서 서버에 먼저 데이터를 보내야만 ReceiveFrom()함수를 사용할 수 있음.
        Connect();
        
        // 멀티스레드를 활용하여 서버로부터 ReceiveFrom()함수로 계속 블락킹 및 Packet 처리
        receiveThread = new Thread(ReceiveFromServer)
        {
            IsBackground = true
        };
        receiveThread.Start();
    }

    #region 서버 접속과 종료

    private void Connect()
    {
        byte[] connectUDP = PacketHandler.PackPacket(PacketData.EPacketType.ConnectClient);
        Debug.Log("[Client] Try Connect to UDP Server");
        SendToServer(connectUDP);
    }
    private void Disconnect()
    {
        byte[] connectUDP = PacketHandler.PackPacket(PacketData.EPacketType.DisconnectClient);
        Debug.Log("[Client] Exit UDP Server");
        SendToServer(connectUDP);
        // clientSocket?.Close();
    }

    #endregion

    private void OnApplicationQuit()
    {
        Disconnect();
    }
    
    // 서버에게 바이트를 보낸다.
    public void SendToServer(byte[] bytes)
    {
        clientSocket.SendTo(bytes, serverEndPoint);
    }

    private void ReceiveFromServer()
    {
        while (true)
        {
            // 데이터 수신
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            int receivedBytes = clientSocket.ReceiveFrom(partialBuffer, ref remoteEndPoint);
            // 수신된 데이터 처리
            if (partialBytesReceived == 0)
            {
                // 처음 수신되는 데이터의 경우, 첫 4바이트를 읽어 패킷의 크기 찾습니다.
                if (receivedBytes < sizeof(int))
                {
                    // 수신된 데이터가 충분하지 않음
                    Debug.Log("Received incomplete packet size");
                    continue; // 데이터를 추가로 기다립니다.
                }

                // 첫 4바이트를 사용하여 패킷의 크기를 파악합니다.
                int packetSize = BitConverter.ToInt32(partialBuffer, 0);

                // 수신된 데이터가 충분하지 않으면 추가로 데이터를 수신합니다.
                if (receivedBytes < packetSize)
                {
                    // 수신된 데이터가 전체 패킷의 크기보다 작음
                    Debug.Log("Received incomplete packet");
                    partialBytesReceived = receivedBytes; // 현재까지 수신된 데이터 크기 저장
                    continue; // 데이터를 추가로 기다립니다.
                }

                // 전체 패킷을 수신한 경우, 패킷을 처리합니다.
                PacketHandler.ProcessPacket(partialBuffer, receivedBytes);

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
                        PacketHandler.ProcessPacket(partialBuffer, partialBytesReceived);

                        // 다음 패킷을 위해 초기화합니다.
                        partialBytesReceived = 0;
                    }
                }
            }
        }
    }
}
