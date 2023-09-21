using System.Web;
using System.Web.Optimization;

namespace RSI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // bundling and minification when web.config's debug is set to false

            bundles.Add(new StyleBundle("~/bundles/style/print")
                                .Include("~/Content/Css/fullcalendar.print.min.css")   /* fullcalendar */
                        );

            bundles.Add(new StyleBundle("~/bundles/style")
                            .Include(
                                  "~/Content/Css/bootstrap.min.css"            /* Bootstrap 3.3.7 */
                                , "~/Content/Css/AdminLTE.min.css"             /* Theme style */
                                , "~/Content/Css/auo.css"                      /* Theme style */
                                , "~/Content/Css/skins/skin-blue-light.css"    /* 選擇版型色系 -- skin-blue.css,  skin-navy.css, skin-yellow.css,  skin-red.css, skin-green.css -- */
                                , "~/Content/Css/css.css"
                            )
                            .Include("~/Content/Css/font-awesome.min.css", new CssRewriteUrlTransformWrapper()) /* Font Awesome 4.7.0 */

                        );

            bundles.Add(new ScriptBundle("~/bundles/script")
                            .Include(
                                  "~/Content/Scripts/jquery.js"                   /* jQuery 3.2.1 */
                                , "~/Content/Third-Part/JQueryUI/jquery-ui.min.js"
                                , "~/Content/Scripts/bootstrap.min.js"            /* Bootstrap 3.3.7 */
                                , "~/Content/Scripts/adminlte.js"                 /* AdminLTE App */
                            )
                        );

            // login page
            bundles.Add(new StyleBundle("~/bundles/login/style")
                            .Include(
                                  "~/Content/Css/bootstrap.min.css"            /* Bootstrap 3.3.7 */
                                , "~/Content/Css/AdminLTE.min.css"             /* Theme style */
                                , "~/Content/Css/auo.css"                      /* Theme style */
                                , "~/Content/Css/skins/skin-blue-light.css"    /* 選擇版型色系 -- skin-blue.css,  skin-navy.css, skin-yellow.css,  skin-red.css, skin-green.css -- */
                                , "~/Content/Css/iCheck/square/blue.css"       /* iCheck */
                                , "~/Content/Css/css.css"
                            )
                            .Include("~/Content/Css/font-awesome.min.css", new CssRewriteUrlTransformWrapper()) /* Font Awesome 4.7.0 */
            );
            bundles.Add(new ScriptBundle("~/bundles/login/script")
                            .Include(
                                  "~/Content/Scripts/jquery.js"        /* jQuery 3.2.1 */
                                , "~/Content/Scripts/bootstrap.min.js" /* Bootstrap 3.3.7 */
                                , "~/Content/Scripts/icheck.min.js"    /* iCheck */
                            )
                        );




            //overwrite web.config setting, force minification and bundling
            //BundleTable.EnableOptimizations = true;
        }


        // fix wrong path to fontawesome font files
        // from: https://stackoverflow.com/questions/19765238/cssrewriteurltransform-with-or-without-virtual-directory
        public class CssRewriteUrlTransformWrapper : IItemTransform
        {
            public string Process(string includedVirtualPath, string input)
            {
                return new CssRewriteUrlTransform().Process("~" + System.Web.VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
            }
        }
    }
}
