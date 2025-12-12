using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pr9_TGBot.Classes
{
    public class User
    {
        public long Id { get; set; }
        public List<Event> Events { get; set; }
        public User(long id)
        {
            Id = id;
            Events = new List<Event>();
        }
    }
}
