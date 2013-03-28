using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModels;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace ServerExplorer.Controllers
{
    public class ExplorerController : Controller
    {
        //
        // GET: /Explorer/

        public ActionResult Index()
        {
            //IList<FileModel> files = FileModel.GetFiles(null);
            return View(new FileModel());
        }

        public ActionResult FileList(string folderPath)
        {
            folderPath = FileModel.Encode(folderPath);
            DirectoryInfo di = new DirectoryInfo(folderPath);
            return PartialView(new FileModel(di));
        }

        public ActionResult LoadActionLinks(string path, List<string> list)
        {
            if (list != null && !string.IsNullOrEmpty(list[0]))
                ViewData["CheckedList"] = list;
            if (path == "" || path == "/")
                return PartialView("ActionLinks", new FileModel());
            else
            {
                DirectoryInfo di = new DirectoryInfo(path);
                return PartialView("ActionLinks", new FileModel(di));
            }
        }

        public JsonResult GetFiles(string path)
        {
            IList<FileModel> files = FileModel.GetFiles(path);
            return new JsonResult() { Data = files };
        }

        public ActionResult DownloadFile(string file)
        {
            ZipResult result = new ZipResult();
            result.AddFile(FileModel.Decode(file));
            return result;
        }

        public ActionResult DownloadFiles(string jlist)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<string> list = serializer.Deserialize<List<string>>(jlist);
            ZipResult result = new ZipResult();
            foreach (string path in list)
            {
                result.AddFile(FileModel.Decode(path));
            }

            return result;
        }


        public JsonResult GoUpper(string path)
        {
            string filePath = FileModel.Decode(path);

            if (filePath != "\\")
                if (Directory.Exists(filePath))
                {
                    DirectoryInfo di = new DirectoryInfo(filePath);
                    if (di.Parent != null)
                    {
                        filePath = FileModel.Encode(di.Parent.FullName);
                    }
                    else
                        filePath = "/";
                }
                else
                    filePath = "";
            return new JsonResult() { Data = filePath };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubFoldersLoading(TreeViewItem node)
        {
            string filePath = node.Value;
            IList<FileModel> subFolders = FileModel.GeFolders(filePath);
            IEnumerable nodes = from item in subFolders
                                select new TreeViewItem
                                {
                                    Text = item.Name,
                                    Value = item.FullPath,
                                    ImageUrl = Url.Content("~/Content/Images/" + item.Category.ToString() + ".png"),
                                    LoadOnDemand = true,
                                    Enabled = true
                                };
            return new JsonResult { Data = nodes };
        }

        [GridAction]
        public ActionResult SelectFiles(string filePath)
        {
            IList<FileModel> files = FileModel.GetFiles(filePath == "/" ? "" : filePath);

            return View(new GridModel<FileModel>
            {
                Total = files.Count,
                Data = files
            });
        }

        [HttpPost]
        public ActionResult LocationLoading(string filePath)
        {
            filePath = FileModel.Decode(filePath);
            SelectList result = null;
            if (filePath != "\\")
            {
                if (Directory.Exists(filePath))
                {
                    DirectoryInfo di = new DirectoryInfo(filePath);
                    if (di.Parent != null)
                    {
                        filePath = FileModel.Encode(di.Parent.FullName);
                    }
                    else
                        filePath = "";
                }
                else
                    filePath = "";
                IList<FileModel> files = FileModel.GeFolders(filePath);
                result = new SelectList(files, "FullPath", "FullName", filePath); 
            }
            else
            {
                result  = new SelectList(new[]
                            {
                                new { Value = "/", Name = "Computer" },
                            }
                   , "Value", "Name", filePath);

            }

              return new JsonResult { Data = result };
         }

    }
}
