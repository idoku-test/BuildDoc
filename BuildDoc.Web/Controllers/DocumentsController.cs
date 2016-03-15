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
	}
}