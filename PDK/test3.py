
import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)

        cell_0_2 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io1'])
        cell_1_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_0_2.pin['east'])
        cell_0_3 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io2'])
        cell_1_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_3.pin['east'])
        cell_2_3 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_1_3.pin['east'])
        cell_0_4 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io3'])
        cell_1_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_4.pin['east'])
        cell_2_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_1_4.pin['east'])
        cell_3_4 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_2_4.pin['east'])
        cell_7_4 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((7)*CAPICPDK._CellSize , (-4)*CAPICPDK._CellSize )
        cell_8_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_7_4.pin['east0'])
        cell_9_4 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_8_4.pin['east'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="Test.gds")
