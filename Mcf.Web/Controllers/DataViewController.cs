using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcf.Controllers
{
    public class DataViewController : Controller
    {
        // GET: DataView
        public ActionResult Index(string datasource, string rootsource, string name)
        {
            ViewBag.DataSource = datasource;
            ViewBag.RootSource = rootsource;
            ViewBag.Name = name;
            return View();
        }
    }
}