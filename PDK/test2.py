import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io0'])
        cell_1_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 0.5).put('west0', cell_0_2.pin['east'])
        cell_0_3 = CAPICPDK.placeCell_StraightWG().put('east', cell_1_2.pin['west1'])
        cell_3_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 0.47000000000000003).put('east1', cell_1_2.pin['east0'])
        cell_5_1 = CAPICPDK.placeCell_Delay(deltaLength = 0.5).put('west', cell_3_2.pin['west1'])
        cell_7_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 0.5).put('east1', cell_5_1.pin['east'])
        cell_12_2 = CAPICPDK.placeCell_Crossing().put((12+0)*CAPICPDK._CellSize,(-2+0)*CAPICPDK._CellSize,0)
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
