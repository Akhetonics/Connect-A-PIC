
import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)

        cell_4_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((4+0.5)*CAPICPDK._CellSize,(-3+-1.5)*CAPICPDK._CellSize,90)
        cell_5_5 = CAPICPDK.placeCell_StraightWG().put('east', cell_4_3.pin['west1'])
        cell_5_6 = CAPICPDK.placeCell_StraightWG().put('east', cell_5_5.pin['west'])
        cell_5_7 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_5_6.pin['west'])
        cell_9_3 = CAPICPDK.placeCell_GratingCoupler().put((9+1)*CAPICPDK._CellSize,(-3+0)*CAPICPDK._CellSize,180)
        cell_9_5 = CAPICPDK.placeCell_GratingCoupler().put((9+0.5)*CAPICPDK._CellSize,(-5+0.5)*CAPICPDK._CellSize,270)
        cell_11_3 = CAPICPDK.placeCell_GratingCoupler().put((11+0.5)*CAPICPDK._CellSize,(-3+-0.5)*CAPICPDK._CellSize,90)
        cell_11_5 = CAPICPDK.placeCell_GratingCoupler().put((11+0)*CAPICPDK._CellSize,(-5+0)*CAPICPDK._CellSize,0)
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="Test.gds")
