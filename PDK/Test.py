import nazca as nd
from TestPDK import TestPDK

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       

        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', grating.pin['io0'])
        cell_2_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', cell_0_2.pin['east1'])
        cell_4_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west1', cell_2_3.pin['east0'])
        cell_4_4 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('east1', cell_2_3.pin['east1'])
        cell_0_2 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west1', grating.pin['io1'])
        cell_2_3 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('west0', cell_0_2.pin['east1'])
        #cell_0_4 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put('', grating.pin['io2'])
        cell_3_0 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((3+0.5)*CAPICPDK._CellSize,(0+-1.5)*CAPICPDK._CellSize,90)
        cell_3_6 = CAPICPDK.placeCell_DirectionalCoupler(deltaLength = 50).put((3+0.5)*CAPICPDK._CellSize,(-6+-1.5)*CAPICPDK._CellSize,90)
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="TestPDK.py")
