using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserMgmtMVC.Models
{
    public class UserAssignRoleViewModel
    {

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public int Id { get; set; }


    }
}