using Org.BouncyCastle.Utilities;
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
            SetHandler(PacketData.EPacketType.ConnectClient, ClientConnected);
            SetHandler(PacketData.EPacketType.DisconnectClient, ClientDisconnected);
            SetHandler(PacketData.EPacketType.RequireUserLogin, UserLoginRequired);
            SetHandler(PacketData.EPacketType.RequireCreateUser, CreateUserRequired);
        }

        private static void ClientConnected(IPEndPoint endPoint, byte[] data = null)
        {
            Console.WriteLine($"[{endPoint}] Client가 게임을 접속하였습니다.");

            ServerHandler.connectedClients.Add(endPoint);
        }
        private static void ClientDisconnected(IPEndPoint endPoint, byte[] data = null)
        {
            Console.WriteLine($"[{endPoint}] Client가 게임을 종료하였습니다.");

            ServerHandler.connectedClients.Remove(endPoint);
        }
        private static void UserLoginRequired(IPEndPoint endPoint, byte[] data)
        {
            if(data.Length != 32)
            {
                return;
            }

            byte[] idBytes = new byte[SwipeGame_User.ID_SIZE];
            byte[] passwordBytes = new byte[SwipeGame_User.PASSWORD_SIZE];
            Arrays.Fill(idBytes, 0);
            Arrays.Fill(passwordBytes, 0);

            int offset = 0;
            Array.Copy(data, 0, idBytes, offset, SwipeGame_User.ID_SIZE);
            offset += SwipeGame_User.ID_SIZE;
            Array.Copy(data, 0, passwordBytes, offset, SwipeGame_User.PASSWORD_SIZE);
            
            string id = Encoding.UTF8.GetString(idBytes);
            string password = Encoding.UTF8.GetString(passwordBytes);

            bool bHasUserData = DatabaseHandler.TryCheckUser(id, password);
            if (bHasUserData)
            {

            }
            else
            {

            }
        }
        private static void CreateUserRequired(IPEndPoint endPoint, byte[] data)
        {

        }
    }
}
