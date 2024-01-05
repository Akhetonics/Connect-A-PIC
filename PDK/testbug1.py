import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_BendWG().put('west', grating.pin['io0'])
        cell_0_3 = CAPICPDK.placeCell_BendWG().put('south', cell_0_2.pin['south'])
        cell_1_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', cell_0_3.pin['west'])
        cell_0_4 = CAPICPDK.placeCell_StraightWG().put('east', cell_1_3.pin['west1'])
        cell_3_3 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_1_3.pin['east0'])
        cell_3_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_1_3.pin['east1'])
        cell_4_4 = CAPICPDK.placeCell_BendWG().put('west', cell_3_4.pin['east'])
        cell_4_5 = CAPICPDK.placeCell_StraightWG().put('east', cell_4_4.pin['south'])
        cell_4_6 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('east0', cell_4_5.pin['west'])
        cell_5_5 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_4_6.pin['east1'])
        cell_5_8 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_4_6.pin['west1'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
