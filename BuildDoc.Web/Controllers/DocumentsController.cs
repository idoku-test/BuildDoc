using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildDoc.Web.Controllers
{
    public class DocumentsController : Controller
    {
        //
        // GET: /Documents/
        public ActionResult Index()
        {           
            return View();
        }

        public JsonResult GetRemarks()
        {
            string path = ConfigurationManager.AppSettings["DocPath"]; ;
            string virtualPah = Server.MapPath(path);

            string docPath = Path.Combine(virtualPah, "test19.doc");
            IBuildWord bword = new BuildWord();
            bword.Load(docPath);
            var marks = bword.GetAllMarks();

            return Json(marks, JsonRequestBehavior.AllowGet);
        }
	}
}