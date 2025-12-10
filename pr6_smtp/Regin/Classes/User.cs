using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regin.Classes
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int? PinCode { get; set; }
        public string Name { get; set; }
        public byte[]? Image { get; set; }
        public DateTime Updated {  get; set; }
        public DateTime Created { get; set; }
        
        public User(string login,  string password,
                    string name, byte[]? image = null)
        {
            Login = login;
            Password = password;
            Name = name;
            Image = image;
            Updated = DateTime.Now;
            Created = DateTime.Now;
        }
    }
}
