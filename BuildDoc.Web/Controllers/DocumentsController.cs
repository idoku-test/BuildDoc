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
    public class DocumentsController : Controller
    {

        private IBuildDocLogic BuildWordInstance
        {
            get { return BuildDocLogic.CreateInstance(); }
        }


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

        /// <summary>
        /// 获得该公司所有构建的列表
        /// </summary>
        /// <param name="stype"></param>
        /// <param name="dtype"></param>
        /// <returns></returns>
        public JsonResult GetStructures(int stype, int dtype)
        {
            decimal customerId = 3;//
            var list = BuildWordInstance.GetStructuresByCustomer(customerId, dtype, stype);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取构件信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult GetStructureInfo(int Id)
        {
            var info = BuildWordInstance.GetStructure(Id);
            return Json(info, JsonRequestBehavior.AllowGet);
        }

	}
}