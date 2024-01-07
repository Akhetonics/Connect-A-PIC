import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io0'])
        cell_1_2 = CAPICPDK.placeCell_DirectionalCoupler(0.5).put('west0', cell_0_2.pin['east'])
        cell_0_3 = CAPICPDK.placeCell_Termination().put('west', cell_1_2.pin['west1'])
        cell_3_2 = CAPICPDK.placeCell_Delay(90).put('west', cell_1_2.pin['east0'])
        cell_3_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_1_2.pin['east1'])
        cell_4_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_3_3.pin['east'])
        cell_5_2 = CAPICPDK.placeCell_DirectionalCoupler(0.5).put('west0', cell_3_2.pin['east'])
        cell_7_2 = CAPICPDK.placeCell_Termination().put('west', cell_5_2.pin['east0'])
        cell_7_3 = CAPICPDK.placeCell_BendWG().put('west', cell_5_2.pin['east1'])
        cell_7_4 = CAPICPDK.placeCell_BendWG().put('west', cell_7_3.pin['south'])
        cell_6_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_7_4.pin['south'])
        cell_5_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_6_4.pin['east'])
        cell_4_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_5_4.pin['east'])
        cell_3_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_4_4.pin['east'])
        cell_2_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_3_4.pin['east'])
        cell_1_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_2_4.pin['east'])
        cell_0_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_1_4.pin['east'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")



