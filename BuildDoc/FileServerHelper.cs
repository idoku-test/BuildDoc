using Common;
using FileServerInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc
{
    class FileServerHelper
    {
        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns>文件流</returns>
        public static Stream GetFileStream(decimal fileID)
        {
            if (fileID <= 0)
            {
                return null;
            }

            IFileProcess service = GetFileProcess();
            byte[] bites = service.GetFile((int)fileID);
            if (bites != null)
            {
                return new MemoryTributary(bites);
            }
            else
            {
                return new MemoryTributary();
            }
        }

        /// <summary>
        /// 获取远程文件操作接口
        /// </summary>
        /// <returns>远程文件操作接口</returns>
        public static IFileProcess GetFileProcess()
        {
            IFileProcess fileCline = CreateServiceClient<IFileProcess>();
            return fileCline;
        }

        /// <summary>
        /// 创建远程服务接口
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>远程服务接口</returns>
        private static T CreateServiceClient<T>()
        {
            Type type = typeof(T);
            var factory = new System.ServiceModel.ChannelFactory<T>(type.FullName);
            return factory.CreateChannel();
        }
    }
}
