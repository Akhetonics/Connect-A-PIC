import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_BendWG().put('west', grating.pin['io0'])
        cell_0_3 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io1'])
        cell_1_3 = CAPICPDK.placeCell_BendWG().put('west', cell_0_3.pin['east'])
        cell_1_4 = CAPICPDK.placeCell_BendWG().put('south', cell_1_3.pin['south'])
        cell_2_4 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', cell_1_4.pin['west'])
        cell_4_1 = CAPICPDK.placeCell_BendWG().put((4+0)*CAPICPDK._CellSize,(-1+0)*CAPICPDK._CellSize,0)
        cell_5_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((5+0)*CAPICPDK._CellSize,(-3+0)*CAPICPDK._CellSize,0)
        cell_7_1 = CAPICPDK.placeCell_BendWG().put((7+0)*CAPICPDK._CellSize,(-1+0)*CAPICPDK._CellSize,0)
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
