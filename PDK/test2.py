import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_1 = CAPICPDK.placeCell_Delay(deltaLength = 0.3).put('west', grating.pin['io0'])
        cell_2_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 0.5).put('west0', cell_0_1.pin['east'])
        cell_0_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 0.5).put('east0', cell_2_2.pin['west1'])
        cell_2_4 = CAPICPDK.placeCell_BendWG().put('west', cell_0_3.pin['east1'])
        cell_2_5 = CAPICPDK.placeCell_StraightWG().put('east', cell_2_4.pin['south'])
        cell_2_6 = CAPICPDK.placeCell_Termination().put('west', cell_2_5.pin['west'])
        cell_4_1 = CAPICPDK.placeCell_Delay(deltaLength = 0.84).put('west', cell_2_2.pin['east0'])
        cell_6_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 0.5).put('west0', cell_4_1.pin['east'])
        cell_4_3 = CAPICPDK.placeCell_Delay(deltaLength = 1.82).put('west', cell_6_2.pin['west1'])
        cell_8_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_6_2.pin['east0'])
        cell_8_3 = CAPICPDK.placeCell_Crossing().put('west', cell_6_2.pin['east1'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
