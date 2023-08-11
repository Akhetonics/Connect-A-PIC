using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.Model.Compiler
{
    public class PythonResources
    {
        public static string CreateHeader(string PDKName, string StandardInputCellName)
        {
            return @"
using nazca as nd;
from TestPDK import TestPDK;

" + PDKName + @" = TestPDK();

def FullDesign(layoutName)
{
    using (var fullLayoutInner = nd.Cell(name: layoutName))
    {
        var " + StandardInputCellName + @" = CAPICPDK.placeGrating_East(8).put(0, 0);
    }
}
";
        }
        public static string CreateFooter()
        {
            return @"return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign(""Akhetonics_ConnectAPIC""), filename=""Test.gds"")";
        }
    }
}
