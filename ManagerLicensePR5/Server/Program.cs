using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Classes;

namespace Server
{
    public class Program
    {
        static IPAddress ServerIpAddress;
        static int ServerPort;
        static int MaxClient;
        static int Duration;
        static List<Classes.Client> AllClients = new List<Classes.Client>();
        static void Main(string[] args)
        {
            OnSettings();

            Thread tListenel = new Thread(ConnectServer);
            tListenel.Start();

            Thread tDisconnect = new Thread(CheckDisconnectClient);
            tDisconnect.Start();
            while (true)
            {
                SetCommand();
            }
        }
        static void CheckDisconnectClient()
        {
            while (true)
            {
                for (int iClient = 0; iClient < AllClients.Count; iClient++)
                {
                    int ClientDuration = (int)DateTime.Now.Subtract(AllClients[iClient].DateConnect).TotalSeconds;

                    if (ClientDuration > Duration)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"Client: {AllClients[iClient].Token} disconnect from server to timeout");

                        AllClients.RemoveAt(iClient);
                    }
                }
                Thread.Sleep(1000);
            }
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
            Console.Write("/disconnect");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - disconnect users from the server ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("/status");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - show list users ");
        }

        public static void ConnectServer()
        {
            IPEndPoint EndPoint = new IPEndPoint(ServerIpAddress, ServerPort);
            Socket SocketListener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            SocketListener.Bind(EndPoint);
            SocketListener.Listen(10);
            while (true)
            {
                Socket Handler = SocketListener.Accept();
                byte[] Bytes = new byte[10485760];
                int ByteRec = Handler.Receive(Bytes);

                string Message = Encoding.UTF8.GetString(Bytes, 0, ByteRec);
                string Response = SetCommandClient(Message);

                Handler.Send(Encoding.UTF8.GetBytes(Response));
            }

        }
        static string SetCommandClient(string message)
        {
            string[] Command = message.Split(' ');
            switch (Command[0])
            {
                case "/connect":
                    using (var con = new context())
                    {
                        var clients = con.Clients.ToList();
                        if(!(clients.Count() < MaxClient))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"There is not enough space on the license server");
                            return "/limit";
                        }
                        var client = clients.Find(x => x.Login == Command[1] && x.Password == Command[2]);
                        if(client is null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Wrong login or password");
                            return "/wrong";
                        }
                        if (client.Blocked)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Client blocked");
                            return "/blocked";
                        }

                        client.Token = Classes.Client.GenToken();
                        AllClients.Add(client);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"New client connection: " + client.Token);
                        con.SaveChanges();
                        return client.Token;
                    }
                    break;
                default:
                    Classes.Client Client = AllClients.Find(x => x.Token == message);
                    return Client != null ? "/connect" : "/dissconnect";
            }
        }
        public static void SetCommand()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string Command = Console.ReadLine();

            if (Command == "/config")
            {
                File.Delete(Directory.GetCurrentDirectory() + "/.config");
                OnSettings();
            }
            else if (Command.Contains("/disconnect")) DisconnectServer(Command);
            else if (Command == "/status") GetStatus();
            else if (Command == "/help") Help();
        }
        static void DisconnectServer(string сommand)
        {
            try
            {
                string Token = сommand.Replace("/disconnect", "").Trim();
                Classes.Client DisconnectClient = AllClients.Find(x => x.Token == Token);
                AllClients.Remove(DisconnectClient);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"Client: {Token} disconnect from server");
            }
            catch (Exception exp)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + exp.Message);
            }

        }
        public static void GetStatus()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Count Clients: {AllClients.Count}");
            foreach (Classes.Client Client in AllClients)
            {
                int Duration = (int)DateTime.Now.Subtract(Client.DateConnect).TotalSeconds;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Client: {Client.Token}, time connection: {Client.DateConnect.ToString("HH:mm:ss dd.MM")}, " +
                    $"duration: {Duration}"
                    );
            }


        }
        public static void OnSettings()
        {
            string Path = Directory.GetCurrentDirectory() + "/.config";
            Console.WriteLine(Path);
            string IpAddress = "";

            if (File.Exists(Path))
            {
                StreamReader streamReader = new StreamReader(Path);
                IpAddress = streamReader.ReadLine();
                ServerIpAddress = IPAddress.Parse(IpAddress);
                ServerPort = int.Parse(streamReader.ReadLine());
                MaxClient = int.Parse(streamReader.ReadLine());
                Duration = int.Parse(streamReader.ReadLine());
                streamReader.Close();

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Server address: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(IpAddress);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Server port: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(ServerPort.ToString());

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Max count clients: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(MaxClient.ToString());

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Token lifetime: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Duration.ToString());
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

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Please indicate the largest number of clients: ");
                Console.ForegroundColor = ConsoleColor.Green;
                MaxClient = int.Parse(Console.ReadLine());

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Specify the token lifetime: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Duration = int.Parse(Console.ReadLine());

                StreamWriter streamWriter = new StreamWriter(Path);
                
                streamWriter.WriteLine(IpAddress);
                streamWriter.WriteLine(ServerPort.ToString());
                streamWriter.WriteLine(MaxClient);
                streamWriter.WriteLine(Duration);
                streamWriter.Close();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("To change, write the command: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("/config");
        }
    }
}
