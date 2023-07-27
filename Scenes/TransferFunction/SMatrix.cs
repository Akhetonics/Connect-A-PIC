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
		public readonly List<Pin>? PinReference;
		private readonly int size;

		public SMatrix(List<Pin> allPinsInGrid)
		{
			if (allPinsInGrid != null && allPinsInGrid.Count > 0)
			{
				this.size = allPinsInGrid.Count;
			}
			else
			{
				this.size = 1;
			}

			this.SMat = Matrix<Complex>.Build.Dense(this.size, this.size);
			this.PinReference = allPinsInGrid;
		}

		public void setValues(Dictionary<(Pin, Pin), Complex> transfers, bool reset = false)
		{
			if (transfers == null || this.PinReference == null)
			{
				return;
			}

			// Reset matrix
			if (reset == true)
			{
				this.SMat = Matrix<Complex>.Build.Dense(this.size, this.size);
			}
			
			foreach (var relation in transfers.Keys)
			{
				if (PinReference.Contains(relation.Item1) && PinReference.Contains(relation.Item2))
				{
					// TODO: These might need to be switched?
					int row = PinReference.IndexOf(relation.Item2);
					int col = PinReference.IndexOf(relation.Item1);

					this.SMat[row, col] = transfers[relation];
				}
			}
		}

		public Dictionary<(Pin, Pin), Complex> getValues()
		{
			var transfers = new Dictionary<(Pin, Pin), Complex>();
			for (int i = 0; i < this.size; i++)
			{
				for (int j = 0; j < this.size; j++)
				{
					if (this.SMat[i, j] != 0)
					{
						transfers[new (this.PinReference[j], this.PinReference[i])] = this.SMat[i, j];
					}
				}
			}
			return transfers;
		}

		public static SMatrix createSystemSMatrix(List<SMatrix> matrices)
		{
			var portsReference = matrices.SelectMany(x => x.PinReference).Distinct().ToList();
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
