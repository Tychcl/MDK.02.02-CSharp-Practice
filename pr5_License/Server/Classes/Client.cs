using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Classes
{
    public class Client
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public DateTime DateConnect { get; set; }
        public bool Blocked { get; set; }

        public static string GenToken()
        {
            Random random = new Random();
            string Chars = "FDSAFASGfdsfGSDFGSDFGFDSF12345678988884441111";
            return new string(Enumerable.Repeat(Chars, 15).Select(x => x[random.Next(Chars.Length)]).ToArray());
        }
    }
}
