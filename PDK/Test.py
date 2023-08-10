import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGrating_East(8).put(0, 0)

# -------------------

        cell_00 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io5'])
        cell_01 = CAPICPDK.placeCell_Crossing().put('west', cell_00.pin['east'])
        cell_02 = CAPICPDK.placeCell_BendWG().put('west', cell_01.pin['east'])
        cell_03 = CAPICPDK.placeCell_StraightWG().put('west', cell_02.pin['south'])
        cell_04 = CAPICPDK.placeCell_BendWG().put('south', cell_03.pin['east'])
        cell_05 = CAPICPDK.placeCell_StraightWG().put('west', cell_04.pin['west'])
        cell_06 = CAPICPDK.placeCell_BendWG().put('west', cell_05.pin['east'])
        cell_07 = CAPICPDK.placeCell_BendWG().put('west', cell_06.pin['south'])
        cell_08 = CAPICPDK.placeCell_StraightWG().put('west', cell_07.pin['south'])
        cell_09 = CAPICPDK.placeCell_StraightWG().put('west', cell_08.pin['east'])
        cell_10 = CAPICPDK.placeCell_StraightWG().put('west', cell_09.pin['east'])
        cell_11 = CAPICPDK.placeCell_StraightWG().put('west', cell_10.pin['east'])
        
        cell_12 = CAPICPDK.placeCell_AWG().put('west', cell_01.pin['north'])
        cell_13 = CAPICPDK.placeCell_Termination().put('west', cell_12.pin['east0'])
        cell_14 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', cell_12.pin['east1'])
        cell_15 = CAPICPDK.placeCell_Ring(deltaLength = 50).put('west0', cell_14.pin['east1'])
        
        cell_16 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_01.pin['south'])
        
        cell_17 = CAPICPDK.placeCell_BendWG().put('south', cell_14.pin['east0'])
        cell_18 = CAPICPDK.placeCell_StraightWG().put('west', cell_17.pin['west'])
# -------------------

    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="Test.gds")
