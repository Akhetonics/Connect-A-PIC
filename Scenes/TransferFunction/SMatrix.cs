using MathNet.Numerics.LinearAlgebra;
using System.Numerics;
using System.Collections.Generic;

public class SMatrix
{
    public Matrix<Complex> SMat;
    public readonly List<ComponentPort>? PortsReference;
    private readonly int _size;

    public SMatrix(List<ComponentPort> ports)
    {
        if (ports != null && ports.Count > 0)
        {
            this._size = ports.Count;
        }
        else
        {
            this._size = 1;
        }

        this.SMat = Matrix<Complex>.Build.Dense(this._size, this._size);
        this.PortsReference = ports;
    }

    public void setValues(Dictionary<Tuple<ComponentPort, ComponentPort>, Complex> transfers)
    {
        if (transfers == null || this.PortsReference == null)
        {
            return;
        }

        // Reset matrix
        this.SMat = Matrix<Complex>.Build.Dense(this._size, this._size);


        foreach(Tuple<ComponentPort, ComponentPort> relation in transfers.Keys)
        {
            if (PortsReference.Contains(relation.Item1) && PortsReference.Contains(relation.Item2))
            {
                // TODO: These might need to be switched?
                int row = PortsReference.IndexOf(relation.Item2);
                int col = PortsReference.IndexOf(relation.Item1);

                this.SMat[row, col] = transfers[relation];
            }
        }
    }

    public override string ToString()
    {
        // TODO: 
        return "";
    }
}