using McF.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcf.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/Corn")]
        public ActionResult Index()
        {
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/Corn")]
        public ActionResult Corn()
        {
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/Sugar Companies")]
        public ActionResult SugarCompanies()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "SugarCompanies";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/Corn Stocks")]
        public ActionResult CornStocks()
        {
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/Corn Syrup Demand")]
        public ActionResult CornSyrupDemand()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "CornSyrupDemand";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/U.S Imports of Mexican Sugar")]
        public ActionResult MexicanSugarImports()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "MexicanSugarImports";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/Sugar Market")]
        public ActionResult SugarMarket()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "SugarMarket";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/U.S Corn Wet Mills")]
        public ActionResult CornWetMills()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "CornWetMills";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/Beet Cane Sugar Deliveries")]
        public ActionResult BeetCaneSugarDeliveries()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "BeetCaneSugarDeliveries";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/HFCS Demand")]
        public ActionResult HFCSDemand()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "HFCSDemand";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/Sugar Prices")]
        public ActionResult SugarPrices()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "SugarPrices";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/Sugar Deliveries - Region")]
        public ActionResult SugarDeliveriesRegion()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "SugarDeliveriesRegion";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/HFCS Demand - North")]
        public ActionResult HFCSDemandNorth()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "HFCSDemandNorth";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/North American wet milling facilities")]
        public ActionResult WetMillsNorth()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "WetMillsNorth";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }

        [MvcBreadCrumbs.BreadCrumb(Clear = true, Label = "Reports/HFCS Demand Monthly")]
        public ActionResult HFCSDemandMonthly()
        {
            string url = @"http://mckeany.saven.in:8085/commonreport/" + "HFCSDemandMonthly";
            string myData = "<div><iframe src=\"" + url + "\" style=\"height:100%;width:100%;border:0px;\"></div>";
            ViewBag.embeddedText = myData;
            return View();
        }
    }
}
