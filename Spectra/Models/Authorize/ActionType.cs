using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models.Authorize
{
    [Flags]
    public enum ActionType
    {
        [Description("View")]
        Xem = 1,
        [Description("Add")]
        Them = 2,
        [Description("Edit")]
        Sua = 4,
        [Description("Delete")]
        Xoa = 8,
        [Description("Export")]
        XuatFile = 16,

    }
}
