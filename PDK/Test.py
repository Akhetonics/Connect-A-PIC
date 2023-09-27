import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', grating.pin['io0'])
        cell_2_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', cell_0_2.pin['east0'])
        cell_4_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', cell_2_2.pin['east0'])
        cell_10_1 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((10+0)*CAPICPDK._CellSize,(-1+0)*CAPICPDK._CellSize,0)
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
