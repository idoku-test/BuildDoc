using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CN.SGS.Expense.Common
{
    public class EmailPop3
    {
        #region 私有变量

        private string mstrHost = null; //主机名称或IP地址
        private int mintPort = 110; //主机的端口号（默认为110）
        private TcpClient mtcpClient = null; //客户端
        private NetworkStream mnetStream = null; //网络基础数据流
        private StreamReader m_stmReader = null; //读取字节流
        private string mstrStatMessage = null; //执行STAT命令后得到的消息（从中得到邮件数）
        private string mstrUser = string.Empty;
        private string mstrPassword = string.Empty;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>一个邮件接收对象</remarks>
        public EmailPop3()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host">主机名称或IP地址</param>
        public EmailPop3(string host)
        {
            mstrHost = host;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host">主机名称或IP地址</param>
        /// <param name="port">主机的端口号</param>
        /// <remarks>一个邮件接收对象</remarks>
        public EmailPop3(string host, int port, string user, string password)
        {
            mstrHost = host;
            mintPort = port;
            mstrUser = user;
            mstrPassword = password;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 主机名称或IP地址
        /// </summary>
        /// <remarks>主机名称或IP地址</remarks>
        public string HostName
        {
            get { return mstrHost; }
            set { mstrHost = value; }
        }

        /// <summary>
        /// 主机的端口号
        /// </summary>
        /// <remarks>主机的端口号</remarks>
        public int Port
        {
            get { return mintPort; }
            set { mintPort = value; }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 向网络访问的基础数据流中写数据（发送命令码）
        /// </summary>
        /// <param name="netStream">可以用于网络访问的基础数据流</param>
        /// <param name="command">命令行</param>
        /// <remarks>向网络访问的基础数据流中写数据（发送命令码）</remarks>
        private void WriteToNetStream(ref NetworkStream netStream, String command)
        {
            string strToSend = command + "\r\n";
            byte[] arrayToSend = System.Text.Encoding.ASCII.GetBytes(strToSend.ToCharArray());
            netStream.Write(arrayToSend, 0, arrayToSend.Length);
        }

        /// <summary>
        /// 检查命令行结果是否正确
        /// </summary>
        /// <param name="message">命令行的执行结果</param>
        /// <param name="check">正确标志</param>
        /// <returns>
        /// 类型：布尔
        /// 内容：true表示没有错误，false为有错误
        /// </returns>
        /// <remarks>检查命令行结果是否有错误</remarks>
        private bool CheckCorrect(string message, string check)
        {
            if (message.IndexOf(check) == -1)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 邮箱中的未读邮件数
        /// </summary>
        /// <param name="message">执行完LIST命令后的结果</param>
        /// <returns>
        /// 类型：整型
        /// 内容：邮箱中的未读邮件数
        /// </returns>
        /// <remarks>邮箱中的未读邮件数</remarks>
        private int GetMailNumber(string message)
        {
            string[] strMessage = message.Split(' ');
            return strMessage.Length - 2;
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 与主机建立连接
        /// </summary>
        /// <returns>
        /// 类型：布尔
        /// 内容：连接结果（true为连接成功，false为连接失败）
        /// </returns>
        /// <remarks>与主机建立连接</remarks>
        public bool Connect()
        {
            bool result = true;
            if (mstrHost == null)
            {
                return false;
            }
            if (mintPort == 0)
            {
                return false;
            }
            try
            {
                mtcpClient = new TcpClient(mstrHost, mintPort);
                mnetStream = mtcpClient.GetStream();
                m_stmReader = new StreamReader(mtcpClient.GetStream());

                string strMessage = m_stmReader.ReadLine();
                if (CheckCorrect(strMessage, "+OK") == false)
                    return false;
                //用户名
                strMessage = ExecuteCommand("USER " + mstrUser) + "\r\n";
                if (strMessage == "Error")
                {
                    return false;
                }
                //密码
                strMessage = ExecuteCommand("PASS " + mstrPassword) + "\r\n";
                if (strMessage == "Error")
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.Error(ex);
            }
            return result;
        }

        #region Pop3命令

        /// <summary>
        /// 执行Pop3命令，并检查执行的结果
        /// </summary>
        /// <param name="command">Pop3命令行</param>
        /// <returns>
        /// 类型：字符串
        /// 内容：Pop3命令的执行结果
        /// </returns>
        private string ExecuteCommand(string command)
        {
            string strMessage = null; //执行Pop3命令后返回的消息

            try
            {
                //发送命令
                WriteToNetStream(ref mnetStream, command);

                //读取多行
                if (command.Substring(0, 4).Equals("LIST") || command.Substring(0, 4).Equals("RETR") || command.Substring(0, 4).Equals("UIDL")) //记录STAT后的消息（其中包含邮件数）
                {
                    strMessage = ReadMultiLine();

                    if (command.Equals("LIST")) //记录LIST后的消息（其中包含邮件数）
                        mstrStatMessage = strMessage;
                }
                //读取单行
                else
                    strMessage = m_stmReader.ReadLine();

                //判断执行结果是否正确
                if (CheckCorrect(strMessage, "+OK"))
                    return strMessage;
                else
                    return "Error";
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return "Error";
            }
        }

        /// <summary>
        /// 在Pop3命令中，LIST、RETR和UIDL命令的结果要返回多行，以点号（.）结尾，
        /// 所以如果想得到正确的结果，必须读取多行
        /// </summary>
        /// <returns>
        /// 类型：字符串
        /// 内容：执行Pop3命令后的结果
        /// </returns>
        private string ReadMultiLine()
        {
            string strMessage = m_stmReader.ReadLine();
            string strTemp = string.Empty;
            while (strMessage != ".")
            {
                strTemp = strTemp + strMessage + "\r\n";
                strMessage = m_stmReader.ReadLine();
            }
            return strTemp;
        }

        /// <summary>
        /// 实现多种编码方式的转换
        /// </summary>
        /// <param name="str">要转换的字符</param>
        /// <param name="From">从哪种方式转换，如UTF-8</param>
        /// <param name="To">转换成哪种编码,如GB2312</param>
        /// <returns>转换结果</returns>
        private string ConvertStr(string str, string From, string To)
        {
            byte[] bs = System.Text.Encoding.GetEncoding(From).GetBytes(str);
            bs = System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(From), System.Text.Encoding.GetEncoding(To), bs);
            string res = System.Text.Encoding.GetEncoding(To).GetString(bs);
            return res;
        }

        //USER命令
        private string USER(string user)
        {
            return ExecuteCommand("USER " + user) + "\r\n";
        }

        //PASS命令
        private string PASS(string password)
        {
            return ExecuteCommand("PASS " + password) + "\r\n";
        }

        //LIST命令
        private string LIST()
        {
            return ExecuteCommand("LIST") + "\r\n";
        }

        //UIDL命令
        private string UIDL()
        {
            return ExecuteCommand("UIDL") + "\r\n";
        }

        //NOOP命令
        private string NOOP()
        {
            return ExecuteCommand("NOOP") + "\r\n";
        }

        //STAT命令
        private string STAT()
        {
            return ExecuteCommand("STAT") + "\r\n";
        }

        //RETR命令
        private string RETR(int number)
        {
            return ExecuteCommand("RETR " + number.ToString()) + "\r\n";
        }

        //DELE命令
        private string DELE(int number)
        {
            //删除是从序号0开始
            int no = number + 1;
            return ExecuteCommand("DELE " + no.ToString()) + "\r\n";
        }

        //QUIT命令
        private void Quit()
        {
            WriteToNetStream(ref mnetStream, "QUIT");
        }

        #endregion

        /// <summary>
        /// 断开所有与服务器的会话
        /// </summary>
        /// <remarks>断开所有与服务器的会话</remarks>
        public void DisConnect()
        {
            try
            {
                Quit();
                if (m_stmReader != null)
                    m_stmReader.Close();
                if (mnetStream != null)
                    mnetStream.Close();
                if (mtcpClient != null)
                    mtcpClient.Close();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        /// <summary>
        /// 删除邮件
        /// </summary>
        /// <param name="number">邮件号,从1开始</param>
        public bool DeleteMail(int number)
        {
            if (DELE(number).Equals("Error"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取邮件总数
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfEmail()
        {
            string str = STAT();
            if (str == "Error")
            {
                return 0;
            }
            string[] splitString = str.Split(' ');
            int count = int.Parse(splitString[1]);
            return count;
        }

        /// <summary>
        /// LIST指令
        /// </summary>
        /// <returns></returns>
        public string ShowEmailDetail()
        {
            return LIST();
        }

        /// <summary>
        /// 读取邮件内容
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string GetEmailContent(int number)
        {
            return RETR(number);
        }

        #endregion
    }
}
