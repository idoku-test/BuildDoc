using BuildDoc.Entities.Forms;
using DataAccess;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Logic
{
    public class FormLogic : IFormLogic
    {

        private static IFormLogic _instance;
        public static IFormLogic CreateInstance()
        {
            IApplicationContext context = ContextRegistry.GetContext();
            if (_instance == null && context != null)
                _instance = context.GetObject("IFormLogic") as IFormLogic;
            return _instance;
        }

        public FormViewModel GetFormDataByObjectId(int ObjectId, int FormType)
        {
            FormViewModel formInstance = new FormViewModel();
            FormLabelDTO label = new FormLabelDTO();
            Dictionary<string, object> dsDic = new Dictionary<string, object>();
            dsDic.Add("i_object_id", ObjectId);
            dsDic.Add("i_form_type", FormType);
            using (BaseDB db = new RedasDBHelper())
            {
                IList<FormViewModel> InstanceList = db.ExecuteListProc<FormViewModel>("pkg_forminstance.sp_get_formDataByObj", dsDic);
                if (InstanceList != null && InstanceList.Count > 0)
                {
                    formInstance = InstanceList[0];
                    //formInstance.LABLELIST = JsonTools.JsonToObject2<List<FormLabelDTO>>(formInstance.CONTENTS).OrderBy(l => l.SORT).ToList();
                    formInstance.LABLELIST = JsonTools.JsonToObject2<DFormViewModel>(formInstance.CONTENTS).FormLabels
                                            .Where(l => l.IsSelected == true)
                                            .OrderBy(l => l.SORT).ToList();
                    List<LabelDataDTO> lstData = new List<LabelDataDTO>();
                    if (!string.IsNullOrEmpty(formInstance.FORMDATA))
                    {
                        lstData = JsonTools.JsonToObject2<List<LabelDataDTO>>(formInstance.FORMDATA);
                    }

                    foreach (LabelDataDTO c in lstData)
                    {
                        decimal id = 0;
                        decimal.TryParse(c.NAME, out id);
                        if (id > 0)
                        {
                            label = formInstance.LABLELIST.SingleOrDefault(l => l.FORM_LABEL_ID == Convert.ToDecimal(c.NAME));
                            if (label == null)
                            {
                                continue;
                            }
                            label.CONTENT = c.VALUE;
                            //label.CONTENT =label.FIELD_TYPE == "DATE"?Convert.ToDateTime(c.VALUE): c.VALUE;
                        }
                    }
                }
            }
            return formInstance;

        }



        public string GetValFormList(List<FormLabelDTO> lst, string Field)
        {
            string val = "";
            var lstLabel = lst.Where(l => l.LABEL_NAME == Field);
            if (lstLabel != null && lstLabel.Count() > 0)
            {
                val = lstLabel.ToList()[0].CONTENT;
            }
            return val;

        }
    }
}
