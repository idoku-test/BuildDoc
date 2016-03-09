using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Cache
{
    /// <summary>
    /// Cache Client Interface
    /// </summary>
    public interface ICacheClient
    {
        bool KeyExists(string key);

        /// <summary>
        /// Set an object to cache server
        /// </summary>
        bool Set(string key, object value);

        /// <summary>
        /// Get an object from cache server
        /// </summary>
        object Get(string key);

        /// <summary>
        /// Delete an object from cache server
        /// </summary>
        bool Delete(string key);

        /// <summary>
        /// Clear all cache items from cache server
        /// </summary>
        bool FlushAll();

        /// <summary>
        /// Init Cache Client Pool
        /// </summary>
        void Init();

        /// <summary>
        /// Shutdown Cache Client
        /// </summary>
        void Shutdown();
    }
}
