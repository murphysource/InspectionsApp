using System.Web;
using System.Web.Optimization;

namespace InspectionsApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/jquery.dataTables.min.js",
                      "~/Scripts/dataTables.bootstrap5.js",
                      "~/Scripts/Chart.min.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/luxon.js",
                      "~/Scripts/daterangepicker.js",
                      "~/Scripts/bootstrap.bundle.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/dataTables.bootstrap5.css",
                      "~/Content/daterangepicker.css",
                      "~/Content/site.css"));
        }
    }
}
