using BuildDoc.Entities.Forms;
using BuildDoc.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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

        private IFormLogic FormInstance
        {
            get { return FormLogic.CreateInstance(); }
        }


        public ActionResult Index()
        {
            return View();
        }

        // GET: Report
        public ActionResult GetMontherSet()
        {
            int customer = 389;
            var lstMethodSet = BuildWordInstance.GetMotherSetByCustomer(customer, 40016003); ;
        
            return Json(lstMethodSet, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadDoc(decimal masterId)
        {

            string error = "";
            //报告构建
            //"{\"Header\":[{\"Key\":\"5\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"1.1封面-抵押\"},{\"Key\":\"6\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"2.1致函-抵押\"},{\"Key\":\"29\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"3.1-目录\"},{\"Key\":\"7\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"4.1估价师声明-抵押\"},{\"Key\":\"8\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"5.1假限-抵押\"}],\"Detail\":[{\"Key\":\"30\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"1-委托方、估价方\"},{\"Key\":\"31\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"2-目的－抵押\"},{\"Key\":\"32\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"3-估价对象-商品住宅-\"},{\"Key\":\"33\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"4时点\"},{\"Key\":\"34\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"5-价值类型-抵押\"},{\"Key\":\"35\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"6-原则-抵押\"},{\"Key\":\"36\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"7依据-抵押-住宅\"},{\"Key\":\"37\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"8-估价方法\"},{\"Key\":\"38\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"9-结果-抵押\"},{\"Key\":\"39\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"10-查勘期、作业期、有\"},{\"Key\":\"40\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"11-优先受偿-变现能力\"},{\"Key\":\"41\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"11T-项目独有风险\"},{\"Key\":\"93\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"12附件目录\"}],\"Footer\":[{\"Key\":\"42\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"1-估价对象描述分析-住\"},{\"Key\":\"43\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"2-市场背景分析-住宅－\"},{\"Key\":\"44\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"3-最高最佳利用分析-简\"},{\"Key\":\"45\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"4-估价方法介绍\"},{\"Key\":\"46\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"5-适用的方法\"},{\"Key\":\"47\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"6.不适用的方法\"},{\"Key\":\"48\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"7A-比较法-住(商品)\"},{\"Key\":\"94\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"收益法(商品住宅)\"},{\"Key\":\"50\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"8-测算过程-估价结果确\"},{\"Key\":\"56\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"9-附件目录-项目相关\"},{\"Key\":\"57\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"10A附件-项目相关\"},{\"Key\":\"92\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"10B附件-常规类\"}]}"
            //string st = "{\"Header\":[{\"Key\":\"2\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"简单版预估-头部\"}],"
            //    +"\"Detail\":[{\"Key\":\"3\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"简单版预估-中部\"}],"
            //    +"\"Footer\":[{\"Key\":\"4\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"简单版预估-尾部\"},"
            //    + "{\"Key\": \"308\",\"Type\": \"Config\",\"Condition\": \"\",\"Name\": \"2-单标-比较法-农-预\"},"
            //    + "{\"Key\": \"256\",\"Type\": \"Config\",\"Condition\": \"\",\"Name\": \"位置图及照片\"}"
            //    +"]}";
            string st = "{\"Header\":[{\"Key\":\"5\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"1.1封面-抵押\"},{\"Key\":\"6\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"2.1致函-抵押\"},{\"Key\":\"29\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"3.1-目录\"},{\"Key\":\"7\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"4.1估价师声明-抵押\"},{\"Key\":\"8\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"5.1假限-抵押\"}],\"Detail\":[{\"Key\":\"30\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"1-委托方、估价方\"},{\"Key\":\"31\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"2-目的－抵押\"},{\"Key\":\"3016\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"3.0.1.2.0-估价对象-工业-简-\"},{\"Key\":\"33\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"4时点\"},{\"Key\":\"34\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"5-价值类型-抵押\"},{\"Key\":\"35\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"6-原则-抵押\"},{\"Key\":\"36\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"7依据-抵押-住宅\"},{\"Key\":\"37\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"8-估价方法\"},{\"Key\":\"38\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"9-结果-抵押\"},{\"Key\":\"39\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"10-查勘期、作业期、有\"},{\"Key\":\"3015\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"11.1.2.0.0-优先受偿-变现能力\"}],\"Footer\":[{\"Key\":\"3014\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"1.0.2.1.0-估价对象描述分析-工\"},{\"Key\":\"3041\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"2104100-市场背景分析\"},{\"Key\":\"44\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"3-最高最佳利用分析-简\"},{\"Key\":\"45\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"4-估价方法介绍\"},{\"Key\":\"46\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"5-适用的方法\"},{\"Key\":\"47\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"6.不适用的方法\"},{\"Key\":\"4337\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"7C.0.4.2.0-测算过程-成本法－\"},{\"Key\":\"50\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"8-测算过程-估价结果确\"},{\"Key\":\"57\",\"Type\":\"Config\",\"Condition\":\"\",\"Name\":\"10A附件-项目相关\"}]}";
            DocMaster master = CreateDocMaster(792, 25476, st, 0, 143, 389, false, ref error, false, 40016003, 1);
            Stream stream = master.BuildDoc();

            string path = ConfigurationManager.AppSettings["DocPath"]; ;
            string virtualPah = Server.MapPath(path);

            string docPath = Path.Combine(virtualPah, "a.doc");

            using (FileStream file = new FileStream(docPath, FileMode.Create))
            {
                stream.CopyTo(file);
            }           
           // return File(stream, "application/msword", "a.doc");//xls文件
            return File(docPath, "application/msword");
        }

        public DocMaster CreateDocMaster(decimal masterID, decimal objectID, string jsonStructure,
          decimal instanceDocumentID, int currentUser, int customer_id, bool isBuildDoc, ref string error,
          bool? isReduction, int? businessType, int? objectIndex)
        {
            string returnJSON = "";
            bool isSearch = true;
            //查询数据库是否已保存示例
            //isReduction为空的情况下是下载   1:预估函重新获取的时候，不需要传值；   2：报告 有实例的情况下：如果重新获取，即要拿预估函的信息；非重新获取，即拿本报告的
            //                                                                               无实例的情况下：拿预估函的实例   
          
            string conCode = "", buildingCode = "", houseCode = "", unitCode = "";
            var formView = FormInstance.GetFormDataByObjectId(Convert.ToInt32(objectID), 1);
            if (formView.LABLELIST != null)
            {
                List<FormLabelDTO> lstLabel = formView.LABLELIST.ToList();

                //查询对应的楼盘，楼栋，房号的code
                conCode = FormInstance.GetValFormList(lstLabel, "CONSTRUCTION_CODE");
                buildingCode = FormInstance.GetValFormList(lstLabel, "BUILDING_CODE");
                houseCode = FormInstance.GetValFormList(lstLabel, "HOUSE_CODE");
                unitCode = FormInstance.GetValFormList(lstLabel, "UNIT_CODE");
            }


            try
            {
                Dictionary<string, string> inputParams = new Dictionary<string, string>();
                inputParams.Add("ConstructionCode", conCode == null ? "" : conCode);
                inputParams.Add("BuildingCode", buildingCode == null ? "" : buildingCode);
                inputParams.Add("HouseCode", houseCode == null ? "" : houseCode);
                inputParams.Add("UnitCode", unitCode == null ? "" : unitCode);
                inputParams.Add("ObjectType", formView.OBJECT_TYPE_ID.ToString());
                inputParams.Add("CurrentUser", currentUser.ToString());
                inputParams.Add("CustomerID", customer_id.ToString());
                inputParams.Add("标的物顺序", objectIndex.ToString());
                //IsReduction 重新获取时 预估函:不需要查预估函   报告：需要查预估
                inputParams.Add("IsGetEstimate", isSearch.ToString());
                DocMaster docmaster = new DocMaster(masterID, objectID, returnJSON, jsonStructure, BuildWordInstance.GetValue, inputParams, isBuildDoc);
                return docmaster;
            }
            catch (Exception ex)
            {
                //throw ex;
                error = ex.Message;
                return null;
            }

        }
    }
}