using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CongEspVilaGuilhermeApi.Domain.Models
{
    public class UserListItem
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}