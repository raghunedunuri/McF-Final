using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace McF.Controllers
{
    
    public class USWeeklyController : Controller
    {
        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Data Source/Weekly Exports")]
        // GET: USWeekly
        public ActionResult Index()
        {
            return View();
        }

        // GET: USWeekly/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: USWeekly/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: USWeekly/Create
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

        // GET: USWeekly/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: USWeekly/Edit/5
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

        // GET: USWeekly/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: USWeekly/Delete/5
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
