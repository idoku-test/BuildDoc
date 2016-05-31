using BuildDoc.Entities.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Logic
{
    public interface IFormLogic
    {

        FormViewModel GetFormDataByObjectId(int ObjectId, int FormType);

        string GetValFormList(List<FormLabelDTO> lst, string Field);
    }
}
