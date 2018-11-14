using McF.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using McF.Contracts;

namespace McF.Controllers
{
    public class DTNController : Controller
    {
        private IDTNService dtnService;

        public DTNController()
        {
        }
        public DTNController(IDTNService dtnService)
        {
            this.dtnService = dtnService;
        }
        // GET: DTN
        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Data Source/Monthly Contracts")]
        public ActionResult Index()
        {
            ViewBag.JobSummary = new JobSummary { ScheduleToRun = 10, Inprogress = 20, CompletedJobs = 15, Failed = 2, NewSymbols = 3, NewRecords = 5, UpdatedRecords = 3, UnmappedRecords = 4 };
            return View();
        }

        // GET: DTN/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DTN/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DTN/Create
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

        // GET: DTN/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DTN/Edit/5
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

        // GET: DTN/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DTN/Delete/5
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
