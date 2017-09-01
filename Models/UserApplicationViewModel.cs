using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserMgmtMVC.Models
{
    public class UserApplicationViewModel
    {
        public int UserApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationLink { get; set; }

        public int RoleId { get; set; }

    }
}