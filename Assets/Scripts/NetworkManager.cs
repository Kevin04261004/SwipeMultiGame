using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UIElements;

public class NetworkManager : MonoBehaviour
{
    public int networkFlag { get; set; } = 0;

    public void SendLengthToServer(float length_)
    {
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Debug.Assert(clientSocket != null);
        byte[] buf = Encoding.Default.GetBytes(length_.ToString());
        EndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 10200);

        clientSocket.SendTo(buf, serverEndPoint);
        byte[] recvBytes = new byte[1024];
        int nRecv = clientSocket.ReceiveFrom(recvBytes, ref serverEndPoint);
        string text = Encoding.Default.GetString(recvBytes, 0, nRecv);

        networkFlag = 0;

        Debug.Log(text);
    }
}
