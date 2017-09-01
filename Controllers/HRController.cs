using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UserMgmtMVC.Models;



namespace UserMgmtMVC.Controllers
{
    //[Authorize(Roles="HR")]
    public class HRController : Controller
    {
        // GET: HR
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HRPortal()
        {
            return View();
        }


        
        public ActionResult HRLogin()
        {

            return View();

        }


    }
}