using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopcornExport.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Enum"/>
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieve friendly name
        /// </summary>
        /// <param name="code">Enum</param>
        /// <returns>Friendly name</returns>
        public static string ToFriendlyString(this Enum code)
        {
            return Enum.GetName(code.GetType(), code);
        }
    }
}
