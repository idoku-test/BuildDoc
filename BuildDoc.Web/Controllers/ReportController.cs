using BuildDoc.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildDoc.Web.Controllers
{
    public class ReportController : Controller
    {
        private IBuildDocLogic BuildWordInstance
        {
            get { return BuildDocLogic.CreateInstance(); }
        }


        public ActionResult Index()
        {
            return View();
        }

        // GET: Report
        public ActionResult GetMontherSet()
        {
            int customer = 389;
            var lstMethodSet = BuildWordInstance.GetMotherSetByCustomer(customer, 40016004); ;
        
            return Json(lstMethodSet, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadDoc(decimal masterId)
        {
            DocMaster master = new DocMaster(masterId, 0, "", false);
            Stream stream = master.BuildDoc();
            return File(stream, "application/vnd.ms-word", "a.doc");//xls文件

        }
    }
}