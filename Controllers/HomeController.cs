using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserMgmtMVC;
using System.Web.Security;
using UserMgmtMVC.Models;


namespace UserMgmtMVC.Controllers
{
    
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();

        }

        [HttpGet]
        public ActionResult Login()
        {
            Session.Clear();
            Session.Abandon();
            Session["LoggedUsername"] = null;
            return View();

        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(AppUserViewModel U)
        {
            if (ModelState.IsValid)
            {
                
                using (UserDatabaseEntities db = new UserDatabaseEntities())
                {

                    AppUser loggedUser = db.AppUsers.Where(a => a.Username.Equals(U.Username) && a.Password.Equals(U.Password)).FirstOrDefault();

                    if (loggedUser != null)
                    {
                        FormsAuthentication.SetAuthCookie(loggedUser.Username, false);

                        string[] UserRoles = (from roles in db.UserRoles
                                              join rm in db.RoleMappings on roles.RoleId equals rm.RoleId
                                              where rm.UserId == loggedUser.UserId
                                              select roles.RoleName).ToArray();

                        string test = string.Join(",", UserRoles);

                        var authTicket = new FormsAuthenticationTicket(1, loggedUser.Username, DateTime.Now, DateTime.Now.AddMinutes(20), false, test);
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        HttpContext.Response.Cookies.Add(authCookie);

                        if (Session["LoggedUsername"] == null)
                        {
                            Session["LoggedUsername"] = loggedUser.Username.ToString();
                            Session["UserId"] = loggedUser.UserId;

                            return RedirectToAction("NormalUerLoginPage", "Home");

                        }
                        else
                        {
                            var id=U.RoleName;
                            var AppId = db.UserApplications.Where(a => a.ApplicationName.Equals(id)).Select(x => x.UserApplicationId).ToString();


                            var UsrRoles = (from roles in db.UserRoles
                                             join rm in db.RoleMappings on roles.RoleId equals rm.RoleId
                                             where rm.UserId == loggedUser.UserId && roles.RoleName == id
                                             select roles.RoleName).FirstOrDefault();


                            if (UsrRoles != null && UsrRoles == "HR")
                            {
                                ViewBag.Message = "You are not authorised to access HR portal";

                                return RedirectToAction("HRLogin", "HR");
                            }
                            else
                            {
                                if (UsrRoles != null && UsrRoles == "Finance")
                                {
                                    ViewBag.Message = "You are not authorised to access Finance portal";

                                    return RedirectToAction("FinanceLogin", "Finance");
                                }
                                else
                                {

                                    //ViewBag.Message = "You are not authorised to access this portal";
                                    //return View("NormalUerLoginPage");

                                   ModelState.AddModelError("", "Invalid login attempt.");
                                   return View("Error");
                                }
                            }
                        }
                        
                    }
                }
            }
            return View(U);
        }





        //[HttpGet]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }



        [HandleError()]
        [Authorize(Roles = "Admin")]
        public ActionResult AdminLoginPage()
        {

            return View();
        }


        [HandleError()]
        [NoDirectAccess]
        public ActionResult NormalUerLoginPage()
        {
            UserDatabaseEntities db = new UserDatabaseEntities();
            ViewBag.Apps = db.UserApplications.ToList();

            return View();


            //List<SelectListItem> ObjItem = new List<SelectListItem>()
            //                {
            //              new SelectListItem {Text="Applications",Value="0",Selected=true },
            //              new SelectListItem {Text="HR Portal",Value="1" },
            //              new SelectListItem {Text="Finance",Value="2"},
            //              
            //                };
            //ViewBag.ListItem = ObjItem;
        }




        [HttpPost]
        public ActionResult AppRedirection(UserApplicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserDatabaseEntities db = new UserDatabaseEntities();

                var a = db.UserApplications.Where(q => q.UserApplicationId.Equals(model.UserApplicationId)).FirstOrDefault();

                if (model.UserApplicationId == 1)
                {
                    TempData["AppId"] = a.ApplicationName.ToString();
                    return Redirect(a.ApplicationLink);

                    //return RedirectToAction("HRPortal","Home");

                }
                else
                {
                    TempData["AppId"] = a.ApplicationName.ToString();
                    return Redirect(a.ApplicationLink);
                }

            }
            else
            {
                return View();
            }

        }




        //public ActionResult HRPortal()
        //{
        //    return View();
        //}


        //[HttpPost]
        //public ActionResult HRLogin(AppUserViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (UserDatabaseEntities db = new UserDatabaseEntities())
        //        {
        //            var v = db.AppUsers.Where(a => a.Username.Equals(model.Username) && a.Password.Equals(model.Password)).FirstOrDefault();
        //            Session["LoggedUsername"] = v.Username.ToString();
        //            Session["UserId"] = v.UserId;

        //            var q = (from au in db.AppUsers
        //                     join rm in db.RoleMappings on au.UserId equals rm.UserId
        //                     where au.UserId == v.UserId && rm.RoleId == 3
        //                     select new
        //                     {
        //                         rm.RoleId,
        //                         au.UserId,
        //                         au.Username,
        //                         au.Password

        //                     }).FirstOrDefault();
        //            Session["RoleId"] = q.RoleId;

        //            if (v != null && q.RoleId == 3)
        //            {
        //                FormsAuthentication.SetAuthCookie(v.Username, false);

        //                var authTicket = new FormsAuthenticationTicket(1, v.Username, DateTime.Now, DateTime.Now.AddMinutes(20), false, v.UserId.ToString());
        //                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
        //                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        //                HttpContext.Response.Cookies.Add(authCookie);
        //                return View("HRafterLoginPage");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Invalid login attempt.");
        //                return View("Error");
        //            }
        //        }
        //    }

        //    return View(model);

        //}

        //public ActionResult FinancePortal()
        //{
        //    return View();
        //}



        //[HttpPost]

        //public ActionResult FinanceLogin(AppUserViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (UserDatabaseEntities db = new UserDatabaseEntities())
        //        {
        //            var v = db.AppUsers.Where(a => a.Username.Equals(model.Username) && a.Password.Equals(model.Password)).FirstOrDefault();
        //            Session["LoggedUsername"] = v.Username.ToString();
        //            Session["UserId"] = v.UserId;



        //            var q = (from au in db.AppUsers
        //                     join rm in db.RoleMappings on au.UserId equals rm.UserId
        //                     where au.UserId == v.UserId && rm.RoleId == 5
        //                     select new
        //                     {
        //                         rm.RoleId,
        //                         au.UserId,
        //                         au.Username,
        //                         au.Password

        //                     }).FirstOrDefault();
        //            //Session["RoleId"] = q.RoleId;

        //            if (v != null)
        //            {
        //                if (q.RoleId == 5)
        //                {
        //                    ViewBag.Message = "You are authorised to access Finance portal";
        //                    FormsAuthentication.SetAuthCookie(v.Username, false);
        //                    var authTicket = new FormsAuthenticationTicket(1, v.Username, DateTime.Now, DateTime.Now.AddMinutes(20), false, v.UserId.ToString());
        //                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
        //                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        //                    HttpContext.Response.Cookies.Add(authCookie);
        //                    return View("FinanceAfterLoginPage");
        //                }
        //                else
        //                {
        //                    ViewBag.Message = "You are not authorised to access Finance portal";
        //                    return View("NormalUerLoginPage");
        //                }

        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Invalid login attempt.");
        //                return View("Error");
        //            }
        //        }
        //    }

        //    return View(model);

        //}


        public ActionResult GetDetails(int id)
        {
            UserDatabaseEntities dc = new UserDatabaseEntities();
            var v = dc.AppUsers.Where(a => a.UserId.Equals(id)).FirstOrDefault();

            return View(v);
        }



        public ActionResult Edit(int id)
        {
            UserDatabaseEntities dc = new UserDatabaseEntities();
            var obj = dc.AppUsers.Where(a => a.UserId.Equals(id)).FirstOrDefault();

            return View("Edit", obj);
        }


        //public bool UpdateUserDetails(string name, string password, string mobile, string city )
        //{
        //    bool status = false;
        //    try
        //    {
        //        UserMgmtMVC.UserDatabaseEntities db = new UserMgmtMVC.UserDatabaseEntities();

        //        AppUser usr = db.AppUsers.Where(a => a.Username == name).FirstOrDefault();
        //        usr.Password = password;
        //        usr.City = city;
        //        usr.Mobile = mobile;
        //        db.SaveChanges();
        //        status = true;
        //    }
        //    catch (Exception)
        //    {
        //        status = false;
        //    }
        //    return status;
        //}

        [HttpPost]
        public ActionResult Save(AppUser obj)
        {
            var status = false;
            if (ModelState.IsValid)
            {
                UserMgmtMVC.UserDatabaseEntities db = new UserMgmtMVC.UserDatabaseEntities();

                AppUser usr = db.AppUsers.Where(a => a.Username == obj.Username).FirstOrDefault();
                usr.Password = obj.Password;
                usr.City = obj.City;
                usr.Mobile = obj.Mobile;
                db.SaveChanges();
                status = true;

                if (status)
                {
                    return RedirectToAction("GetDetails", "Home", new { id = Session["UserId"] });
                }
                else
                {
                    return View("Error");

                }
            }
            else
            {
                return View();
            }


        }




        public List<AppUser> GetUsersByLinq()
        {
            UserDatabaseEntities dc = new UserDatabaseEntities();

            List<AppUser> lstUsers = null;
            try
            {
                lstUsers = (from c in dc.AppUsers select c).ToList<AppUser>();
            }
            catch (Exception)
            {
                lstUsers = null;
            }
            return lstUsers;
        }



        [Authorize(Roles = "Admin")]

        public ActionResult GetUsers()
        {
            var usrsList = GetUsersByLinq();
            var usersobj = new List<AppUser>();

            try
            {
                if (usrsList.Any())
                {
                    foreach (var usr in usrsList)
                    {
                        usersobj.Add(usr);
                    }
                }
                return View(usersobj);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }


        [Authorize(Roles = "Admin")]

        public ActionResult GetUsersToAssign()
        {
            UserDatabaseEntities db = new UserDatabaseEntities();
            ViewBag.usrs = db.AppUsers.ToList();
            ViewBag.roles = db.UserRoles.ToList();

            return View("GetUsersToAssign");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetUsersToAssign(UserAssignRoleViewModel model)
        {
            try
            {
                UserDatabaseEntities db = new UserDatabaseEntities();

                RoleMapping rmap = new RoleMapping();
                rmap.UserId = model.UserId;
                rmap.RoleId = model.RoleId;

                List<RoleMapping> UserRoleMappings = db.RoleMappings.Where(x => x.UserId == model.UserId && x.RoleId == model.RoleId).ToList();
                if (UserRoleMappings.Count == 0)
                {
                    db.RoleMappings.Add(rmap);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("MappingSucceed");


            //if (ModelState.IsValid)
            //{
            //    UserDatabaseEntities db = new UserDatabaseEntities();
            //    RoleMapping rmap = new RoleMapping();
            //    rmap.UserId = model.UserId;
            //    rmap.RoleId = model.RoleId;

            //    db.RoleMappings.Add(rmap);
            //    db.SaveChanges();
            //    return View("MappingSucceed");
            //}
            //else
            //{
            //    return View("RoleExists");
            //}

        }


        public ActionResult DeleteUsersRoles()
        {
            UserDatabaseEntities db = new UserDatabaseEntities();

            ViewBag.usrs = db.AppUsers.ToList();
            ViewBag.roles = db.UserRoles.ToList();

            return View("DeleteUsersRoles");

        }

        [HttpPost]
        public ActionResult DeleteRecord(UserAssignRoleViewModel model)
        {
            try
            {
                UserDatabaseEntities db = new UserDatabaseEntities();

                RoleMapping rmap = new RoleMapping();
                rmap.UserId = model.UserId;
                rmap.RoleId = model.RoleId;

                var v = db.RoleMappings.Where(a => a.UserId == rmap.UserId && a.RoleId == rmap.RoleId).FirstOrDefault();

                db.RoleMappings.Remove(v);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View("RoleDeleted");
        }



    }
}