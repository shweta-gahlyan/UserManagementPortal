using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UserMgmtMVC.Models;


namespace UserMgmtMVC.Controllers
{
    public class FinanceController : Controller
    {
        // GET: Finance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FinancePortal()
        {
            return View();
        }

       
        public ActionResult FinanceLogin()
        {
            return View("FinanceAfterLoginPage");

        }



    }
}
