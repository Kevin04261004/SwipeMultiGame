using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UDPGameServer
{
    public static class ServerHandler
    {
        public static void ServerFunction(object obj_)
        {
            Socket srvSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 10200);

            srvSocket.Bind(endPoint);

            byte[] recvBytes = new byte[1024];
            EndPoint clientEP = new IPEndPoint(IPAddress.None, 0);
            DatabaseHandler.SetMySqlConnection();
            while (true)
            {
                int nRecv = srvSocket.ReceiveFrom(recvBytes, ref clientEP);
                string txt = Encoding.Default.GetString(recvBytes, 0, nRecv);
                Console.WriteLine("- 게임 클라이언트가 보내온 기록 : " + txt);

                byte[] sendBytes = Encoding.Default.GetBytes(" Echo : " + txt);
                srvSocket.SendTo(sendBytes, clientEP);

                if (true)
                {
                    DatabaseHandler.TryCheckUser();
                }
            }
        }
    }
}
