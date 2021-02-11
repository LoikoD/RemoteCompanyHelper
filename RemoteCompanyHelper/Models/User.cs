using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCompanyHelper.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public User() { }
        public User(string u, string p)
        {
            UserName = u;
            Password = p;
        }
        public User(int id, string u, string p)
        {
            Id = id;
            UserName = u;
            Password = p;
        }
        public User(int id, string u)
        {
            Id = id;
            UserName = u;
        }
    }
}
