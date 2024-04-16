using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UDPGameServer
{
    public static partial class PacketHandler
    {
        private static void SetAllHandlers()
        {
            SetHandler(PacketData.EPacketType.ConnectUDP, UDPClientSendMsgToServer);
        }

        private static void UDPClientSendMsgToServer(IPEndPoint endPoint)
        {
            Console.WriteLine($"[{endPoint}] Client가 게임을 접속하였습니다.");
        }
    }
}
