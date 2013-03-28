using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;
using DataModels;

namespace SearchBoxPlugin.Controllers
{
    public class SearchController : Controller
    {
        public string EmbeddedViewPath
        {
            get
            {
                return string.Format("~/Plugins/SearchBoxPlugin.dll/SearchBoxPlugin.Views.{0}.{1}.ascx",
                     this.ControllerContext.RouteData.Values["controller"],
                      this.ControllerContext.RouteData.Values["action"]);
            }
        }
       
        //
        // GET: /Search/

        public ActionResult SearchView(string folderPath, string searchString)
        {
            ViewBag.SearchFolder = folderPath;
            ViewBag.SearchString = searchString;
            return PartialView(this.EmbeddedViewPath);
            
        }

        [GridAction]
        public ActionResult SearchResult(string searchFolder, string searchString)
        {
            IList<FileModel> result = FileModel.Search(searchFolder, searchString);

            return View(new GridModel<FileModel>
            {
                Total = result.Count,
                Data = result
            });
        }

    }
}
