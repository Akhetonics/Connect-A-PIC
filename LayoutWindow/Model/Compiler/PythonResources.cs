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
            return @"
import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)

";
        }
        public static string CreateFooter()
        {
            return @"    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign(""Akhetonics_ConnectAPIC""), filename=""Test.gds"")
";
        }
    }
}
