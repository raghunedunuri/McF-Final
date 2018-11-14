using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace McF.Controllers
{
    
    public class MasterListController : Controller
    {
        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Master List")]
        // GET: MasterList
        public ActionResult Index()
        {
            return View();
        }

        // GET: MasterList/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MasterList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MasterList/Create
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

        // GET: MasterList/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MasterList/Edit/5
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

        // GET: MasterList/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MasterList/Delete/5
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
