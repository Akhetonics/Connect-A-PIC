import nazca as nd
import math

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
        self._CouplerSpacing = 0.35
        self._CrossOver = 50 

        self._CellSize = 250        # Has to be a multiple of the Fiber Array pitch for the grating couplers
        
        self._SiNLayer = 203
        self._GratingLayer = 204
        self._BoxLayer = 1001
        self._TextLayer = 1002

    def placeCell_StraightWG(self):
        with nd.Cell(name='AkhetCell_StraightWG') as akhetCell:
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

            # CORNERSTONE 1550nm Grating Couplers
            nd.Polygon(layer=self._SiNLayer, points=[(0,-self._WGWidth * 0.5), (0, self._WGWidth * 0.5), (180, 5), (180, -5)]).put(0, 0)
            nd.Polygon(layer=self._SiNLayer, points=[(180, 5), (180, -5), (230, -5), (230, 5)]).put(0, 0)
            for i in range(22):
                nd.Polygon(layer=self._GratingLayer, points=[(191.62 + 1.32 * i, 5.5), (191.62 + 1.32 * i, -5.5), (192.28 + 1.32 * i, -5.5), (192.28 + 1.32 * i, 5.5)]).put(0, 0)

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

            # This is a really bad directional coupler.
            angle = 45
            radians = math.pi * angle / 180 
            offsetLength = (self._CellSize * 0.5 - (self._WGWidth + self._CouplerSpacing) * 0.5 - (self._BendRadius * (1 - math.cos(radians))) * 2) * (1 / math.cos(radians))
            coupleLength = self._CrossOver * (deltaLength / 100)
            totalLength = coupleLength + 4 * math.sin(radians) * self._BendRadius + 2 * offsetLength * math.cos(radians)

            nd.bend(angle=-angle, radius=self._BendRadius, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)
            nd.strt(length=offsetLength, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.bend(angle=angle, radius=self._BendRadius, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.strt(length=coupleLength, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.bend(angle=angle, radius=self._BendRadius, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.strt(length=offsetLength, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.bend(angle=-angle, radius=self._BendRadius, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.strt(length=self._CellSize * 2 - totalLength, width=self._WGWidth, layer=self._SiNLayer).put()
            

            nd.bend(angle=angle, radius=self._BendRadius, width=self._WGWidth, layer=self._SiNLayer).put(0, -self._CellSize)
            nd.strt(length=offsetLength, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.bend(angle=-angle, radius=self._BendRadius, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.strt(length=coupleLength, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.bend(angle=-angle, radius=self._BendRadius, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.strt(length=offsetLength, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.bend(angle=angle, radius=self._BendRadius, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.strt(length=self._CellSize * 2 - totalLength, width=self._WGWidth, layer=self._SiNLayer).put()

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


#    def placeCell_Ring(self, deltaLength):
#        with nd.Cell(name='AkhetCell_RingResonator') as akhetCell:
#            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 1.5), (self._CellSize * 2, -self._CellSize * 1.5), (self._CellSize * 2, self._CellSize * 0.5), (0, self._CellSize * 0.5)]).put()
#            nd.text(text='Ring Resonator', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 0.4)
#
#
#            nd.strt(length=self._CellSize * 2, width=self._WGWidth, layer=self._SiNLayer).put(0, 0)
#            nd.strt(length=self._CellSize * 2, width=self._WGWidth, layer=self._SiNLayer).put(0, -self._CellSize)

#            nd.Pin('west0', width=self._WGWidth).put(0, 0, 180)
#            nd.Pin('west1', width=self._WGWidth).put(0, -self._CellSize, 180)
#            nd.Pin('east0', width=self._WGWidth).put(self._CellSize * 2, 0, 0)
#            nd.Pin('east1', width=self._WGWidth).put(self._CellSize * 2, -self._CellSize, 0)
#        return akhetCell


#    def placeCell_AWG(self):
#        with nd.Cell(name='AkhetCell_AWG') as akhetCell:
#            nd.Polygon(layer=self._BoxLayer, points=[(0,-self._CellSize * 1.5), (self._CellSize * 2, -self._CellSize * 1.5), (self._CellSize * 2, self._CellSize * 1.5), (0, self._CellSize * 1.5)]).put()
#            nd.text(text='Arrayed Waveguide Grating', height=15, layer=self._TextLayer, align='lb').put(self._CellSize * 0.05, self._CellSize * 1.4)
#
#
#
#            nd.Pin('west', width=self._WGWidth).put(0, 0, 180)
#            nd.Pin('east0', width=self._WGWidth).put(self._CellSize * 2, self._CellSize, 0)
#            nd.Pin('east1', width=self._WGWidth).put(self._CellSize * 2, 0, 0)
#            nd.Pin('east2', width=self._WGWidth).put(self._CellSize * 2, -self._CellSize, 0)
#        return akhetCell



    def placeGratingArray_East(self, numIO):
        with nd.Cell(name='AkhetGratingEast') as gratingEast:
            nd.text(text='Grating Coupler Array E', height=15, layer=self._TextLayer, align='lb').put(-280, -50)
            nd.Polygon(layer=self._BoxLayer, points=[(0,20), (-550, 20), (-550, -(numIO + 3) * self._CellSize - 20), (0, -(numIO + 3) * self._CellSize - 20)]).put(0, 0)

            gratingOffset = 150

            # Grating Couplers
            for k in range(numIO + 2):
                # CORNERSTONE 1550nm Grating Couplers
                nd.Polygon(layer=self._SiNLayer, points=[(0,-self._WGWidth * 0.5), (0, self._WGWidth * 0.5), (180, 5), (180, -5)]).put(-gratingOffset, -self._CellSize * (k + 1), 180 )
                nd.Polygon(layer=self._SiNLayer, points=[(180, 5), (180, -5), (230, -5), (230, 5)]).put(-gratingOffset, -self._CellSize * (k + 1), 180)
                for i in range(22):
                    nd.Polygon(layer=self._GratingLayer, points=[(191.62 + 1.32 * i, 5.5), (191.62 + 1.32 * i, -5.5), (192.28 + 1.32 * i, -5.5), (192.28 + 1.32 * i, 5.5)]).put(-gratingOffset, -self._CellSize * (k + 1), 180)
                
                if k > 0 and k < (numIO + 1):
                    nd.strt(length=gratingOffset, width=self._WGWidth, layer=self._SiNLayer).put(-gratingOffset, -self._CellSize * (k + 1))
                    
            # Grating Coupler Locator
            nd.bend(angle=180, radius=self._CellSize * 0.5, width=self._WGWidth, layer=self._SiNLayer).put(-gratingOffset, -self._CellSize * (1))
            nd.strt(length=self._CellSize, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.bend(angle=90, radius=self._CellSize * 0.5, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.strt(length=self._CellSize * (numIO + 2), width=self._WGWidth, layer=self._SiNLayer).put()
            nd.bend(angle=90, radius=self._CellSize * 0.5, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.strt(length=self._CellSize, width=self._WGWidth, layer=self._SiNLayer).put()
            nd.bend(angle=180, radius=self._CellSize * 0.5, width=self._WGWidth, layer=self._SiNLayer).put()

            for i in range(numIO):
                nd.Pin('io' + str(i), width=self._WGWidth).put(0, -i * self._CellSize - self._CellSize * 2)
        return gratingEast
