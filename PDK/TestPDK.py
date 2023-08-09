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
        self._WGWidth = 1.1      
        self._CellSize = 250            
        self._GratingOffset = self._CellSize        
        self._SiNLayer = 1
        self._BoxLayer = 1001
        self._PinLayer = 1002

    def placeCell_StraightWG(self):
        with nd.Cell(name='AkhetCell_StraightWG') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 0.5), (self._CellSize, -self._CellSize * 0.5), (self._CellSize, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
            
            nd.strt(length=self._CellSize, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)

            nd.Pin('south', width=self._WGWidth).put(self._CellSize * 0.5, -self._CellSize * 0.5, 270)
            nd.Pin('east', width=self._WGWidth).put(self._CellSize, 0, 0)
            nd.Pin('north', width=self._WGWidth).put(self._CellSize * 0.5, self._CellSize * 0.5, 90)
            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
        return akhetCell

    def placeCell_BendWG(self):
        with nd.Cell(name='AkhetCell_BendWG') as akhetCell:
            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 0.5), (self._CellSize, -self._CellSize * 0.5), (self._CellSize, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
            
            nd.bend(angle=-90, radius=self._CellSize * 0.5, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)

            nd.Pin('south', width=self._WGWidth).put(self._CellSize * 0.5, -self._CellSize * 0.5, 270)
            nd.Pin('east', width=self._WGWidth).put(self._CellSize, 0, 0)
            nd.Pin('north', width=self._WGWidth).put(self._CellSize * 0.5, self._CellSize * 0.5, 90)
            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
        return akhetCell
    
    def placeGrating_East(self, numIO):
        with nd.Cell(name='AkhetGratingEast') as gratingEast:
            # TODO: Draw correct shape

            nd.Polygon(layer=self._BoxLayer, points=[(0,0), (-300, 0), (-300, -(numIO + 4) * self._GratingOffset), (0, -(numIO + 4) * self._GratingOffset)]).put()

            for i in range(numIO):
                nd.Pin('io' + str(i), width=self._WGWidth).put(0, -i * self._GratingOffset - self._GratingOffset * 2)
        return gratingEast
