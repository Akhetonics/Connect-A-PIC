import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io0'])
        cell_1_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_2.pin['east'])
        cell_2_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_1_2.pin['east'])
        cell_0_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', grating.pin['io1'])
        cell_2_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_3.pin['east0'])
        cell_0_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west1', grating.pin['io2'])
        cell_2_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_3.pin['east0'])
        cell_4_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((4+0)*CAPICPDK._CellSize,(-2+0)*CAPICPDK._CellSize,0)
        cell_6_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_4_2.pin['east0'])
        cell_6_3 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_4_2.pin['east1'])
        cell_9_0 = CAPICPDK.placeCell_StraightWG().put((9+0)*CAPICPDK._CellSize,(0+0)*CAPICPDK._CellSize,0)
        cell_10_0 = CAPICPDK.placeCell_StraightWG().put('west', cell_9_0.pin['east'])
        cell_9_1 = CAPICPDK.placeCell_StraightWG().put((9+0)*CAPICPDK._CellSize,(-1+0)*CAPICPDK._CellSize,0)
        cell_9_2 = CAPICPDK.placeCell_StraightWG().put((9+0)*CAPICPDK._CellSize,(-2+0)*CAPICPDK._CellSize,0)

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
