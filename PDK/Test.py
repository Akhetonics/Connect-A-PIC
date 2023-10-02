import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', grating.pin['io0'])
        cell_2_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_2.pin['east0'])
        cell_3_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_2_2.pin['east'])
        cell_4_2 = CAPICPDK.placeCell_StraightWG().put((4+0)*CAPICPDK._CellSize,(-2+0)*CAPICPDK._CellSize,0)
        cell_5_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_4_2.pin['east'])
        cell_6_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_5_2.pin['east'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
