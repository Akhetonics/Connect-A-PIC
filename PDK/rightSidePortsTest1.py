import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:

        grating1 = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        grating2 = CAPICPDK.placeGratingArray_West(8).put(CAPICPDK._CellSize * 24, 0)
        cell_23_4 = CAPICPDK.placeCell_StraightWG().put('east', grating2.pin['wio2'])
        cell_0_7 = CAPICPDK.placeCell_StraightWG().put((0+0)*CAPICPDK._CellSize,(-7+0)*CAPICPDK._CellSize,0)
        cell_0_8 = CAPICPDK.placeCell_StraightWG().put((0+0)*CAPICPDK._CellSize,(-8+0)*CAPICPDK._CellSize,0)
        cell_0_9 = CAPICPDK.placeCell_StraightWG().put((0+0)*CAPICPDK._CellSize,(-9+0)*CAPICPDK._CellSize,0)
        cell_23_5 = CAPICPDK.placeCell_StraightWG().put((23+0)*CAPICPDK._CellSize,(-5+0)*CAPICPDK._CellSize,0)
        cell_23_6 = CAPICPDK.placeCell_StraightWG().put((23+0)*CAPICPDK._CellSize,(-6+0)*CAPICPDK._CellSize,0)
        cell_23_7 = CAPICPDK.placeCell_StraightWG().put((23+0)*CAPICPDK._CellSize,(-7+0)*CAPICPDK._CellSize,0)
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
