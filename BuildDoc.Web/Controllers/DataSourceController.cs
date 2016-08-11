using BuildDoc.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildDoc.Web.Controllers
{
    public class DataSourceController : Controller
    {
        private IBuildDocLogic BuildWordInstance
        {           
            get { return BuildDocLogic.CreateInstance(); }
        }

        //
        // GET: /DataSource/
        public ActionResult GetDataSource()
        {
            //获取所有数据源
            var source = BuildWordInstance.GetDataSourceByType(-1);
            return Json(source, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLabelDealSource()
        {
            var source = BuildWordInstance.GetLabelDealSource();

            return Json(source, JsonRequestBehavior.AllowGet);
        }

        
	}
}