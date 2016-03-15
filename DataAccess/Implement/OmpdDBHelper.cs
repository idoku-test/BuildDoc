using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DataAccess
{
    public class OmpdDBHelper : BaseDB
    {
        protected override string connectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["OmpdDB"].ConnectionString;
            }
        }
    }
}
