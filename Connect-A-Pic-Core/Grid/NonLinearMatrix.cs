using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Grid
{
    public class NonLinearMatrix<T> : Matrix<T> where T : struct, IEquatable<T>, IFormattable
    {
        public NonLinearMatrix(MatrixStorage<T> storage) : base(storage)
        {
        }

        public override Cholesky<T> Cholesky()
        {
            throw new NotImplementedException();
        }

        public override void CoerceZero(double threshold)
        {
            throw new NotImplementedException();
        }

        public override Vector<T> ColumnAbsoluteSums()
        {
            throw new NotImplementedException();
        }

        public override Vector<double> ColumnNorms(double norm)
        {
            throw new NotImplementedException();
        }

        public override Vector<T> ColumnSums()
        {
            throw new NotImplementedException();
        }

        public override Matrix<T> ConjugateTranspose()
        {
            throw new NotImplementedException();
        }

        public override void ConjugateTranspose(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        public override Evd<T> Evd(Symmetricity symmetricity = Symmetricity.Unknown)
        {
            throw new NotImplementedException();
        }

        public override double FrobeniusNorm()
        {
            throw new NotImplementedException();
        }

        public override GramSchmidt<T> GramSchmidt()
        {
            throw new NotImplementedException();
        }

        public override double InfinityNorm()
        {
            throw new NotImplementedException();
        }

        public override bool IsHermitian()
        {
            throw new NotImplementedException();
        }

        public override double L1Norm()
        {
            throw new NotImplementedException();
        }

        public override LU<T> LU()
        {
            throw new NotImplementedException();
        }

        public override Matrix<T> NormalizeColumns(double norm)
        {
            throw new NotImplementedException();
        }

        public override Matrix<T> NormalizeRows(double norm)
        {
            throw new NotImplementedException();
        }

        public override Matrix<T> PseudoInverse()
        {
            throw new NotImplementedException();
        }

        public override QR<T> QR(QRMethod method = QRMethod.Thin)
        {
            throw new NotImplementedException();
        }

        public override Vector<T> RowAbsoluteSums()
        {
            throw new NotImplementedException();
        }

        public override Vector<double> RowNorms(double norm)
        {
            throw new NotImplementedException();
        }

        public override Vector<T> RowSums()
        {
            throw new NotImplementedException();
        }

        public override Svd<T> Svd(bool computeVectors = true)
        {
            throw new NotImplementedException();
        }

        public override T Trace()
        {
            throw new NotImplementedException();
        }

        protected override void DoAdd(T scalar, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoAdd(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoConjugate(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoConjugateTransposeAndMultiply(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoConjugateTransposeThisAndMultiply(Vector<T> rightSide, Vector<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoConjugateTransposeThisAndMultiply(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoDivide(T divisor, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoDivideByThis(T dividend, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoModulus(T divisor, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoModulusByThis(T dividend, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoMultiply(T scalar, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoMultiply(Vector<T> rightSide, Vector<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoMultiply(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoNegate(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseAbs(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseAbsoluteMaximum(T scalar, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseAbsoluteMaximum(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseAbsoluteMinimum(T scalar, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseAbsoluteMinimum(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseAcos(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseAsin(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseAtan(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseAtan2(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseCeiling(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseCos(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseCosh(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseDivide(Matrix<T> divisor, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseExp(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseFloor(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseLog(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseLog10(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseMaximum(T scalar, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseMaximum(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseMinimum(T scalar, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseMinimum(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseModulus(Matrix<T> divisor, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseMultiply(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwisePower(T exponent, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwisePower(Matrix<T> exponent, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseRemainder(Matrix<T> divisor, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseRound(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseSign(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseSin(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseSinh(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseSqrt(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseTan(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoPointwiseTanh(Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoRemainder(T divisor, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoRemainderByThis(T dividend, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoSubtract(T scalar, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoSubtract(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoTransposeAndMultiply(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoTransposeThisAndMultiply(Vector<T> rightSide, Vector<T> result)
        {
            throw new NotImplementedException();
        }

        protected override void DoTransposeThisAndMultiply(Matrix<T> other, Matrix<T> result)
        {
            throw new NotImplementedException();
        }
    }
}
