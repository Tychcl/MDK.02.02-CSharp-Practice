using HtmlAgilityPack;
using System.Net;
using System.Text;

namespace pr7;
static class Program
{
    static void Main(string[] args)
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
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Сначала надо войти для кукисов");
                        continue;
                    }
                    break;
            }
            if(code == HttpStatusCode.OK)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
            }
            Console.WriteLine(m);
        }
    }

    static CookieCollection? SignIn(string login, string password, out string message, out HttpStatusCode code)
    {
        try
        {
            string url = "http://127.0.0.1:8080/ajax/login.php";
            HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);
            r.Method = "POST";
            r.ContentType = "application/x-www-form-urlencoded";
            r.CookieContainer = new CookieContainer();
            string query = $"login={login}&password={password}";
            byte[] data = Encoding.ASCII.GetBytes(query);
            r.ContentLength = data.Length;
            using (var s = r.GetRequestStream())
            {
                s.Write(data, 0, data.Length);
            }
            HttpWebResponse response = (HttpWebResponse)r.GetResponse();
            message = new StreamReader(response.GetResponseStream()).ReadToEnd();
            code = response.StatusCode;
            return response.Cookies;
        }
        catch(WebException ex)
        {
            code = HttpStatusCode.Forbidden;
            message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            return null;
        }
    }

    static CookieCollection? Parse(CookieCollection cookie, out string message, out HttpStatusCode code)
    {
        try
        {
            string url = "http://127.0.0.1:8080/main";
            HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);
            r.CookieContainer = new CookieContainer();
            r.CookieContainer.Add(cookie);
            HttpWebResponse response = (HttpWebResponse)r.GetResponse();
            message = new StreamReader(response.GetResponseStream()).ReadToEnd();
            code = response.StatusCode;
            return response.Cookies;
        }
        catch (WebException ex)
        {
            code = HttpStatusCode.Forbidden;
            message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            return null;
        }
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
            str
        }
    }
}
