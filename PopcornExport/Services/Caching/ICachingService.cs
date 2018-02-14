using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Services.Caching
{
    /// <summary>
    /// Caching service
    /// </summary>
    public interface ICachingService
    {
        /// <summary>
        /// Flush cache
        /// </summary>
        Task Flush();
    }
}
