
import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)

        cell_0_1 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io1'])
        cell_1_1 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_1.pin['east'])
        cell_2_1 = CAPICPDK.placeCell_StraightWG().put('west', cell_1_1.pin['east'])
        cell_3_1 = CAPICPDK.placeCell_StraightWG().put('west', cell_2_1.pin['east'])
        cell_4_1 = CAPICPDK.placeCell_StraightWG().put('west', cell_3_1.pin['east'])
        cell_5_1 = CAPICPDK.placeCell_StraightWG().put('west', cell_4_1.pin['east'])
        cell_0_3 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io2'])
        cell_1_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_3.pin['east'])
        cell_2_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_1_3.pin['east'])
        cell_3_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_2_3.pin['east'])
        cell_4_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_3_3.pin['east'])
        cell_5_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_4_3.pin['east'])
        cell_6_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_5_3.pin['east'])
        cell_7_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_6_3.pin['east'])
        cell_0_5 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', grating.pin['io3'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="Test.gds")
