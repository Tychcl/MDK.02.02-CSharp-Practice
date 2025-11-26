using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Client
{
    public class Server
    {
        private IPAddress IP;
        private EndPoint EndPoint;
        private Socket Socket;
        public string Ip { get; private set; }
        public int Port { get; private set; }
        public int ClientId { get; private set; }
        public bool connected { get; private set; }

        public Server(string? ip = null, int? port = null)
        {
            ClientId = -1;
            connected = connect(ip, port);
        }
        public bool connect(string? ip = null, int? port = null) 
        {
            try
            {
                ip = ip is null ? "127.0.0.1" : ip;
                Ip = ip;
                IP = IPAddress.Parse(ip);
                Port = port is null ? 8080 : (int)port;

                EndPoint = new IPEndPoint(IP, Port);
                Socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                Socket.Connect(EndPoint);
                return Socket.Connected;
                
            }
            catch (Exception ex)
            {
                return false;
                //тут тоже чето придумать надо
            }
        }

        public bool Send(string message)
        {
            try
            {


                if (!connected)
                {
                    //и тут обработку надо
                    return false;
                }
                if (!CheckCommand(message))
                {
                    //и тут тоже
                    //может сделать оут переменную для сообщения ошибки?
                    return false;
                }

                ViewModelSend send = new ViewModelSend(message, ClientId);
                if (message.Split(new string[1] { " " }, StringSplitOptions.None)[0] == "set")
                {
                    string[] DataMessage = message.Split(new string[1] { " " }, StringSplitOptions.None);
                    string Name = "";
                    for (int i = 1; i < DataMessage.Length; i++)
                    {
                        if (Name == "")
                        {
                            Name += DataMessage[i];
                        }
                        else
                        {
                            Name += " " + DataMessage[i];
                        }
                    }

                    if (File.Exists(Name))
                    {
                        FileInfo fileInfo = new FileInfo(Name);
                        FileInfoFTP fileInfoFTP = new FileInfoFTP(File.ReadAllBytes(Name), fileInfo.Name);
                        send = new ViewModelSend(JsonConvert.SerializeObject(fileInfoFTP), ClientId);
                    }
                    else
                    {
                        //если файла нет чето надо сделать
                    }
                }
                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(send));
                int bytesSend = Socket.Send(bytes);
                byte[] bar = new byte[10485760];
                int rec = Socket.Receive(bar);
                string responce = Encoding.UTF8.GetString(bar, 0, rec);
                ViewModelMessage mes = JsonConvert.DeserializeObject<ViewModelMessage>(responce);
                switch (mes.Command)
                {
                    case "autorization":
                        ClientId = int.Parse(mes.Data);
                        break;
                    case "message":
                        //тут нужен вывод на экран уведомления сообления
                        //mes.Data
                        break;
                    case "cd":
                        List<string> folders = new List<string>();
                        folders = JsonConvert.DeserializeObject<List<string>>(mes.Data);
                        foreach (var str in folders)
                        {
                            //вывод файлов и папок в окно через итем
                        }
                        break;
                    case "file":
                        string[] Datames = send.Message.Split(new string[1] { " " }, StringSplitOptions.None)
                        string Name = "";
                        for (int i = 1; i < Datames.Length; i++)
                        {
                            if (Name == "")
                            {
                                Name += Datames[i];
                            }
                            else
                            {
                                Name += " " + Datames[i];
                            }
                        }
                        byte[] bytesar = JsonConvert.DeserializeObject<byte[]>(mes.Data);
                        File.WriteAllBytes(Name, bytesar);
                        break;
                    default:
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                //тут тоже вывод должен быть какой нибудь
                return false;
            }
        }

        private bool CheckCommand(string message)
        {
            bool BCommand = false;
            string[] DataMessage = message.Split(new string[1] { " " }, StringSplitOptions.None);
            switch (DataMessage[0])
            {
                case "connect":
                    BCommand = DataMessage.Length == 3;
                    break;
                case "cd":
                    BCommand = true;
                    break;
                case "get":
                    BCommand = DataMessage.Length == 2;
                    break;
                case "set":
                    BCommand = DataMessage.Length == 2;
                    break;
            }
            return BCommand;
        }
    }
}
