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
            SetHandler(PacketData.EPacketType.CreateUser, CreateUser);
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
            ClientExitInGame(endPoint);
        }
        private static void UserLoginRequired(IPEndPoint endPoint, byte[] data)
        {
            if(data.Length != (SwipeGame_User.ID_SIZE + SwipeGame_User.PASSWORD_SIZE))
            {
                return;
            }

            byte[] idBytes = new byte[SwipeGame_User.ID_SIZE];
            byte[] passwordBytes = new byte[SwipeGame_User.PASSWORD_SIZE];
            Arrays.Fill(idBytes, 0);
            Arrays.Fill(passwordBytes, 0);

            int offset = 0;
            Array.Copy(data, offset, idBytes, 0, SwipeGame_User.ID_SIZE);
            offset += SwipeGame_User.ID_SIZE; // offset을 ID 크기만큼 증가시킵니다.
            Array.Copy(data, offset, passwordBytes, 0, SwipeGame_User.PASSWORD_SIZE);

            string id = idBytes.ChangeToString();
            string password = passwordBytes.ChangeToString();

            bool bHasUserData = DatabaseHandler.TryCheckUser(id, password);
            byte[] buf;
            if (bHasUserData)
            {
                DatabaseHandler.GetUserNickName(id, password, out string nickName);
                if(string.IsNullOrEmpty(nickName))
                {
                    buf = PackPacket(PacketData.EPacketType.UserLoginFail);
                    ServerHandler.SendToClient(endPoint, buf);
                    Console.WriteLine($"[{endPoint}] User Login FAIL ({id}, {password})");
                    return;
                }
                buf = PackPacket(PacketData.EPacketType.UserLoginSuccess, idBytes, nickName.ChangeToByte());
                /* inGameClientList에 추가. */
                ClientEnterInGame(endPoint, id, nickName);
                ServerHandler.SendToClient(endPoint, buf);
                Console.WriteLine($"[{endPoint}] User Login SUCCESS ({id}, {password})");
            }
            else
            {
                buf = PackPacket(PacketData.EPacketType.UserLoginFail);
                ServerHandler.SendToClient(endPoint, buf);
                Console.WriteLine($"[{endPoint}] User Login FAIL ({id}, {password})");
            }
        }
        private static void CreateUserRequired(IPEndPoint endPoint, byte[] data)
        {
            if (data.Length != (SwipeGame_User.ID_SIZE + SwipeGame_User.PASSWORD_SIZE))
            {
                return;
            }

            byte[] idBytes = new byte[SwipeGame_User.ID_SIZE];
            byte[] passwordBytes = new byte[SwipeGame_User.PASSWORD_SIZE];
            Arrays.Fill(idBytes, 0);
            Arrays.Fill(passwordBytes, 0);

            int offset = 0;
            Array.Copy(data, offset, idBytes, 0, SwipeGame_User.ID_SIZE);
            offset += SwipeGame_User.ID_SIZE; // offset을 ID 크기만큼 증가시킵니다.
            Array.Copy(data, offset, passwordBytes, 0, SwipeGame_User.PASSWORD_SIZE);

            string id = idBytes.ChangeToString();
            string password = passwordBytes.ChangeToString();

            bool bHasUserData = DatabaseHandler.TryCheckUserID(id);
            if (bHasUserData)
            {
                byte[] buf = PackPacket(PacketData.EPacketType.CantCreateUserData);
                ServerHandler.SendToClient(endPoint, buf);
                Console.WriteLine($"[{endPoint}] User Did not Allowed ({id})");
            }
            else
            {
                byte[] buf = PackPacket(PacketData.EPacketType.AllowCreateUserData);
                ServerHandler.SendToClient(endPoint, buf);
                Console.WriteLine($"[{endPoint}] Create new User Allowed ({id})");
            }
        }
        private static void CreateUser(IPEndPoint endPoint, byte[] data)
        {
            if (data.Length != (SwipeGame_User.ID_SIZE + SwipeGame_User.PASSWORD_SIZE + SwipeGame_User.NICKNAME_SIZE))
            {
                return;
            }

            byte[] idBytes = new byte[SwipeGame_User.ID_SIZE];
            byte[] passwordBytes = new byte[SwipeGame_User.PASSWORD_SIZE];
            byte[] nickNameBytes = new byte[SwipeGame_User.NICKNAME_SIZE];
            Arrays.Fill(idBytes, 0);
            Arrays.Fill(passwordBytes, 0);
            Arrays.Fill(nickNameBytes, 0);

            int offset = 0;
            Array.Copy(data, offset, idBytes, 0, SwipeGame_User.ID_SIZE);
            offset += SwipeGame_User.ID_SIZE;
            Array.Copy(data, offset, passwordBytes, 0, SwipeGame_User.PASSWORD_SIZE);
            offset += SwipeGame_User.PASSWORD_SIZE;
            Array.Copy(data, offset, nickNameBytes, 0, SwipeGame_User.NICKNAME_SIZE);

            string id = idBytes.ChangeToString();
            string password = passwordBytes.ChangeToString();
            string nickName = nickNameBytes.ChangeToString();


            bool bCreateUser = DatabaseHandler.CreateUser(id, password, nickName);
            if (bCreateUser)
            {
                byte[] buf = PackPacket(PacketData.EPacketType.UserCreateSuccess, idBytes, nickNameBytes);
                /* inGameClientList에 추가 */
                ClientEnterInGame(endPoint, id, nickName);
                ServerHandler.SendToClient(endPoint, buf);
            }
            else
            {
                byte[] buf = PackPacket(PacketData.EPacketType.UserCreateFail);
                ServerHandler.SendToClient(endPoint, buf);
            }
        }
        private static void ClientEnterInGame(IPEndPoint endPoint, string id, string nickName)
        {
            PlayerData data = new PlayerData()
            {
                id = id,
                nickName = nickName
            };
            ServerHandler.inGameClients.Add(endPoint, data);
        }
        private static void ClientExitInGame(IPEndPoint endPoint)
        {
            ServerHandler.inGameClients.Remove(endPoint);
        }
    }
}
