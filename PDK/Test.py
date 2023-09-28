import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io0'])
        cell_1_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_0_2.pin['east'])
        cell_0_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', grating.pin['io1'])
        cell_2_2 = CAPICPDK.placeCell_StraightWG().put((2+0)*CAPICPDK._CellSize,(-2+0)*CAPICPDK._CellSize,0)
        cell_3_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_2_2.pin['east'])
        cell_4_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_3_2.pin['east'])
        cell_5_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_4_2.pin['east'])
        cell_3_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((3+0.5)*CAPICPDK._CellSize,(-3+-1.5)*CAPICPDK._CellSize,90)
        cell_4_6 = CAPICPDK.placeCell_GratingCoupler().put((4+1)*CAPICPDK._CellSize,(-6+0)*CAPICPDK._CellSize,180)
        cell_5_6 = CAPICPDK.placeCell_StraightWG().put('west', cell_4_6.pin['west'])
        cell_6_6 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_5_6.pin['east'])
        cell_5_5 = CAPICPDK.placeCell_GratingCoupler().put((5+0.5)*CAPICPDK._CellSize,(-5+-0.5)*CAPICPDK._CellSize,90)
        cell_5_7 = CAPICPDK.placeCell_GratingCoupler().put((5+0.5)*CAPICPDK._CellSize,(-7+0.5)*CAPICPDK._CellSize,270)
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
