import nazca as nd
from nazca.interconnects import Interconnect
ic = nd.interconnects.Interconnect(layer=1)
def MMI_Cell(cellsize):
    wg = 1
    
 
    MMI_LENGTH = 160 + 10
    MMI_WIDTH = 15
    with nd.Cell(name='MMI') as cell:
        mmi = MMI_3x3().put('a1',(cellsize - MMI_LENGTH)/2)
        #inputs and outputs to the cell
 
        
        in0 = ic.strt(length=5).put(0, -cellsize/2 + 125)
        in1 = ic.strt(length=5).put(0, 0)
        in2 = ic.strt(length=5).put(0, cellsize/2 - 125)
        out0 = ic.strt(length=5).put(cellsize-5, -cellsize/2 + 125)
        out1 = ic.strt(length=5).put(cellsize-5, 0)
        out2 = ic.strt(length=5).put(cellsize-5, cellsize/2 - 125)
        
        #connect ports to MMI

        ic.cobra_p2p(pin1=in0.pin['b0'], pin2=mmi.pin['a0']).put()
        ic.cobra_p2p(pin1=in1.pin['b0'], pin2=mmi.pin['a1']).put()
        ic.cobra_p2p(pin1=in2.pin['b0'], pin2=mmi.pin['a2']).put()
        ic.cobra_p2p(pin1=out0.pin['a0'], pin2=mmi.pin['b0']).put()
        ic.cobra_p2p(pin1=out1.pin['a0'], pin2=mmi.pin['b1']).put()
        ic.cobra_p2p(pin1=out2.pin['a0'], pin2=mmi.pin['b2']).put()

        return cell

def MMI_3x3():
    
    width=15
    length=160
    wg = 1
    taper_width = 3
    taper_length = 10
    delta_spacing = 4
    with nd.Cell(name='MMI_3x3') as mmi:
        
        body = ic.strt(length=length, width=width, arrow=False).put(0,0)
        # east tapers
        t_e0 = ic.taper(width1=wg, width2=taper_width, length=taper_length, arrow=False).put(-taper_length, - delta_spacing)
        t_e1 = ic.taper(width1=wg, width2=taper_width, length=taper_length, arrow=False).put(-taper_length, 0)
        t_e2 = ic.taper(width1=wg, width2=taper_width, length=taper_length, arrow=False).put(-taper_length, delta_spacing)
        t_w0 = ic.taper(width2=wg, width1=taper_width, length=taper_length, arrow=False).put(length, - delta_spacing)
        t_w1 = ic.taper(width2=wg, width1=taper_width, length=taper_length, arrow=False).put(length, 0)
        t_w2 = ic.taper(width2=wg, width1=taper_width, length=taper_length, arrow=False).put(length, delta_spacing)

        nd.Pin('a0').put(t_e0.pin['a0'])
        nd.Pin('a1').put(t_e1.pin['a0'])
        nd.Pin('a2').put(t_e2.pin['a0'])
        nd.Pin('b0').put(t_w0.pin['b0'])
        nd.Pin('b1').put(t_w1.pin['b0'])
        nd.Pin('b2').put(t_w2.pin['b0'])
        #nd.put_stub()
        return mmi

if __name__ == "__main__":
    
    MMI_Cell(750).put()
    nd.export_gds()