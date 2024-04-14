using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MySql.Data;

namespace UDPGameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Thread serverThread = new Thread(ServerHandler.ServerFunction);
            serverThread.IsBackground = true;
            serverThread.Start();
            Thread.Sleep(500);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("*** Unity Swipe Game Server ***");
            Console.WriteLine("*** Enter Key To Quit Server ***");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
            serverThread.Abort();
        }
        
    }
}
