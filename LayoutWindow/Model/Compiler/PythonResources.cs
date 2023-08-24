using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConnectAPIC.LayoutWindow.Model.Compiler
{
    public class PythonResources
    {
        public static string CreateHeader(string PDKName, string StandardInputCellName)
        {
            return Resources.NazcaHeader
                .Replace("CAPICPDK", PDKName)
                .Replace("grating" , StandardInputCellName);
        }

        public static string CreateFooter(string layoutName= null, string gdsFileName = null)
        {
            layoutName ??= Resources.NazcaLayoutName;
            gdsFileName ??= Resources.PDKFileName;
            return Resources.NazcaFooter
                .Replace("Akhetonics_ConnectAPIC", layoutName)
                .Replace("ReplaceThisFileName.gds", gdsFileName);
        }

    }
}
