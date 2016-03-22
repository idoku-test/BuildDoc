using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildDoc.Web.Controllers
{
    public class LabelsController : Controller
    {
        //
        // GET: /Labels/
        public ActionResult Index()
        {
            string path = ConfigurationManager.AppSettings["DocPath"]; ;
            string virtualPah = Server.MapPath(path);

            string docPath = Path.Combine(virtualPah, "test19.doc");
            IBuildWord bword = new BuildWord();
            bword.Load(docPath);
            var marks = bword.GetAllMarks();

            //标签类型
            
            return View(marks);
        }

        public ActionResult Condition()
        {

            return View();
        }

	}
}