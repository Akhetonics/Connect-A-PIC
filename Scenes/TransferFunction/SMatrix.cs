using MathNet.Numerics.LinearAlgebra;
using System.Numerics;
using System.Collections.Generic;
using System;
using System.Linq;
using ConnectAPIC.Scenes.Component;

namespace TransferFunction
{
	public class SMatrix
	{
		public Matrix<Complex> SMat;
		public readonly List<ConnectionPort>? PortsReference;
		private readonly int _size;

		public SMatrix(List<ConnectionPort> ports)
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

		public void setValues(Dictionary<Tuple<ConnectionPort, ConnectionPort>, Complex> transfers, bool reset = false)
		{
			if (transfers == null || this.PortsReference == null)
			{
				return;
			}

			// Reset matrix
			if (reset == true)
			{
				this.SMat = Matrix<Complex>.Build.Dense(this._size, this._size);
			}
			
			foreach (var relation in transfers.Keys)
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

		public Dictionary<Tuple<ConnectionPort, ConnectionPort>, Complex> getValues()
		{
			var transfers = new Dictionary<Tuple<ConnectionPort, ConnectionPort>, Complex>();
			for (int i = 0; i < this._size; i++)
			{
				for (int j = 0; j < this._size; j++)
				{
					if (this.SMat[i, j] != 0)
					{
						transfers[new Tuple<ConnectionPort, ConnectionPort>(this.PortsReference[j], this.PortsReference[i])] = this.SMat[i, j];
					}
				}
			}
			return transfers;
		}

		public static SMatrix createSystemSMatrix(List<SMatrix> matrices)
		{
			var portsReference = matrices.SelectMany(x => x.PortsReference).Distinct().ToList();
			SMatrix sysMat = new SMatrix(portsReference);

			foreach (SMatrix matrix in matrices)
			{
				var transfers = matrix.getValues();
				sysMat.setValues(transfers);
			}

			return sysMat;
		}

		// Takes this matrix to the power n and sets it permanently to that value.
		public void setToPower(int n)
		{
			this.SMat = this.SMat.Power(n);
		}

		public override string ToString()
		{
			// TODO: 
			return "";
		}
	}
}
