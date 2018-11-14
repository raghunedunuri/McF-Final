using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace McF.Controllers
{
    
    public class EthonalController : Controller
    {
        // GET: Ethonal
        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Data Source/Ethanol")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Ethonal/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Ethonal/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ethonal/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Ethonal/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Ethonal/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Ethonal/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Ethonal/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
