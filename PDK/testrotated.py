
import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)

        cell_0_2 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io0'])
        cell_2_2 = CAPICPDK.placeCell_StraightWG().put((2+0.5)*CAPICPDK._CellSize,(-2+-0.5)*CAPICPDK._CellSize,90)
        cell_4_2 = CAPICPDK.placeCell_StraightWG().put((4+1)*CAPICPDK._CellSize,(-2+0)*CAPICPDK._CellSize,180)
        cell_6_2 = CAPICPDK.placeCell_StraightWG().put((6+0.5)*CAPICPDK._CellSize,(-2+0.5)*CAPICPDK._CellSize,270)
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="Test.gds")
