using Corp2.Web.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Corp2.Web.Controllers
{
    public class CorporationsController : Controller
    {
        CorporationService _corporationService;

        public CorporationsController()
        {
            _corporationService = new CorporationService();
        }
        // GET: Corporations
        public ActionResult Index()
        {
            var corpList = _corporationService.GetCorporationList();
            return View(corpList);
        }

        // GET: Corporations/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Corporations/Create
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.IsNew = true;
            return View();
        }

        // POST: Corporations/Create
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            Models.CorporationModel corp = null;
            try
            {
                // TODO: Add insert logic here
                corp = new Models.CorporationModel
                {
                    CorporationId = collection.Get("CorporationId"),
                    Name = collection.Get("Name"),
                    Address = collection.Get("Address"),
                    Phone = collection.Get("Phone"),
                };
                if (await _corporationService.AddCorporation(corp))
                {
                    return View("Index", _corporationService.GetCorporationList());
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _corporationService.ErrorDescription);
                    throw new Exception();
                }
            }
            catch
            {
                ViewBag.IsNew = true;
                return View(corp);
            }
        }

        // GET: Corporations/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.IsNew = false;
            Models.CorporationModel corp = _corporationService.GetCorporation(id);
            return View("Create",corp);
        }

        // POST: Corporations/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            Models.CorporationModel corp = null;
            try
            {
                corp = new Models.CorporationModel
                {
                    CorporationId = collection.Get("CorporationId"),
                    Name = collection.Get("Name"),
                    Address = collection.Get("Address"),
                    Phone = collection.Get("Phone"),
                };
                if (_corporationService.UpdateCorporation(corp))
                {
                    return View("Index", _corporationService.GetCorporationList());
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _corporationService.ErrorDescription);
                    throw new Exception();
                }
            }
            catch
            {
                ViewBag.IsNew = false;
                return View("Create",corp);
            }
        }
 
        // GET: Corporations/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                if (_corporationService.DeleteCorporation(id))
                    return View("Index", _corporationService.GetCorporationList());
                else
                {
                    ModelState.AddModelError("Error", _corporationService.ErrorDescription);
                    throw new Exception();
                }
            }
            catch
            {
                return View("Index", _corporationService.GetCorporationList());
            }
        }

        // POST: Corporations/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                _corporationService.DeleteCorporation(collection.Get("CorporationId"));
                return View("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
