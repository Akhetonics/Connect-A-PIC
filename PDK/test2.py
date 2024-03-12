import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_MMI3x3().put('west0', grating.pin['io0'])
        cell_3_0 = CAPICPDK.placeCell_MMI3x3().put('west2', cell_0_2.pin['east0'])
        cell_3_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_2.pin['east1'])
        cell_4_3 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_3_3.pin['east'])
        cell_3_4 = CAPICPDK.placeCell_BendWG().put('west', cell_0_2.pin['east2'])
        cell_3_5 = CAPICPDK.placeCell_BendWG().put('west', cell_3_4.pin['south'])
        cell_2_5 = CAPICPDK.placeCell_StraightWG().put('east', cell_3_5.pin['south'])
        cell_1_5 = CAPICPDK.placeCell_StraightWG().put('east', cell_2_5.pin['west'])
        cell_0_5 = CAPICPDK.placeCell_StraightWG().put('east', cell_1_5.pin['west'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
