﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace McF.Controllers
{
    
    public class ChickenController : Controller
    {
        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Data Source/Chicken & Eggs")]
        // GET: COT
        public ActionResult Index()
        {
            return View();
        }

        // GET: COT/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: COT/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: COT/Create
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

        // GET: COT/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: COT/Edit/5
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

        // GET: COT/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: COT/Delete/5
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
