using Corp2.Web.Services;
using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using Corp2.Web.Models;

namespace Corp2.Web.Controllers
{
    public class UsersController : Controller
    {
        readonly UserService _userService;
        readonly CorporationService _corporationService;

        public UsersController()
        {
            _userService = new UserService();
            _corporationService = new CorporationService();
        }

        // GET: Users
        [Route("users")]
        public ActionResult GetUsers()
        {
            return View("Index", _userService.GetUserList());
        }

        // GET: Users
        [Route("users/corp/{corporationId}")]
        public ActionResult GetUsersByCorporation(string corporationId)
        {
            //return View("Index", new SelectList(_userService.GetUserList().Select(u=>u.CorporationId==corporationId).ToList(), "CorporationId", "Name"));
            Session["CorporationId"] = corporationId;
            return View("Index", _userService.GetUserListByCorporation(corporationId));
        }

        // GET: Users/Details/5
        [Route("users/{id}")]
        public ActionResult GetUser(int id)
        {
            return View();
        }

        // GET: Users/Create
        [Route("users/add")]
        public ActionResult AddUser()
        {
            ViewBag.IsNew = true;
            //IEnumerable<SelectListItem> corpList = _corporationService.GetCorporationList().Select(corp => new SelectListItem
            //{
            //    Text = corp.Name,
            //    Value = corp.CorporationId
            //});
            ViewBag.CorporationList = new SelectList(_corporationService.GetCorporationList(),"CorporationId","Name");
            if (Session["CorporationId"] != null)
            {
                return View("Users", new UserModel {CorporationId= (string)Session["CorporationId"] } );

            }
            return View("Users");
        }

        // POST: Users/Create
        [HttpPost]
        [Route("users/add")]
        public ActionResult AddUser(FormCollection collection)
        {
            Models.UserModel corp = null;
            try
            {
                // TODO: Add insert logic here
                corp = new Models.UserModel
                {
                    UserId = collection.Get("UserId"),
                    UserName = collection.Get("UserName"),
                    Password = collection.Get("Password"),
                    CorporationId = collection.Get("CorporationId"),
                    Email = collection.Get("Email"),
                };
                if (_userService.AddUser(corp))
                {
                    return RedirectToRoute("");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _userService.ErrorDescription);
                    throw new Exception();
                }
            }
            catch
            {
                ViewBag.CorporationList = new SelectList(_corporationService.GetCorporationList(), "CorporationId", "Name");
                ViewBag.IsNew = true;
                return View("Users",corp);
            }
        }

        // GET: Users/Edit/5
        [Route("users/edit")]
        public ActionResult Edit(string id)
        {
            ViewBag.IsNew = false;
            ViewBag.CorporationList = new SelectList(_corporationService.GetCorporationList(), "CorporationId", "Name");
            Models.UserModel user = _userService.GetUser(id);
            return View("Users",user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [Route("users/edit")]
        public ActionResult Edit(string id, FormCollection collection)
        {
            Models.UserModel corp = null;
            try
            {
                corp = new Models.UserModel
                {
                    UserId = collection.Get("UserId"),
                    UserName = collection.Get("UserName"),
                    Password = collection.Get("Password"),
                    CorporationId = collection.Get("CorporationId"),
                    Email = collection.Get("Email"),
                };
                if (_userService.UpdateUser(corp))
                {
                    return RedirectToRoute("");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _userService.ErrorDescription);
                    throw new Exception();
                }
            }
            catch
            {
                ViewBag.IsNew = false;
                return View("Users",corp);
            }
        }

        // GET: Users/Delete/5
        [Route("users/delete")]
        public ActionResult Delete(string id)
        {
            try
            {
                if (_userService.DeleteUser(id))
                    return RedirectToRoute("");
                else
                {
                    ModelState.AddModelError(string.Empty, _userService.ErrorDescription);
                    throw new Exception();
                }
            }
            catch
            {
                return View("Users");
            }
        }

    }
}
