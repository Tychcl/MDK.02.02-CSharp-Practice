using Common;
using Server;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Server
{
    class Program
    {
        public static List<User> Users = new List<User>();
        public static IPAddress IpAdress;
        public static int Port = 8080;

        static void Main(string[] args)
        {
            Users.Add(new User("Tychcl", "f1l1n.TQ5", @"A:\Авиатехникум"));
            string ip = "127.0.0.1";
            IpAdress = IPAddress.Parse(ip);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"IP: {ip} Port: {Port}");
            Start();
        }

        public static bool AuthorizationUser(string login, string password)
        {
            User user = null;
            user = Users.Find(x => x.login == login && x.password == password);
            return user is not null;
        }

        public static List<string> GetDirectory(string src)
        {
            List<string> dir = new List<string>();
            if (Directory.Exists(src))
            {
                string[] dirs = Directory.GetDirectories(src);
                foreach (string dirName in dirs)
                {
                    string Name = dirName.Replace(src, "");
                    dir.Add(Name+"/");
                }
                string[] files = Directory.GetFiles(src);
                foreach(string file in files)
                {
                    string Name = file.Replace(src, "");
                    dir.Add(Name);
                }
            }

            return dir;
        }

        public static void Start()
        {
            IPEndPoint end = new IPEndPoint(IpAdress, Port);
            Socket sListener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            sListener.Bind(end);
            sListener.Listen(10);
            Console.WriteLine("Запуск");
            while (true)
            {
                try
                {
                    Socket handler = sListener.Accept();
                    string data = null;
                    byte[] Bytes = new byte[10485760];
                    int BytesRec = handler.Receive(Bytes);
                    data += Encoding.UTF8.GetString(Bytes, 0, BytesRec);
                    string reply = "";
                    byte[] message = null;
                    ViewModelSend ViewModelSend = JsonConvert.DeserializeObject<ViewModelSend>(data);
                    if (ViewModelSend is not null)
                    {
                        ViewModelMessage viewModelMessage = new ViewModelMessage();
                        string[] DataCommand = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                        List<string> Folder = new List<string>();

                        if (ViewModelSend.Id == -1 & DataCommand[0] != "connect")
                        {
                            viewModelMessage = new ViewModelMessage("message", "Надо авторизоваться.");
                            reply = JsonConvert.SerializeObject(viewModelMessage);
                            message = Encoding.UTF8.GetBytes(reply);
                            handler.Send(message);
                            continue;

                        }

                        string[] DataMessage = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                        switch (DataCommand[0])
                        {
                            case "connect":

                                if (AuthorizationUser(DataMessage[1], DataMessage[2]))
                                {
                                    int IdUser = Users.FindIndex(x => x.login == DataMessage[1] && x.password == DataMessage[2]);
                                    viewModelMessage = new ViewModelMessage("autorization", IdUser.ToString());
                                }
                                else
                                {
                                    viewModelMessage = new ViewModelMessage("message", "Не правильный логин и пароль пользователя.");
                                }
                                break;
                            case "cd":
                                List<string> FoldersFiles = new List<string>();

                                if (DataMessage.Length == 1)
                                {
                                    Users[ViewModelSend.Id].temp_src = Users[ViewModelSend.Id].src;
                                    FoldersFiles = GetDirectory(Users[ViewModelSend.Id].src);
                                }
                                else
                                {
                                    string cdfolder = "";
                                    for (int i = 1; i < DataMessage.Length; i++)
                                    {
                                        if (cdfolder == "")
                                            cdfolder += DataMessage[i];
                                        else
                                            cdfolder += " " + DataMessage[i];
                                    }
                                    Users[ViewModelSend.Id].temp_src = Users[ViewModelSend.Id].temp_src + cdfolder;
                                    FoldersFiles = GetDirectory(Users[ViewModelSend.Id].temp_src);
                                }

                                if (FoldersFiles.Count == 0)
                                    viewModelMessage = new ViewModelMessage("message", "Директория пуста или не существует.");
                                else
                                    viewModelMessage = new ViewModelMessage("cd", JsonConvert.SerializeObject(FoldersFiles));
                                break;
                            case "get":
                                string get = "";
                                for(int i = 1; i < DataMessage.Length; i++)
                                {
                                    if (get == "")
                                    {
                                        get += DataMessage[i];
                                    }
                                    else
                                    {
                                        get += " " + DataMessage[i];
                                    }
                                }
                                byte[] bytes = File.ReadAllBytes(Users[ViewModelSend.Id].temp_src + get);
                                viewModelMessage = new ViewModelMessage("file", JsonConvert.SerializeObject(bytes));
                                break;
                            default:
                                FileInfoFTP SendFileFTP = JsonConvert.DeserializeObject<FileInfoFTP>(ViewModelSend.Message);
                                File.WriteAllBytes(Users[ViewModelSend.Id].temp_src + @"\" + SendFileFTP.Name, SendFileFTP.Data);
                                viewModelMessage = new ViewModelMessage("message", "Файл загружен");
                                break;
                        }
                        reply = JsonConvert.SerializeObject(viewModelMessage);
                        message = Encoding.UTF8.GetBytes(reply);
                        handler.Send(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
