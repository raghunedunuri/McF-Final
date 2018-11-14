using System.Web;
using System.Web.Optimization;

namespace McF
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.csv.min.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.bootstrap.js",
                        "~/Scripts/csv_to_html_table.js",
                        "~/Scripts/excel-formula.min.js",
                        "~/Scripts/jquery.csv.min.js",
                        "~/Scripts/jquery.jcalendar.js",
                        "~/Scripts/jquery.jexcel.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/site.js",
                      "~/Scripts/bootstrap-datepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                      "~/Scripts/angular.min.js",
                      "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                      "~/Scripts/angular-resource.js",
                      "~/Scripts/app/app.js",
                      "~/Scripts/app/common.services/dtnservice.js",
                      "~/Scripts/app/common.services/jobservice.js",
                      "~/Scripts/app/common.services/ethanolservice.js",
                      "~/Scripts/app/common.services/usweeklyservice.js",
                      "~/Scripts/app/common.services/cocoaservice.js",
                      "~/Scripts/app/common.services/sugarservice.js",
                      "~/Scripts/app/common.services/wasdeworldservice.js",
                      "~/Scripts/app/common.services/wasdedomesticservice.js",
                      "~/Scripts/app/common.services/cropprogressservice.js",
                      "~/Scripts/app/common.services/broilerservice.js",
                      "~/Scripts/app/common.services/commonservice.js",
                      "~/Scripts/app/common.services/cattleservice.js",
                      //"~/Scripts/app/common.services/chickenservice.js",
                      //"~/Scripts/app/common.services/hogsservice.js",
                      "~/Scripts/app/common.services/fatsservice.js",
                      "~/Scripts/app/common.services/cotservice.js",
                      "~/Scripts/app/common.services/dataviewservice.js",
                      "~/Scripts/app/dtn/dtncontroller.js",
                      "~/Scripts/app/ethanol/ethanolcontroller.js",
                      "~/Scripts/app/usweekly/usweeklycontroller.js",
                      "~/Scripts/app/home/homecontroller.js",
                      "~/Scripts/app/cot/cotcontroller.js",
                      "~/Scripts/app/sugar/sugarcontroller.js",
                      "~/Scripts/app/cocoa/cocoacontroller.js",
                      "~/Scripts/app/cropprogress/cropprogresscontroller.js",
                      "~/Scripts/app/wasdeworld/wasdeworldcontroller.js",
                      "~/Scripts/app/broiler/broilercontroller.js",
                      "~/Scripts/app/cattle/cattlecontroller.js",
                      "~/Scripts/app/chicken/chickencontroller.js",
                      "~/Scripts/app/fats/fatscontroller.js",
                      "~/Scripts/app/hogs/hogscontroller.js",
                      "~/Scripts/app/wasdedomestic/wasdedomesticcontroller.js",
                      "~/Scripts/app/datasource/datasourcecontroller.js",
                      "~/Scripts/app/layout/layoutcontroller.js",
                      "~/Scripts/app/dataview/dataviewcontroller.js",
                      "~/Scripts/ui-grid.js",
                      "~/Scripts/graphs/jquery.combobox.js",
                      "~/Scripts/graphs/jquery-ui.js",
                      "~/Scripts/jquery.blockUI.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/ui-bootstrap-csp.css",
                      "~/Content/fontawesome.css",
                      "~/Content/elegant-icons-style.css",
                      "~/Content/style.css",
                      "~/Content/line - icons.css",
                      "~/Content/ui-grid.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/ui-bootstrap-csp.css",
                      "~/Content/jquery.jexcel.bootstrap.css",
                      "~/Content/jquery.jexcel.css",
                      "~/Content/jquery.jexcel.green.css"));
        }
    }
}
