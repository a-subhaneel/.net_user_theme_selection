using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using User_Theme_selection.Models;

namespace User_Theme_selection.Controllers
{
    public class HomeController : Controller
    {
        private UserDBContext db = new UserDBContext();

      

        public ActionResult Index()
        {
            if (Session["UserName"] == null)
            {
                Response.Redirect("Login");
            }
            return View(db.userAccount);
        }


        //public ActionResult SelectColor(colorOPT clr)
        //{
        //    ViewBag.Message = "Your application description page.";

        //    if (clr.colorcode != null)
        //    {
        //        db.colored.Add(clr);
        //        db.SaveChanges();

        //        ModelState.Clear();
        //        TempData["colorsuccess"] = clr.colorcode + " successfully registered.";
        //        ModelState.Clear();
        //        return RedirectToAction("SelectColor", "Home");
        //    }


        //    return View();
        //}


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

        //REGISTER
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserAccounts adm)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState != null)
            {
                db.userAccount.Add(adm);
                db.SaveChanges();

                ModelState.Clear();
                TempData["Success"] = adm.FirstName + " " + adm.LastName + " successfully registered.";
                ModelState.Clear();
                return RedirectToAction("Login", "Home");
            }
            return View();
        }


        //LOGIN
        public ActionResult Login()
        {
            UserAccounts admin = new UserAccounts();
            if (Session["UserName"] != null)
            {
                return RedirectToAction("Index", "Home", new { UserName = Session["UserName"].ToString() });
                //  ViewData["count"] = counter++;
            }
            return View(admin);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserAccounts adm, bool? value)
        {

            var nouser = db.userAccount.Where(u => u.EmailId == adm.EmailId && u.Password != adm.Password).Any();
            var newudb = db.userAccount.Where(u => u.EmailId == adm.EmailId && u.Password == adm.Password).FirstOrDefault();
            if (newudb != null)
            {
                Session["ID"] = newudb.ID.ToString();
                Session["UserName"] = newudb.UserName.ToString();
                Session["colorcomplex"] = newudb.colorcomplex.ToString();
                return RedirectToAction("Index", "Home", adm);
            }
            else if (nouser == true)
            {
                ModelState.AddModelError("", "Password doesnot match with email-id");
            }
            else
            {
                ModelState.AddModelError("", "credentials mis-match");
            }
            return View();
        }


        //LOGOUT
        public ActionResult Logout()
        {
            {
                FormsAuthentication.SignOut();

                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Home");

            }
        }



        //LOGGEDIN
        public ActionResult LoggedIn()
        {
            if (Session["ID"] != null)
            {
                ViewBag.EmailId = Session["UserName"];
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        //DELETE
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAccounts user = db.userAccount.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {

            UserAccounts user = db.userAccount.Find(id);
            db.userAccount.Remove(user);

            db.SaveChanges();
            if (user.ID.ToString() == Session["ID"].ToString())
            {
                Session.Abandon();
            }

            return RedirectToAction("Index", "Home");
        }



        //EDIT
        public ActionResult Edit(int? id, UserAccounts adm)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (Session["ID"].ToString() != adm.ID.ToString())
            {
                ModelState.AddModelError("", "you are not authorized to make data changes except your own.!");
                return RedirectToAction("Index", "Home", adm);
            }
            else
            {
                //           var model = new UserAccountViewModel();
                UserAccounts user = db.userAccount.Find(id);

                return View(user);
            }
        }



        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserAccounts user)
        {
            var newudb = db.userAccount.Where(u => u.EmailId == user.EmailId && u.Password == user.Password).First();

            Session["ID"] = newudb.ID.ToString();
            Session["UserName"] = newudb.UserName.ToString();
            Session["colorcomplex"] = newudb.colorcomplex.ToString();


            //db.Entry(user).State = EntityState.Modified;
            db.Set<UserAccounts>().AddOrUpdate(user);


            db.SaveChanges();
            Session["UserName"] = newudb.UserName.ToString();
            Session["colorcomplex"] = newudb.colorcomplex.ToString();
            ModelState.Clear();
            TempData["updated"] = user.FirstName + " " + user.LastName + " successfully updated";

            return RedirectToAction("Edit", "Home", user);
        }


        //DETAILS
        public ActionResult Details(int id)
        {

            var product = db.userAccount.Where(p => p.ID == id).FirstOrDefault();
            if (product == null)
            {
                return new HttpNotFoundResult();
            }
            return View(product);
        }



    }
}
