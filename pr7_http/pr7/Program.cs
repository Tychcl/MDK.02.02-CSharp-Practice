using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace pr7;
static class Program
{
    //private static readonly HttpClient _httpClient = new HttpClient();
    static async Task Main(string[] args)
    {
        CookieCollection? SessionCookie = new CookieCollection();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Введите одну из команд");
        string m = string.Empty;
        HttpStatusCode code = HttpStatusCode.BadRequest;
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            string[] message = Console.ReadLine().Split(' ');
            switch (message[0])
            {
                case "/signin":
                    if(message.Length == 3)
                    {
                        SessionCookie = SignIn(message[1], message[2], out m, out code);
                        Output(m, code);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Неверное колличество аргументов");
                        continue;
                    }
                break;
                case "/parse":
                    if (SessionCookie is not null && SessionCookie.Count > 0 && SessionCookie.First(x=>x.Name == "token") is not null)
                    {
                        Parse(SessionCookie, out m, out code);
                        Output(m, code);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Сначала надо войти для кукисов");
                        continue;
                    }
                    break;
                case "/parseZamenu":
                    m = await ParseZamenu();
                    break;
                case "/save":
                    Save(m);
                    break;
                case "/add":
                    if (SessionCookie is not null && SessionCookie.Count > 0 && SessionCookie.First(x => x.Name == "token") is not null)
                    {
                        Add(SessionCookie, out m, out code);
                        Output(m, code);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Сначала надо войти для кукисов");
                        continue;
                    }
                    break;
                case "/help":
                    Console.WriteLine("/signin <Логин> <Пароль> - Получение токена для сайта авика");
                    Console.WriteLine("/parse - Парсинг главной страницы новостей авика");
                    Console.WriteLine("/parseZamenu - Парсинг замен пар");
                    Console.WriteLine("/save - сохранение в txt файл последнего сообщения");
                    Console.WriteLine("/add - добавить новость");
                    break;
            }
        }
    }

    static CookieCollection? SignIn(string login, string password, out string message, out HttpStatusCode code)
    {
        try
        {
            string url = "http://127.0.0.1:8080/ajax/login.php";
            var requestContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("login", login),
            new KeyValuePair<string, string>("password", password)
        });

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            using var client = new HttpClient(handler);

            var response = client.PostAsync(url, requestContent).Result;
            message = response.Content.ReadAsStringAsync().Result;
            code = response.StatusCode;

            return handler.CookieContainer.GetCookies(new Uri(url));
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is HttpRequestException httpEx && httpEx.StatusCode.HasValue)
            {
                code = httpEx.StatusCode.Value;
                message = ex.Message;
            }
            else
            {
                code = HttpStatusCode.Forbidden;
                message = ex.Message;
            }
            return null;
        }
    }

    static void Parse(CookieCollection cookie, out string message, out HttpStatusCode code)
    {
        try
        {
            string url = "http://127.0.0.1:8080/main";
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            foreach (Cookie c in cookie)
            {
                handler.CookieContainer.Add(new System.Net.Cookie(c.Name, c.Value, c.Path, c.Domain));
            }

            using var client = new HttpClient(handler);

            var response = client.GetAsync(url).Result;
            message = HtmlConvert(response.Content.ReadAsStringAsync().Result);
            code = response.StatusCode;
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is HttpRequestException httpEx && httpEx.StatusCode.HasValue)
            {
                code = httpEx.StatusCode.Value;
            }
            else
            {
                code = HttpStatusCode.Forbidden;
            }
            message = ex.Message;
        }
    }

    static async Task<string> ParseZamenu()
    {
        string message;
        HttpStatusCode code;
        try
        {
            string url = "https://permaviat.ru/raspisanie-zamen/";
            HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);
            using (var response = (HttpWebResponse)await r.GetResponseAsync())
            {
                message = HtmlConvertZamenu(new StreamReader(response.GetResponseStream()).ReadToEnd());
                code = response.StatusCode;
            }
            Output(message, code);
            return message;
        }
        catch (WebException ex)
        {
            code = HttpStatusCode.Forbidden;
            message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            Output(message, code);
            return message;
        }
    }

    static string HtmlConvertZamenu(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var node = doc.DocumentNode;
        IEnumerable<HtmlNode> news = node.Descendants(0).Where(x => x.HasClass("file_link"));
        string str = "";
        foreach (var el in news)
        {
            var date = el.ChildNodes[0].InnerText;
            var link = el.ChildNodes[0].GetAttributeValue("href", "none");
            str += $"Дата: {date}\nСсылка: {link}\n\n";
        }
        return str;
    }

    static string HtmlConvert(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var node = doc.DocumentNode;
        IEnumerable<HtmlNode> news = node.Descendants(0).Where(x => x.HasClass("news"));
        string str = "";
        foreach(var el in news)
        {
            var src = el.ChildNodes[1].GetAttributeValue("src", "none");
            var name = el.ChildNodes[3].InnerText;
            var des = el.ChildNodes[5].InnerText;
            str += $"Название: {name}\nИзображение: {src}\nОписание: {des}\n\n";
        }
        return str;
    }
    
    static void Output(string message, HttpStatusCode code)
    {
        if (code == HttpStatusCode.OK)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
        }
        Console.WriteLine(message);
    }

    static bool Save(string str)
    {
        try
        {
            string path = Directory.GetCurrentDirectory() + "\\output.txt";
            File.WriteAllText(path, str);
            Output("Успешно сохранено: "+path, HttpStatusCode.OK);
            return true;
        }
        catch (Exception ex)
        {
            Output("Ошибка сохранения: "+ex.Message, HttpStatusCode.BadRequest);
            return false;
        }        
    }

    static void Add(CookieCollection cookie, out string message, out HttpStatusCode code)
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Ссылка на изображение: ");
            string image = Console.ReadLine();
            Console.Write("Наименование: ");
            string name = Console.ReadLine();
            Console.Write("Описание: ");
            string des = Console.ReadLine();
            string url = "http://127.0.0.1:8080/ajax/add.php";
            var requestContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("src", image),
            new KeyValuePair<string, string>("name", name),
            new KeyValuePair<string, string>("description", des)
        });

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            using var client = new HttpClient(handler);

            var response = client.PostAsync(url, requestContent).Result;
            message = response.Content.ReadAsStringAsync().Result;
            code = response.StatusCode;
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is HttpRequestException httpEx && httpEx.StatusCode.HasValue)
            {
                code = httpEx.StatusCode.Value;
                message = ex.Message;
            }
            else
            {
                code = HttpStatusCode.Forbidden;
                message = ex.Message;
            }
        }            
    }
}
