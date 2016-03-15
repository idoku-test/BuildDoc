using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Common.SharpZip
{
    public class SharpZipHelper
    {

        public class FileBytes
        {
            public byte[] FileByte;
            public string FileName;
        }

        public class ZipFilesOut
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }

            public byte[] FileByte;

        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="filePath">要下载的文件的完整路径</param>
        /// <param name="downloadName">下载时要显示的文件名</param>
        /// <param name="isDelete">下载后是否删除文件</param>
        /// <param name="bts">文件的byte</param>
        public void FileDonwLoad(HttpRequestBase Request, HttpResponseBase Response, string downloadName, bool isDelete = false, byte[] bts = null, string filePath = null)
        {
            //以字符流的形式下载文件
            byte[] bytes;
            if (bts == null)
            {
                FileStream fs = new FileStream(filePath, FileMode.Open);
                bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                //读取完成后删除
                if (isDelete)
                {
                    File.Delete(filePath);
                }
            }
            else
            {
                bytes = bts;
            }
            //下载文件
            Response.HeaderEncoding = System.Text.Encoding.GetEncoding("utf-8");
            if (!string.IsNullOrEmpty(filePath) && filePath.Contains(".xls"))
            {
                Response.ContentType = "application/ms-excel";
            }
            else
            {

                Response.ContentType = "application/octet-stream";
            }

            if (Request.UserAgent.ToLower().IndexOf("msie") > -1)
            {
                downloadName = HttpUtility.UrlPathEncode(downloadName);
            }
            if (Request.UserAgent.ToLower().IndexOf("firefox") > -1)
            {
                Response.AddHeader("Content-Disposition", "attachment;filename=\"" + downloadName + "\"");
            }
            else
            {
                Response.AddHeader("Content-Disposition", "attachment;filename=" + downloadName);
            }
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            Response.BinaryWrite(bytes);
            Response.Flush();
            //context.Response.End();
            //清空以免缓存影响下次下载
            Response.Clear();
        }

        #region 压缩文件夹

        /// <summary>    
        /// 压缩文件夹    
        /// </summary>    
        /// <param name="dirPath">要打包的文件夹</param>    
        /// <param name="GzipFileName">目标文件名</param>    
        /// <param name="CompressionLevel">压缩品质级别（0~9）</param>    
        /// <param name="deleteDir">是否删除原文件夹</param>  
        public void CompressDirectory(string dirPath, string GzipFileName, int CompressionLevel, bool deleteDir)
        {
            //压缩文件为空时默认与压缩文件夹同一级目录    
            if (GzipFileName == string.Empty)
            {
                GzipFileName = dirPath.Substring(dirPath.LastIndexOf("//") + 1);
                GzipFileName = dirPath.Substring(0, dirPath.LastIndexOf("//")) + "//" + GzipFileName + ".zip";
            }
            if (Path.GetExtension(GzipFileName) != ".zip")
            {
                GzipFileName = GzipFileName + ".zip";
            }

            using (ZipOutputStream zipoutputstream = new ZipOutputStream(File.Create(GzipFileName)))
            {
                zipoutputstream.SetLevel(CompressionLevel);
                Crc32 crc = new Crc32();
                Dictionary<string, DateTime> fileList = GetAllFies(dirPath);
                foreach (KeyValuePair<string, DateTime> item in fileList)
                {
                    FileStream fs = File.OpenRead(item.Key.ToString());
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(item.Key.Substring(dirPath.Length));
                    entry.IsUnicodeText = true;//避免中文乱码
                    entry.DateTime = item.Value;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipoutputstream.PutNextEntry(entry);
                    zipoutputstream.Write(buffer, 0, buffer.Length);
                }
                zipoutputstream.Dispose();
            }
            if (deleteDir)
            {
                Directory.Delete(dirPath, true);
            }
        }
        /// <summary>    
        /// 获取所有文件    
        /// </summary>    
        /// <returns></returns>    
        private static Dictionary<string, DateTime> GetAllFies(string dir)
        {
            Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
            DirectoryInfo fileDire = new DirectoryInfo(dir);
            if (!fileDire.Exists)
            {
                throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
            }
            GetAllDirFiles(fileDire, FilesList);
            GetAllDirsFiles(fileDire.GetDirectories(), FilesList);
            return FilesList;
        }
        /// <summary>    
        /// 获取一个文件夹下的文件    
        /// </summary>    
        /// <param name="dir">目录名称</param>    
        /// <param name="filesList">文件列表HastTable</param>    
        private static void GetAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
        {
            foreach (FileInfo file in dir.GetFiles("*.*"))
            {
                filesList.Add(file.FullName, file.LastWriteTime);
            }
        }
        /// <summary>    
        /// 获取一个文件夹下的所有文件夹里的文件    
        /// </summary>    
        /// <param name="dirs"></param>    
        /// <param name="filesList"></param>    
        private static void GetAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
        {
            foreach (DirectoryInfo dir in dirs)
            {
                foreach (FileInfo file in dir.GetFiles("*.*"))
                {
                    filesList.Add(file.FullName, file.LastWriteTime);
                }
                GetAllDirsFiles(dir.GetDirectories(), filesList);
            }
        }
        #endregion


        #region 制作压缩包（多个文件压缩到一个压缩包，支持加密、注释）

        /// <summary>
        /// 制作压缩包（多个文件压缩到一个压缩包，支持加密、注释） 
        /// </summary>
        /// <param name="lstFileBytes">文件流列表</param>
        /// <param name="zipFileName">压缩包文件名</param>
        /// <param name="compresssionLevel">压缩级别 1-9</param>
        /// <param name="password">密码</param>
        /// <param name="comment">注释</param>
        /// <returns></returns>
        public ZipFilesOut ZipFiles(List<FileBytes> lstFileBytes, string zipFileName, int compresssionLevel, string password, string comment)
        {
            ZipFilesOut zipFilesResult = new ZipFilesOut();
            MemoryStream memoryStream = new MemoryStream();

            #region 创建压缩文件
            try
            {
                using (ZipOutputStream zos = new ZipOutputStream(memoryStream))
                {
                    if (compresssionLevel != 0)
                    {
                        zos.SetLevel(compresssionLevel);//设置压缩级别  
                    }

                    if (!string.IsNullOrEmpty(password))
                    {
                        zos.Password = password;//设置zip包加密密码  
                    }

                    if (!string.IsNullOrEmpty(comment))
                    {
                        zos.SetComment(comment);//设置zip包的注释  
                    }
                    foreach (var item in lstFileBytes)
                    {
                        ZipEntry entry = new ZipEntry(item.FileName);
                        entry.IsUnicodeText = true;//避免中文乱码
                        zos.PutNextEntry(entry);
                        zos.Write(item.FileByte, 0, item.FileByte.Length);
                    }
                    zos.Dispose();

                    zipFilesResult.FileByte = memoryStream.ToArray();
                    zipFilesResult.IsSuccess = true;
                    zipFilesResult.Message = "生成压缩文件成功!";
                }

            }
            catch
            {
                memoryStream.Dispose();
                memoryStream.Close();
                zipFilesResult.IsSuccess = false;
                zipFilesResult.Message = "生成压缩文件失败!";
            }


            #endregion

            return zipFilesResult;
        }
        #endregion
    }
}
