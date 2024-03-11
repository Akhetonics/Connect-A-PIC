import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io0'])
        cell_1_2 = CAPICPDK.placeCell_MMI3x3().put('west0', cell_0_2.pin['east'])
        cell_4_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_1_2.pin['east0'])
        cell_5_2 = CAPICPDK.placeCell_MMI3x3().put('west0', cell_4_2.pin['east'])
        cell_4_3 = CAPICPDK.placeCell_StraightWG().put('east', cell_5_2.pin['west1'])
        cell_4_4 = CAPICPDK.placeCell_StraightWG().put('east', cell_5_2.pin['west2'])
        cell_8_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_5_2.pin['east0'])
        cell_9_2 = CAPICPDK.placeCell_MMI3x3().put('west0', cell_8_2.pin['east'])
        cell_8_3 = CAPICPDK.placeCell_StraightWG().put('east', cell_9_2.pin['west1'])
        cell_8_4 = CAPICPDK.placeCell_StraightWG().put('east', cell_9_2.pin['west2'])
        cell_12_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_9_2.pin['east0'])
        cell_13_2 = CAPICPDK.placeCell_MMI3x3().put('west0', cell_12_2.pin['east'])
        cell_12_3 = CAPICPDK.placeCell_StraightWG().put('east', cell_13_2.pin['west1'])
        cell_12_4 = CAPICPDK.placeCell_StraightWG().put('east', cell_13_2.pin['west2'])
        cell_16_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_13_2.pin['east0'])
        cell_17_2 = CAPICPDK.placeCell_MMI3x3().put('west0', cell_16_2.pin['east'])
        cell_16_3 = CAPICPDK.placeCell_StraightWG().put('east', cell_17_2.pin['west1'])
        cell_16_4 = CAPICPDK.placeCell_StraightWG().put('east', cell_17_2.pin['west2'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
