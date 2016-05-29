using BuildDoc.Entities;
using BuildDoc.Logic;
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
        private IBuildDocLogic BuildWordInstance
        {
            get { return BuildDocLogic.CreateInstance(); }
        }

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
            if (Request.IsAjaxRequest()) {
                return View("_LabelsPartial", marks);
            }
            
            return View(marks);
        }

        public ActionResult Save(DataLabelModel model)
        {
            model.CREATED_BY = 1236;
            model.MODIFIED_BY = 1236;
            model.MODIFIED_TIME = DateTime.Now;
            model.CREATED_TIME = DateTime.Now;
            model.CUSTOMER_ID = 1;
            var result = BuildWordInstance.SaveLabel(model);
            return Json(new { IsSuccess = true, Message = "添加成功" }, "Text/html", JsonRequestBehavior.AllowGet);
        }

	}
}