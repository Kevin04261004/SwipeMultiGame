using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class NetworkManager : MonoBehaviour
{
    public static readonly int PORT_NUM = 10200;
    public int networkFlag { get; set; } = 0;
    private Socket clientSocket;
    private EndPoint serverEndPoint;
    private Thread receiveThread;
    
    private byte[] partialBuffer = new byte[1024];
    private int partialBytesReceived = 0;
    public void Awake()
    {
        Init();
    }

    private void Init()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Debug.Assert(clientSocket != null);

        serverEndPoint = new IPEndPoint(IPAddress.Loopback, PORT_NUM);
        Debug.Assert(serverEndPoint != null);

        receiveThread = new Thread(ReceiveFromServer);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    public void SendToServer(byte[] bytes)
    {
        clientSocket.SendTo(bytes, serverEndPoint);
    }

    public void ReceiveFromServer()
    {
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0); // 클라이언트의 주소를 담을 변수

        while (true)
        {
            // 데이터 수신
            int receivedBytes = clientSocket.ReceiveFrom(partialBuffer, ref remoteEndPoint);

            // 수신된 데이터 처리
            if (partialBytesReceived == 0)
            {
                // 처음 수신되는 데이터의 경우, 첫 4바이트를 읽어 패킷의 크기 찾습니다.
                if (receivedBytes < 4)
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
                ProcessPacket(partialBuffer, receivedBytes);

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
                        ProcessPacket(partialBuffer, partialBytesReceived);

                        // 다음 패킷을 위해 초기화합니다.
                        partialBytesReceived = 0;
                    }
                }
            }
        }
    }

    private void ProcessPacket(byte[] packetBuffer, int packetSize)
    {
        byte[] data = new byte[packetSize - sizeof(int) - sizeof(PacketData.EPacketType)];
        PacketData.EPacketType packetType = (PacketData.EPacketType)BitConverter.ToInt32(partialBuffer, sizeof(int));
        Array.Copy(data, 0, partialBuffer, sizeof(int) + sizeof(PacketData.EPacketType), packetSize - sizeof(int) - sizeof(PacketData.EPacketType));
        Debug.Log($"Received complete packet with size: {packetSize}, PacketType = {packetType}");
        switch (packetType)
        {
            case PacketData.EPacketType.RequireUserLogin:
                
                break;
            case PacketData.EPacketType.RequireCreateUser:
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
