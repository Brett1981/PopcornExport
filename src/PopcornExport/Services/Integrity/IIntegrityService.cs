using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Services.Integrity
{
    public interface IIntegrityService
    {
        Task Consolidate();
    }
}
