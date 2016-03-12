using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CN.SGS.Expense.Common
{
    /// <summary>
    /// Email Entiry
    /// </summary>
    [DataContract]
    public class EmailEntity
    {
        [DataMember]
        public int MailID
        {
            get;
            set;
        }

        [DataMember]
        public string Subject
        {
            get;
            set;
        }

        [DataMember]
        public string Body
        {
            get;
            set;
        }

        [DataMember]
        public MailAddressCollection To
        {
            get;
            set;
        }

        [DataMember]
        public MailAddressCollection Cc
        {
            get;
            set;
        }

        [DataMember]
        public MailAddressCollection Bcc
        {
            get;
            set;
        }

        /// <summary>
        /// name,email;name,email...
        /// </summary>
        [DataMember]
        public string StringTo
        {
            get;
            set;
        }

        /// <summary>
        /// name,email;name,email...
        /// </summary>
        [DataMember]
        public string StringCC
        {
            get;
            set;
        }


        [DataMember]
        public List<string> Attachments
        {
            get;
            set;
        }


        [DataMember]
        public string Remark
        {
            get;
            set;
        }

        [DataMember]
        public int MailStatus
        {
            get;
            set;
        }

        /// <summary>
        /// CreatedBy
        /// </summary>
        [DataMember]
        public string CreatedBy
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? CreatedDate
        {
            get;
            set;
        }

    }
}
