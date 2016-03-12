using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Cache
{
    /// <summary>
    /// Local Cache Client
    /// </summary>
    internal class LocalCacheClient : ICacheClient
    {
        private static Dictionary<string, object> _cache = null;

        private static Dictionary<string, object> Cache
        {
            get
            {
                if (_cache == null)
                    _cache = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);

                return _cache;
            }
        }

        public bool KeyExists(string key)
        {
            return Cache.ContainsKey(key);
        }

        /// <summary>
        /// Set
        /// </summary>
        public bool Set(string key, object value)
        {
            Cache[key] = value;
            return true;
        }

        /// <summary>
        /// Get
        /// </summary>
        public object Get(string key)
        {
            return Cache[key];
        }

        /// <summary>
        /// Delete
        /// </summary>
        public bool Delete(string key)
        {
            return Cache.Remove(key);
        }

        /// <summary>
        /// Clear
        /// </summary>
        /// <returns></returns>
        public bool FlushAll()
        {
            Cache.Clear();
            return true;
        }

        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            _cache = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
        }
        
        /// <summary>
        /// Shutdown
        /// </summary>
        public void Shutdown()
        {
            if (_cache != null)
                _cache.Clear();
        }
    }
}
