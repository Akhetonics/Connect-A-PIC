import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_12_4 = CAPICPDK.placeCell_StraightWG().put((12+0)*CAPICPDK._CellSize,(-4+0)*CAPICPDK._CellSize,0)
        cell_13_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_12_4.pin['east'])
        cell_14_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_13_4.pin['east'])
        cell_15_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_14_4.pin['east'])
        cell_16_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_15_4.pin['east'])
        cell_17_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_16_4.pin['east'])
        cell_18_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_17_4.pin['east'])
        cell_19_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_18_4.pin['east'])
        cell_20_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_19_4.pin['east'])
        cell_21_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_20_4.pin['east'])
        cell_22_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_21_4.pin['east'])
        cell_23_4 = CAPICPDK.placeCell_StraightWG().put('west', cell_22_4.pin['east'])
        cell_23_2 = CAPICPDK.placeCell_BendWG().put((23+0.5)*CAPICPDK._CellSize,(-2+-0.5)*CAPICPDK._CellSize,90)
        cell_23_3 = CAPICPDK.placeCell_BendWG().put('south', cell_23_2.pin['west'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
