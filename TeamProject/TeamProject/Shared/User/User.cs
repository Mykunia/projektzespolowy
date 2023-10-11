using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamProject.Shared.Models;

namespace TeamProject.Shared.User
{
    public class User
    {
        //for bigger db is more secure to use guid rather than id
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public Address Address { get; set; }
        //Standard role during register proccess
        public string Role { get; set; } = "Customer";
    }
}
