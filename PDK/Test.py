import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', grating.pin['io0'])
        cell_2_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_0_2.pin['east0'])
        cell_2_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_2.pin['east1'])
        cell_3_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west1', cell_2_3.pin['east'])
        cell_5_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_3_2.pin['east0'])
        cell_6_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_5_2.pin['east'])
        cell_5_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', cell_3_2.pin['east1'])
        cell_0_4 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io2'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
