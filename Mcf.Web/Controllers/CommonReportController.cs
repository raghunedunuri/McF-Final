using McF.Business;
using McF.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcf.Controllers
{
    public class CommonReportController : Controller
    {
        private IDTNService dtnservice;
        private ISugarService sugarService;
        public CommonReportController(IDTNService dtnservice,ISugarService sugarService)
        {
            this.dtnservice = dtnservice;
            this.sugarService = sugarService;
        }
        //public CommonReportController(ISugarService sugarService)
        //{
        //    this.sugarService = sugarService;
        //}

        // GET: Reports
        
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult SugarCompanies(SugarCompaniesData sugarData)
        {
            List<SugarCompaniesData> sugarCompanies = sugarService.GetSugarCompaniesData();
            
            var json = JsonConvert.SerializeObject(sugarCompanies);
            ViewBag.SugarCompanies = json;
            
            return View();
        }

        
        public ActionResult CornStocks()
        {
            return View();
        }

        
        public ActionResult CornSyrupDemand()
        {
            return View();
        }

        
        public ActionResult MexicanSugarImports()
        {
            DataSet ds = sugarService.GetMexicanSugarReportData();
            List<string> year = new List<string>();
            List<double> refined = new List<double>();
            List<double> estandar = new List<double>();
            List<double> total = new List<double>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                //data = row[column].ToString();
                year.Add(row[0].ToString());
                refined.Add(Convert.ToDouble(row[1]));
                estandar.Add(Convert.ToDouble(row[2]));
                total.Add(Convert.ToDouble(row[3]));
            }

            ViewBag.year = year;
            ViewBag.Refined = refined;
            ViewBag.Estandar = estandar;
            ViewBag.Total = total;
            return View();
        }

        
        public ActionResult SugarMarket()
        {
            DataSet sugarRegion = sugarService.GetUSMarketData();
            List<int> years = new List<int>();
            List<string> refinaries = new List<string>();
            Dictionary<string, List<float>> matrix = new Dictionary<string, List<float>>();
            foreach (DataRow row in sugarRegion.Tables[0].Rows)
            {
                if (!years.Contains(Convert.ToInt16(row[1])))
                {
                    years.Add(Convert.ToInt16(row[1]));
                }
                if (!refinaries.Contains(row[0].ToString()))
                {
                    refinaries.Add(row[0].ToString());
                }

            }
            int i = 0;
            foreach (int year in years)
            {
                List<float> values_temp = new List<float>();
                //foreach (string month in months)
                //{
                foreach (DataRow row in sugarRegion.Tables[0].Rows)
                {
                    if (Convert.ToInt16(row[1]) == year)
                    {
                        values_temp.Add(Convert.ToSingle(row[2]));
                    }
                }
                matrix.Add(i.ToString(), values_temp);
                i++;
                //}
            }
            ViewBag.Years = years;
            ViewBag.Refinary = refinaries;
            ViewBag.FirstYear = matrix["0"];
            ViewBag.SecondYear = matrix["1"];
            return View();
        }

        [HttpGet]
        
        public ActionResult CornWetMills()
        {
            List<SugarCompaniesData> cornWetMills = sugarService.GetCornWetMillsData();
            List<int> years = sugarService.GetYears();
            var json = JsonConvert.SerializeObject(cornWetMills);
            ViewBag.CornWetMills = json;
            ViewBag.Years = years;
            return View();
        }

        [HttpPost]
        public ActionResult CornWetMills(Years year)
        {
            return View();
        }

        
        public ActionResult BeetCaneSugarDeliveries()
        {
            DataSet ds = new DataSet();
            ds = dtnservice.GetSugarReportData();
            List<string> year = new List<string>();
            List<double> dd_beet = new List<double>();
            List<double> dd_cane = new List<double>();
            List<double> dd_nonreporters = new List<double>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                //data = row[column].ToString();
                year.Add(row[0].ToString());
                dd_beet.Add(Convert.ToDouble(row[1]));
                dd_cane.Add(Convert.ToDouble(row[2]));
                dd_nonreporters.Add(Convert.ToDouble(row[3]));
            }

            ViewBag.year = year;
            ViewBag.dd_beet = dd_beet;
            ViewBag.dd_cane = dd_cane;
            ViewBag.dd_nonreporters = dd_nonreporters;
            
            return View();

        }

        
        public ActionResult HFCSDemand()
        {
            DataSet sugarRegion = sugarService.GetHFCSDemand();
            List<int> years = new List<int>();
            List<float> value = new List<float>();
            foreach (DataRow row in sugarRegion.Tables[0].Rows)
            {
                years.Add(Convert.ToInt16(row[0]));
                value.Add(Convert.ToSingle(row[1]));
            }
            ViewBag.Years = years;
            ViewBag.Value = value;
            return View();
        }

        
        public ActionResult SugarPrices()
        {
            DataSet sugarRegion = sugarService.GetSugarPrices();
            List<int> years = new List<int>();
            List<float> refine = new List<float>();
            List<float> price = new List<float>();
            List<float> spread = new List<float>();
            foreach (DataRow row in sugarRegion.Tables[0].Rows)
            {
                //if (!years.Contains(Convert.ToInt16(row[0])))
                //{
                    years.Add(Convert.ToInt16(row[0]));
                //}
                refine.Add(Convert.ToSingle(row[1]));
                price.Add(Convert.ToSingle(row[2]));
                spread.Add(Convert.ToSingle(row[3]));
            }
            ViewBag.Years = years;
            ViewBag.Refine = refine;
            ViewBag.Price = price;
            ViewBag.Spread = spread;
            return View();
        }

        
        public ActionResult SugarDeliveriesRegion()
        {
            DataSet sugarRegion = sugarService.GetSugarDelivaries();
            List<string> years = new List<string>();
            List<string> regions = new List<string>();
            Dictionary<string, List<float>> matrix = new Dictionary<string, List<float>>();
            foreach (DataRow row in sugarRegion.Tables[0].Rows)
            {
                if (!years.Contains(row[0].ToString()))
                {
                    years.Add(row[0].ToString());
                }
                if (!regions.Contains(row[1].ToString()))
                {
                    regions.Add(row[1].ToString());
                }

            }
            int i = 0;
            foreach (string region in regions)
            {
                List<float> values_temp = new List<float>();
                //foreach (string month in months)
                //{
                foreach (DataRow row in sugarRegion.Tables[0].Rows)
                {
                    if (row[1].ToString() == region)
                    {
                        values_temp.Add(Convert.ToSingle(row[2]));
                    }
                }
                matrix.Add(i.ToString(), values_temp);
                i++;
                //}
            }
            ViewBag.Years = years;
            ViewBag.Regions = regions;
            ViewBag.FirstRegion = matrix["0"];
            ViewBag.SecondRegion = matrix["1"];
            ViewBag.ThirdRegion = matrix["2"];
            ViewBag.FourthRegion = matrix["3"];
            ViewBag.FifthRegion = matrix["4"];
            return View();
        }

        
        public ActionResult HFCSDemandNorth()
        {
            return View();
        }

        
        public ActionResult WetMillsNorth()
        {
            List<NorthWetMills> cornWetMills = sugarService.GetNorthWetMillings();
            var json = JsonConvert.SerializeObject(cornWetMills);
            ViewBag.CornWetMills = json;
            return View();
        }

        
        public ActionResult HFCSDemandMonthly()
        {
            DataSet hfscDemand = sugarService.GetHFSCExports();
            List<int> years = new List<int>();
            List<string> months = new List<string>();
            Dictionary<string, List<float>> matrix = new Dictionary<string, List<float>>();
            foreach (DataRow row in hfscDemand.Tables[0].Rows)
            {
                if (!years.Contains(Convert.ToInt16(row[0])))
                {
                    years.Add(Convert.ToInt16(row[0]));
                }
                if (!months.Contains(row[2].ToString()))
                {
                    months.Add(row[2].ToString());
                }

            }
            int i = 0;
            foreach(int year in years)
            {
                List<float> values_temp = new List<float>();
                //foreach (string month in months)
                //{
                foreach (DataRow row in hfscDemand.Tables[0].Rows)
                    {
                        if((Convert.ToInt16(row[0])==year))
                        {
                            values_temp.Add(Convert.ToSingle(row[1]));
                        }
                    }
                matrix.Add(i.ToString(), values_temp);
                i++;
                //}
            }
            ViewBag.Years = years;
            ViewBag.Months = months;
            ViewBag.FirstYear = matrix["0"];
            ViewBag.SecondYear = matrix["1"];
            ViewBag.ThirdYear = matrix["2"];
            ViewBag.FourthYear = matrix["3"];
            ViewBag.FifthYear = matrix["4"];
            return View();
        }
    }
}
