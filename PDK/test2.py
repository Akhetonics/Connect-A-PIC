import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io0'])
        cell_1_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_2.pin['east'])
        cell_2_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 0.5).put('east1', cell_1_2.pin['east'])
        cell_0_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 0.8200000000000001).put('east0', cell_2_2.pin['east0'])
        cell_4_2 = CAPICPDK.placeCell_Termination().put('west', cell_2_2.pin['west1'])
        cell_4_3 = CAPICPDK.placeCell_BendWG().put('west', cell_2_2.pin['west0'])
        cell_4_4 = CAPICPDK.placeCell_BendWG().put('south', cell_4_3.pin['south'])
        cell_5_4 = CAPICPDK.placeCell_BendWG().put('west', cell_4_4.pin['west'])
        cell_5_5 = CAPICPDK.placeCell_BendWG().put('west', cell_5_4.pin['south'])
        cell_3_5 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 0.13).put('east0', cell_5_5.pin['south'])
        cell_2_5 = CAPICPDK.placeCell_StraightWG().put('east', cell_3_5.pin['west0'])
        cell_1_5 = CAPICPDK.placeCell_StraightWG().put('east', cell_2_5.pin['west'])
        cell_0_5 = CAPICPDK.placeCell_StraightWG().put('east', cell_1_5.pin['west'])
        cell_2_6 = CAPICPDK.placeCell_BendWG().put('south', cell_3_5.pin['west1'])
        cell_2_7 = CAPICPDK.placeCell_StraightWG().put('east', cell_2_6.pin['west'])
        cell_2_8 = CAPICPDK.placeCell_BendWG().put('south', cell_2_7.pin['west'])
        cell_3_8 = CAPICPDK.placeCell_StraightWG().put('west', cell_2_8.pin['west'])
        cell_4_8 = CAPICPDK.placeCell_StraightWG().put('west', cell_3_8.pin['east'])
        cell_5_8 = CAPICPDK.placeCell_StraightWG().put('west', cell_4_8.pin['east'])
        cell_6_8 = CAPICPDK.placeCell_StraightWG().put('west', cell_5_8.pin['east'])
        cell_7_8 = CAPICPDK.placeCell_BendWG().put('south', cell_6_8.pin['east'])
        cell_7_7 = CAPICPDK.placeCell_StraightWG().put('west', cell_7_8.pin['west'])
        cell_7_6 = CAPICPDK.placeCell_BendWG().put('south', cell_7_7.pin['east'])
        cell_5_6 = CAPICPDK.placeCell_Delay(deltaLength = 0.75).put('west', cell_7_6.pin['west'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
