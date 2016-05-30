using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FileServerInterface.Model;
using System.IO;

namespace FileServerInterface
{
    /// <summary>
    /// 文件服务处理接口
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract()]
    public interface IFileProcess
    {

        /// <summary>
        ///获取文件.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        [OperationContract]       
        byte[] GetFile(int fileId);

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        [OperationContract]
        byte[] GetThumbnail(int fileId, int width, int height);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileData">The file data.</param>
        /// <returns></returns>
        [OperationContract()]
        int Upload(string token,string fileName,byte[] fileData);

        [OperationContract()]
        int StartUploadFile(string strFileName, int nFileSize, string strHashCode,out int nUploadedSize);
        /// <summary>
         /// 分块上传文件
         /// </summary>
        [OperationContract()]
        bool UploadFileChunk(int nFileID, byte[] data, int nFileOffset);
        [OperationContract()]
        byte[] GetFileChunk(int nFileID, int nFileOffset, int nChunkSize);

        /// <summary>
        /// 获取资源信息,不返回文件内容.
        /// </summary>
        /// <returns></returns>
        [OperationContract()]
        StorageFileInfo Stat(string fileName);


        /// <summary>
        /// 获取资源信息,不返回文件内容.
        /// </summary>
        /// <returns></returns>
        [OperationContract()]
        StorageFileInfo GetFileInfo(int nFileID);


        /// <summary>
        /// 删除资源
        /// </summary>
        /// <returns></returns>
        [OperationContract()]
        int Delete(string fileName);

        ///// <summary>
        ///// 返回文件列表
        ///// </summary>
        ///// <param name="pageIndex">开始页</param>
        ///// <param name="pageSize">页数</param>
        ///// <returns></returns>
        //[OperationContract()]
        //IList<StorageFileInfo> GetFileInfos(int pageIndex, int pageSize,out int pageTotal);

    }
}
