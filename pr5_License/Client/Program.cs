using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static IPAddress ServerIpAddress;
        static int ServerPort;

        static string ClientToken;
        static DateTime ClientDateConnection;
        static void Main(string[] args)
        {
            OnSettings();

            Thread tCheckToken = new Thread(CheckToken);
            tCheckToken.Start();

            while (true)
            {
                SetCommand();
            }
        }
        public static void SetCommand()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string[] Command = Console.ReadLine().Split(' ');

            if (Command[0] == "/config")
            {
                File.Delete(Directory.GetCurrentDirectory() + "/.config");
                OnSettings();
            }
            else if (Command[0] == "/connect" && Command.Length == 3) ConnectServer(Command[1], Command[2]);
            else if (Command[0] == "/status") GetStatus();
            else if (Command[0] == "/help") Help();
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Wrong command");
            }
        }
        public static void GetStatus()
        {
            int Duration = (int)DateTime.Now.Subtract(ClientDateConnection).TotalSeconds;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Client: {ClientToken}, time connection: {ClientDateConnection.ToString("HH:mm:ss dd.MM")}, " +
                $"duration: {Duration}"
                );
        }
       
        public static void Help()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Commands to the server: ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("/config");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - set initial settings ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("/connect <login> <password>");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - connection to the server ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("/status");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - show list users ");
        }
        public static void ConnectServer(string login, string password)
        {
           

            IPEndPoint endPoint = new IPEndPoint(ServerIpAddress, ServerPort);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           
            try
            {
                socket.Connect(endPoint);
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " +  ex.Message);
            }
            if(socket.Connected)
            {

                socket.Send(Encoding.UTF8.GetBytes($"/connect {login} {password}"));

                byte[] Bytes = new byte[10486760];
                int ByteRec = socket.Receive(Bytes);

                string Response = Encoding.UTF8.GetString(Bytes, 0, ByteRec);
                switch (Response)
                {
                    case "/limit":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There is enough space on the license server");
                        break;
                    case "/wrong":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("wrong login or password");
                        break;
                    case "/blocked":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Blocked");
                        break;
                    case "/dissconnect":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Dissconnected from server");
                        socket.Close();
                        break;
                    default:
                        ClientToken = Response;
                        ClientDateConnection = DateTime.Now;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Recieved connection token: " + ClientToken);
                        break;
                }
            }
        }
        public static void CheckToken()
        {
            while (true)
            {
                if (!String.IsNullOrEmpty(ClientToken))
                {
                    IPEndPoint EndPoint = new IPEndPoint(ServerIpAddress, ServerPort);
                    Socket Socket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Stream,
                        ProtocolType.Tcp);

                    try
                    {
                        Socket.Connect(EndPoint); ;

                    }
                    catch (Exception exp)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error: " + exp.Message);
                    }

                    if (Socket.Connected)
                    {

                        Socket.Send(Encoding.UTF8.GetBytes(ClientToken));

                        byte[] Bytes = new byte[10485760];
                        int ByteRec = Socket.Receive(Bytes);

                        string Response = Encoding.UTF8.GetString(Bytes, 0, ByteRec);
                        if (Response == "/dissconnect")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("The client is disconnected from server");
                            ClientToken = String.Empty;
                        }
                    }
                }

                Thread.Sleep(1000);
            }


        }



        public static void OnSettings()
        {
            string Path = Directory.GetCurrentDirectory() + "/.config";
            string IpAddress = "";

            if (File.Exists(Path))
            {
                StreamReader streamReader = new StreamReader(Path);
                IpAddress = streamReader.ReadLine();
                ServerIpAddress = IPAddress.Parse(IpAddress);
                ServerPort = int.Parse(streamReader.ReadLine());
                streamReader.Close();

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Server address: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(IpAddress);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Server port: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(ServerPort.ToString());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Please provide the IP address if the license server: ");
                Console.ForegroundColor = ConsoleColor.Green;
                IpAddress = Console.ReadLine();
                ServerIpAddress = IPAddress.Parse(IpAddress);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Please specify the license server port: ");
                Console.ForegroundColor = ConsoleColor.Green;
                ServerPort = int.Parse(Console.ReadLine());

                StreamWriter streamWriter = new StreamWriter(Path);
                streamWriter.WriteLine(IpAddress);
                streamWriter.WriteLine(ServerPort.ToString());
                streamWriter.Close();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("To change, write the command: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("/config");
        }
    }
}
