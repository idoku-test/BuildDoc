using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections;
using System.Threading;
using System.IO;
using System.Configuration;

namespace Common.News
{
    public class GetNews
    {
        #region 私有成员
        public string pageUrl;
        public bool homeOnly = false;
        public bool hasPrefix = true;
        public int pageWantToGet = 1;
        public bool hasManyPage = false;
        public string manyPageRegex;
        public string prefix;
        private List<string> pageUrls;
        public string pageUrlsRegex;
        public string titleRegex;
        public string timeRegex;
        public string bodyRegex;
        public string hrefRegex;
        public string fileSave;
        public string hostName;
        public string encoding;
        #endregion

        public void threadStart()
        {
            if (!prefix.EndsWith("/")) prefix += "/";
            ThreadStart ts = new ThreadStart(start);
            Thread th = new Thread(ts);
            th.Start();

        }
        private void start()
        {

            if (homeOnly)
            {
                getPageUrls(-1);
            }
            else
            {
                for (int i = 1; i <= pageWantToGet; i++)
                    getPageUrls(i);
            }
            startGetAll();
        }
        private void WriteFile(string str)
        {
            FileStream fs = new FileStream(fileSave, FileMode.Append);
            StreamWriter streamWriter = new StreamWriter(fs, System.Text.Encoding.GetEncoding("gb2312"));
            streamWriter.WriteLine(str);
            streamWriter.Flush();
            streamWriter.Close();
            fs.Close();
        }
        private void deleteTag(ref string str)
        {

            str = Regex.Replace(str, "<[\\s]*p[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*p[\\s]*?>", "\r\n");
            str = Regex.Replace(str, "<[\\s]*br[\\s]*/[\\s]*[^>]*>?>", "\r\n");
            str = Regex.Replace(str, "<[\\s]*br[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*br[^>]*>?>", "\r\n");
            str = Regex.Replace(str, "<[\\s]*a[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*a[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*img[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*img[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*strong[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*strong[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*div[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*div[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*b[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*b[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*span[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*span[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*script[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*script[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*li[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*li[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*img[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*img[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*style[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*style[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*i[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*i[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*h3[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*h2[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*h3[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*h2[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*font[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*font[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "<[\\s]*q[\\s]*[^>]*>?>", string.Empty);
            str = Regex.Replace(str, "</[\\s]*q[\\s]*[^>]*>?>", string.Empty);
            str = str.Replace("&rdquo;", "\"");
            str = str.Replace("&ldquo;", "\"");
            str = str.Replace("&lsquo;", "'");
            str = str.Replace("&rsquo;", "'");
            str = str.Replace("&nbsp;", " ");
            str = str.Replace("&hellip;", "…");
            str = str.Replace("&ndash;", "-");
            str = str.Replace("&mdash;", "—");
        }
        public GetNews()
        {
            //this.homeUrl = url;
            pageUrls = new List<string>(50);
        }
        private string getNextPageContent(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "get";
                req.ContentType = "text/html;charset=utf-8";

                StringBuilder sb = new StringBuilder(string.Empty);
                StringBuilder cont = new StringBuilder(string.Empty);
                using (HttpWebResponse wr = req.GetResponse() as HttpWebResponse)
                {

                    System.IO.Stream respStream = wr.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(respStream, System.Text.Encoding.GetEncoding(this.encoding));
                    Regex bodyr = new Regex(this.bodyRegex, RegexOptions.Singleline);
                    do
                    {
                        sb.Append(reader.ReadLine());
                    } while (!reader.EndOfStream);

                    string str = sb.ToString();

                    Match m = bodyr.Match(str);
                    if (m.Success)
                    {
                        string body = m.Groups["body"].Value;

                        deleteTag(ref body);
                        return body;
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            return string.Empty;



        }
        private void getContent(string url, int index, int total)
        {

            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

                req.Method = "get";
                req.ContentType = "	text/html;charset=utf-8";

                StringBuilder sb = new StringBuilder(string.Empty);
                StringBuilder cont = new StringBuilder(string.Empty);
                using (HttpWebResponse wr = req.GetResponse() as HttpWebResponse)
                {

                    System.IO.Stream respStream = wr.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(respStream, System.Text.Encoding.GetEncoding(this.encoding));
                    Regex titler = new Regex(this.titleRegex, RegexOptions.Singleline);
                    Regex timer = new Regex(this.timeRegex, RegexOptions.Singleline);
                    Regex bodyr = new Regex(this.bodyRegex, RegexOptions.Singleline);
                    do
                    {
                        sb.Append(reader.ReadLine());
                    }
                    while(!reader.EndOfStream);

                    string str = sb.ToString();
                    Match m = titler.Match(str);
                    if (m.Success)
                    {
                        cont.AppendLine(m.Groups["title"].Value);

                    }
                    cont.AppendLine(string.Format("({0}/{1}){2}", index, total, url));
                    m = timer.Match(str);
                    if (m.Success)
                    {
                        cont.AppendLine(m.Groups["time"].Value);
                    }
                    m = bodyr.Match(str);
                    if (m.Success)
                    {
                        string body = m.Groups["body"].Value;
                        deleteTag(ref body);
                        cont.AppendLine(body);
                    }
                    if (hasManyPage)
                    {

                        Regex mr = new Regex(this.manyPageRegex, RegexOptions.Singleline);
                        Match mm = mr.Match(str);
                        if (mm.Success)
                        {
                            string pagesurl = mm.Groups["np"].Value;
                            Regex r = new Regex(this.pageUrlsRegex, RegexOptions.Singleline);
                            MatchCollection mc = r.Matches(pagesurl);
                            for (int i = 0; i < mc.Count; i++)
                            {
                                string u = mc[i].Groups["url"].Value;
                                if (pageUrls.IndexOf(u) == -1)
                                {

                                    pageUrls.Add(u);
                                    cont.AppendLine(getNextPageContent(u));
                                }
                            }

                        }

                    }
                    cont.AppendLine("--------------------------------------------------------------");
                    WriteFile(cont.ToString());

                }
            }
            catch (Exception ex)
            {
                return;
            }


        }
        private void startGetAll()
        {

            for (int i = 0; i < pageUrls.Count; i++)
            {
                string u;
                if (hasPrefix)
                {
                    if (pageUrls[i].StartsWith("/"))
                        u = string.Format("{0}{1}", prefix, pageUrls[i].Substring(1));
                    else u = string.Format("{0}{1}", prefix, pageUrls[i]);

                }
                else u = pageUrls[i];


                getContent(u, i, pageUrls.Count);

            }
        }
        private void getPageUrls(int pageIndex)
        {
            string url;
            if (pageIndex == -1) url = prefix;
            else url = string.Format("{0}{1}", this.pageUrl, pageIndex);
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "get";
                req.ContentType = "	text/html;charset=utf-8";
                StringBuilder sb = new StringBuilder(string.Empty);
                using (HttpWebResponse wr = req.GetResponse() as HttpWebResponse)
                {

                    System.IO.Stream respStream = wr.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(respStream, System.Text.Encoding.GetEncoding(this.encoding));
                    Regex r = new Regex(this.pageUrlsRegex, RegexOptions.Singleline);

                    do
                    {
                        sb.Append(reader.ReadLine());
                    } while (!reader.EndOfStream);

                    MatchCollection m = r.Matches(sb.ToString());
                    for (int i = 0; i < m.Count; i++)
                    {
                        string temp = m[i].Groups["url"].Value;
                        if (pageUrls.IndexOf(temp) == -1) pageUrls.Add(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }

}

