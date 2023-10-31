import nazca as nd

class TestPDK(object):

    # Singleton
    def __new__(cls):
        if not hasattr(cls, 'instance'):
            cls.instance = super(TestPDK, cls).__new__(cls)
        return cls.instance
    
    def __init__(self):
        # General 
        self._BendRadius = 80  
        self._WGWidth = 1.2    
        self._CellSize = 250            
        self._GratingOffset = self._CellSize        
        self._SiNLayer = 1
        self._BoxLayer = 1001
        self._TextLayer = 1002

    def placeCell_StraightWG(self):
        with nd.Cell(name='AkhetCell_StraightWG', autobbox=True) as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 0.5), (self._CellSize, -self._CellSize * 0.5), (self._CellSize, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
            nd.text(text='Straight WG', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 0.4)

            nd.strt(length=self._CellSize, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)

            nd.Pin('east', width=self._WGWidth).put(self._CellSize, 0, 0)
            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
            nd.Pin('center', width=self._WGWidth).put(self._CellSize/2,self._CellSize/2)
        return akhetCell

    def placeCell_BendWG(self):
        with nd.Cell(name='AkhetCell_BendWG') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 0.5), (self._CellSize, -self._CellSize * 0.5), (self._CellSize, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
            nd.text(text='Bend WG', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 0.4)

            # TODO: Not the correct length, has to be coordinated with Straight
            nd.bend(angle=-90, radius=self._CellSize * 0.5, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)

            nd.Pin('south', width=self._WGWidth).put(self._CellSize * 0.5, -self._CellSize * 0.5, 270)
            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
            nd.Pin('center', width=self._WGWidth).put(self._CellSize/2,self._CellSize/2)
        return akhetCell
    
        
    def placeCell_Termination(self):
        with nd.Cell(name='AkhetCell_Termination') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 0.5), (self._CellSize, -self._CellSize * 0.5), (self._CellSize, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
            nd.text(text='Termination', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 0.4)

            nd.Polygon(layer=self._SiNLayer, points=[(0,-self._WGWidth * 0.5), (0, self._WGWidth * 0.5), (self._CellSize * 0.7, 0)]).put(0, 0)

            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
            nd.Pin('center', width=self._WGWidth).put(self._CellSize/2,self._CellSize/2)
        return akhetCell
        
    def placeCell_GratingCoupler(self):
        with nd.Cell(name='AkhetCell_GratingCoupler') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 0.5), (self._CellSize, -self._CellSize * 0.5), (self._CellSize, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
            nd.text(text='Grating Coupler', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 0.4)

            # TODO: Not a real grating coupler
            nd.Polygon(layer=self._SiNLayer, points=[(0,-self._WGWidth * 0.5), (0, self._WGWidth * 0.5), (self._CellSize * 0.7, 50), (self._CellSize * 0.7, -50)]).put(0, 0)

            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
            nd.Pin('center', width=self._WGWidth).put(self._CellSize/2,self._CellSize/2)
        return akhetCell

    def placeCell_Crossing(self):
        with nd.Cell(name='AkhetCell_Crossing') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 0.5), (self._CellSize, -self._CellSize * 0.5), (self._CellSize, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
            nd.text(text='Crossing', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 0.4)

            # TODO: Not a real crossing
            nd.strt(length=self._CellSize, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)
            nd.strt(length=self._CellSize, width=self._WGWidth, layer=self._SiNLayer).put(self._CellSize * 0.5, -self._CellSize * 0.5, 90)

            nd.Pin('south', width=self._WGWidth).put(self._CellSize * 0.5, -self._CellSize * 0.5, 270)
            nd.Pin('east', width=self._WGWidth).put(self._CellSize, 0, 0)
            nd.Pin('north', width=self._WGWidth).put(self._CellSize * 0.5, self._CellSize * 0.5, 90)
            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
            nd.Pin('center', width=self._WGWidth).put(self._CellSize/2,self._CellSize/2)
        return akhetCell

    def placeCell_DirectionalCoupler(self, deltaLength):
        with nd.Cell(name='AkhetCell_DirectionalCoupler') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 1.5), (self._CellSize * 2, -self._CellSize * 1.5), (self._CellSize * 2, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
            nd.text(text='Directional Coupler', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 0.4)

            # TODO: Wrong Coupler
            nd.strt(length=self._CellSize * 2, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)
            nd.strt(length=self._CellSize * 2, width=self._WGWidth, layer=self._SiNLayer).put(0, -self._CellSize)

            nd.Pin('west0', width=self._WGWidth).put(0, 0, 180)
            nd.Pin('west1', width=self._WGWidth).put(0, -self._CellSize, 180)
            nd.Pin('east0', width=self._WGWidth).put(self._CellSize * 2, 0, 0)
            nd.Pin('east1', width=self._WGWidth).put(self._CellSize * 2, -self._CellSize, 0)
            nd.Pin('center', width=self._WGWidth).put(self._CellSize/2,self._CellSize/2)
        return akhetCell

    def placeCell_Delay(self, deltaLength):
        with nd.Cell(name='AkhetCell_Delay') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 0.5), (self._CellSize * 2, -self._CellSize * 0.5), (self._CellSize * 2, self._CellSize * 0.5 + self._CellSize), (0, self._CellSize * 0.5 + self._CellSize)]).put()
            nd.text(text='Delay', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 0.4)

            # TODO: Not the correct length, has to be coordinated with straight * 4 and deltalength with phase shift
            nd.bend(angle=90, radius=self._CellSize * 0.5, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)
            nd.strt(length=deltaLength, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)
            nd.bend(angle=-180, radius=self._CellSize * 0.5, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)
            nd.strt(length=deltaLength, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)
            nd.bend(angle=90, radius=self._CellSize * 0.5, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)

            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
            nd.Pin('east', width=self._WGWidth).put(self._CellSize * 2, 0, 0)
        return akhetCell


    def placeCell_Ring(self, deltaLength):
        with nd.Cell(name='AkhetCell_RingResonator') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 1.5), (self._CellSize * 2, -self._CellSize * 1.5), (self._CellSize * 2, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
            nd.text(text='Ring Resonator', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 0.4)

            # TODO: Wrong ring
            nd.strt(length=self._CellSize * 2, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)
            nd.strt(length=self._CellSize * 2, width=self._WGWidth, layer=self._SiNLayer).put(0, -self._CellSize)

            nd.Pin('west0', width=self._WGWidth).put(0, 0, 180)
            nd.Pin('west1', width=self._WGWidth).put(0, -self._CellSize, 180)
            nd.Pin('east0', width=self._WGWidth).put(self._CellSize * 2, 0, 0)
            nd.Pin('east1', width=self._WGWidth).put(self._CellSize * 2, -self._CellSize, 0)
        return akhetCell


    def placeCell_AWG(self):
        with nd.Cell(name='AkhetCell_AWG') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 1.5), (self._CellSize * 2, -self._CellSize * 1.5), (self._CellSize * 2, self._CellSize * 1.5), (0, self._CellSize * 1.5)]).put()
            nd.text(text='Arrayed Waveguide Grating', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 1.4)

            # TODO: Wrong AWG

            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
            nd.Pin('east0', width=self._WGWidth).put(self._CellSize * 2, self._CellSize, 0)
            nd.Pin('east1', width=self._WGWidth).put(self._CellSize * 2, 0, 0)
            nd.Pin('east2', width=self._WGWidth).put(self._CellSize * 2, -self._CellSize, 0)
        return akhetCell




    def placeGratingArray_East(self, numIO):
        with nd.Cell(name='AkhetGratingEast') as gratingEast:
            nd.text(text='Grating Coupler Array E', height=15, layer=self._TextLayer, align='lb').put(-280, -50)
            # TODO: Draw correct shape

            nd.Polygon(layer=self._BoxLayer, points=[(0,0), (-300, 0), (-300, -(numIO + 4) * self._GratingOffset), (0, -(numIO + 4) * self._GratingOffset)]).put(0, 0)

            for i in range(numIO):
                nd.Pin('io' + str(i), width=self._WGWidth).put(0, -i * self._GratingOffset - self._GratingOffset * 2)
        return gratingEast
