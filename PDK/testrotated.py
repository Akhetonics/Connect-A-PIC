
import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)

        cell_2_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((2+0)*CAPICPDK._CellSize,(-2+0)*CAPICPDK._CellSize,0)
        cell_5_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((5+0.5)*CAPICPDK._CellSize,(-2+-1.5)*CAPICPDK._CellSize,90)
        cell_8_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((8+2)*CAPICPDK._CellSize,(-2+-1)*CAPICPDK._CellSize,180)
        cell_11_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((11+0)*CAPICPDK._CellSize,(-2+0)*CAPICPDK._CellSize,0)
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="Test.gds")
