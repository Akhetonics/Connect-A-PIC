
import nazca as nd
from TestPDK import TestPDK

class ExtendedCell(nd.Cell):
    def __init__(self, *args, **kwargs):
        # Setzen von autobbox auf True vor dem Aufruf des ursprünglichen Konstruktors
        kwargs["autobbox"] = True
        super().__init__(*args, **kwargs)
    def rotate(self, angle):
        # Voraussetzung ist, dass die Bounding-Box bekannt ist
        self.autobbox = True
        bbox = self.bbox

        # Breite und Höhe der Bounding-Box ermitteln
        width = bbox[1] - bbox[0]
        height = bbox[3] - bbox[2]

        # Abhängig vom Drehwinkel die Zelle verschieben
        if angle == 90:
            self.move(0, -width)
        elif angle == 180:
            self.move(-width, -height)
        elif angle == 270:
            self.move(-height, 0)
        else:
            raise ValueError("Unsupported rotation angle. Only 90, 180, or 270 degrees are supported.")

        # Durchführen der eigentlichen Rotation
        for instance in self.cell_instances:
            instance.transformation.rotate(angle)

CAPICPDK = TestPDK()

def FullDesign(layoutName):
    with nd.Cell(name=layoutName) as fullLayoutInner:       
        cell = ExtendedCell('my_cell')
        nd.Polygon(points=[(0, 0), (10, 0), (10, 10), (0, 10)]).put()
        cell.rotate(90)

        # Demonstration in einer Hauptzelle
        main_cell = nd.Cell('main_cell')
        main_cell.put(cell)
        
        grating = CAPICPDK.placeGratingArray_East(8).put(0, 0)
        cell_0_2 = CAPICPDK.placeCell_StraightWG().put( 0 ,-CAPICPDK._CellSize*2 - CAPICPDK._GratingOffset)
        
        #cell_0_2 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io1'])
        cell_1_2 = CAPICPDK.placeCell_StraightWG().put(CAPICPDK._CellSize ,-CAPICPDK._CellSize*2 - CAPICPDK._GratingOffset)
        #cell_1_2 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_2.pin['east'])
        cell_2_2 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_1_2.pin['east'])
        cell_0_3 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io2'])
        cell_1_3 = CAPICPDK.placeCell_StraightWG().put('west', cell_0_3.pin['east'])
        cell_2_3 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_1_3.pin['east'])
        cell_0_4 = CAPICPDK.placeCell_StraightWG().put('west', grating.pin['io3'])
        cell_1_4 = CAPICPDK.placeCell_GratingCoupler().put('west', cell_0_4.pin['east'])
    return fullLayoutInner

nd.print_warning = False
nd.export_gds(topcells=FullDesign("Akhetonics_ConnectAPIC"), filename="Test.gds")
