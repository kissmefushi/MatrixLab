using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MatrixLibrary;

namespace MatrixLibrary.Tests
{
    [TestClass]
    public class UnitTest
    {
        private const double Delta = 1e-9;

        //1. сума
        [TestMethod]
        public void Addition_SameSizeMatrices_ReturnsElementWiseSum()
        {
            //arrange
            Matrix first = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix second = new Matrix(2, 2, 5, 6, 7, 8);
            Matrix expected = new Matrix(2, 2, 6, 8, 10, 12);

            Matrix actual = first + second; //act

            Assert.AreEqual(expected, actual); //assert
        }

        //2. сума 2x3
        [TestMethod]
        public void Addition_RectangularMatrices_ReturnsSumOfSameSize()
        {
            //arrange
            Matrix first = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);
            Matrix second = new Matrix(2, 3, -1, 0, 1, 0.5, 0.5, -6);
            Matrix expected = new Matrix(2, 3, 0, 2, 4, 4.5, 5.5, 0);

            Matrix actual = first + second; //act

            Assert.AreEqual(expected, actual); //assert
        }

        //3. різні розміри
        [TestMethod]
        public void Addition_DifferentSizes_ThrowsArgumentException()
        {
            //arrange
            Matrix first = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix second = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);

            Action act = () => { Matrix result = first + second; }; //act

            Assert.ThrowsException<ArgumentException>(act); //assert
        }

        //4. null
        [TestMethod]
        public void Addition_NullOperand_ThrowsArgumentNullException()
        {
            //arrange
            Matrix first = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix second = null;

            Action act = () => { Matrix result = first + second; }; //act

            Assert.ThrowsException<ArgumentNullException>(act); //assert
        }

        //5. переповнення
        [TestMethod]
        public void Addition_ResultOutOfDoubleRange_ThrowsOverflowException()
        {
            //arrange
            Matrix first = new Matrix(1, 1, 1e308);
            Matrix second = new Matrix(1, 1, 1e308);

            Action act = () => { Matrix result = first + second; }; //act

            Assert.ThrowsException<OverflowException>(act); //assert
        }

        //6. добуток
        [TestMethod]
        public void Multiplication_SquareMatrices_ReturnsProduct()
        {
            //arrange
            Matrix first = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix second = new Matrix(2, 2, 5, 6, 7, 8);
            Matrix expected = new Matrix(2, 2, 19, 22, 43, 50);

            Matrix actual = first * second; //act

            Assert.AreEqual(expected, actual); //assert
        }

        //7. 2x3 на 3x2
        [TestMethod]
        public void Multiplication_RectangularMatrices_ReturnsProductOfRowsByColumnsSize()
        {
            //arrange
            Matrix first = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);
            Matrix second = new Matrix(3, 2, 7, 8, 9, 10, 11, 12);
            Matrix expected = new Matrix(2, 2, 58, 64, 139, 154);

            Matrix actual = first * second; //act

            Assert.AreEqual(expected, actual); //assert
        }

        //8. одинична матриця
        [TestMethod]
        public void Multiplication_ByIdentityMatrix_ReturnsSameMatrix()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix identity = new Matrix(2, 2, 1, 0, 0, 1);

            Matrix actual = matrix * identity; //act

            Assert.AreEqual(matrix, actual); //assert
        }

        //9. A * B != B * A
        [TestMethod]
        public void Multiplication_SwappedOperands_ReturnsDifferentResult()
        {
            //arrange
            Matrix first = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix second = new Matrix(2, 2, 0, 1, 1, 0);

            //act
            Matrix firstBySecond = first * second;
            Matrix secondByFirst = second * first;

            Assert.AreNotEqual(firstBySecond, secondByFirst); //assert
        }

        //10. неузгоджені розміри
        [TestMethod]
        public void Multiplication_IncompatibleSizes_ThrowsArgumentException()
        {
            //arrange
            Matrix first = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);
            Matrix second = new Matrix(2, 2, 1, 2, 3, 4);

            Action act = () => { Matrix result = first * second; }; //act

            Assert.ThrowsException<ArgumentException>(act); //assert
        }

        //11. null
        [TestMethod]
        public void Multiplication_NullOperand_ThrowsArgumentNullException()
        {
            //arrange
            Matrix first = null;
            Matrix second = new Matrix(2, 2, 1, 2, 3, 4);

            Action act = () => { Matrix result = first * second; }; //act

            Assert.ThrowsException<ArgumentNullException>(act); //assert
        }

        //12. на -1.5
        [TestMethod]
        public void MultiplicationByNumber_NegativeFraction_ReturnsScaledMatrix()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 2, -4, 6, 0);
            Matrix expected = new Matrix(2, 2, -3, 6, -9, 0);

            Matrix actual = matrix * -1.5; //act

            Assert.AreEqual(expected, actual); //assert
        }

        //13. число зліва
        [TestMethod]
        public void MultiplicationByNumber_NumberOnLeft_ReturnsSameResultAsOnRight()
        {
            //arrange
            Matrix matrix = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);
            Matrix expected = matrix * 3;

            Matrix actual = 3 * matrix; //act

            Assert.AreEqual(expected, actual); //assert
        }

        //14. на нуль
        [TestMethod]
        public void MultiplicationByNumber_Zero_ReturnsZeroMatrix()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix expected = new Matrix(2, 2);

            Matrix actual = matrix * 0; //act

            Assert.AreEqual(expected, actual); //assert
        }

        //15. NaN
        [TestMethod]
        public void MultiplicationByNumber_NaN_ThrowsArgumentException()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 3, 4);

            Action act = () => { Matrix result = matrix * double.NaN; }; //act

            Assert.ThrowsException<ArgumentException>(act); //assert
        }

        //16. переповнення
        [TestMethod]
        public void MultiplicationByNumber_ResultOutOfDoubleRange_ThrowsOverflowException()
        {
            //arrange
            Matrix matrix = new Matrix(1, 2, 1, 1e300);

            Action act = () => { Matrix result = matrix * 1e10; }; //act

            Assert.ThrowsException<OverflowException>(act); //assert
        }

        //17. визначник 1x1
        [TestMethod]
        public void Determinant_OneByOneMatrix_ReturnsElement()
        {
            //arrange
            Matrix matrix = new Matrix(1, 1, -7.5);
            double expected = -7.5;

            double actual = ~matrix; //act

            Assert.AreEqual(expected, actual, Delta); //assert
        }

        //18. визначник 2x2
        [TestMethod]
        public void Determinant_TwoByTwoMatrix_ReturnsAdMinusBc()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 3, 4);
            double expected = -2;

            double actual = ~matrix; //act

            Assert.AreEqual(expected, actual, Delta); //assert
        }

        //19. визначник 3x3
        [TestMethod]
        public void Determinant_ThreeByThreeMatrix_ReturnsCorrectValue()
        {
            //arrange
            Matrix matrix = new Matrix(3, 3, 2, -1, 0, 1, 3, 4, 0, 5, 1);
            double expected = -33;

            double actual = ~matrix; //act

            Assert.AreEqual(expected, actual, Delta); //assert
        }

        //20. перестановка рядків
        [TestMethod]
        public void Determinant_ZeroOnDiagonal_SwapsRowsAndReturnsCorrectSign()
        {
            //arrange
            Matrix matrix = new Matrix(3, 3, 0, 1, 0, 1, 0, 0, 0, 0, 1);
            double expected = -1;

            double actual = ~matrix; //act

            Assert.AreEqual(expected, actual, Delta); //assert
        }

        //21. вироджена
        [TestMethod]
        public void Determinant_SingularMatrix_ReturnsZero()
        {
            //arrange
            Matrix matrix = new Matrix(3, 3, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            double expected = 0;

            double actual = ~matrix; //act

            Assert.AreEqual(expected, actual, Delta); //assert
        }

        //22. ~(A * B) = ~A * ~B
        [TestMethod]
        public void Determinant_ProductOfMatrices_EqualsProductOfDeterminants()
        {
            //arrange
            Matrix first = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix second = new Matrix(2, 2, 2, 0, 1, 3);
            double expected = (~first) * (~second);

            double actual = ~(first * second); //act

            Assert.AreEqual(expected, actual, Delta); //assert
        }

        //23. не квадратна
        [TestMethod]
        public void Determinant_NonSquareMatrix_ThrowsInvalidOperationException()
        {
            //arrange
            Matrix matrix = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);

            Action act = () => { double result = ~matrix; }; //act

            Assert.ThrowsException<InvalidOperationException>(act); //assert
        }

        //24. true
        [TestMethod]
        public void OperatorTrue_NonSingularMatrix_ReturnsTrue()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 3, 4);

            bool actual = matrix ? true : false; //act

            Assert.IsTrue(actual); //assert
        }

        //25. false, вироджена
        [TestMethod]
        public void OperatorFalse_SingularMatrix_ReturnsFalse()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 2, 4);

            bool actual = matrix ? true : false; //act

            Assert.IsFalse(actual); //assert
        }

        //26. false, не квадратна
        [TestMethod]
        public void OperatorFalse_NonSquareMatrix_ReturnsFalse()
        {
            //arrange
            Matrix matrix = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);

            bool actual = matrix ? true : false; //act

            Assert.IsFalse(actual); //assert
        }

        //27. неявне приведення
        [TestMethod]
        public void ImplicitConversion_From2DArray_CreatesMatrixWithSameElements()
        {
            //arrange
            double[,] array = { { 1, 2, 3 }, { 4, 5, 6 } };
            Matrix expected = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);

            Matrix actual = array; //act

            Assert.AreEqual(expected, actual); //assert
        }

        //28. явне приведення
        [TestMethod]
        public void ExplicitConversion_OneByOneMatrixToDouble_ReturnsElement()
        {
            //arrange
            Matrix matrix = new Matrix(1, 1, 7.5);
            double expected = 7.5;

            double actual = (double)matrix; //act

            Assert.AreEqual(expected, actual, Delta); //assert
        }

        //29. явне, не 1x1
        [TestMethod]
        public void ExplicitConversion_NotOneByOneMatrix_ThrowsInvalidCastException()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 3, 4);

            Action act = () => { double result = (double)matrix; }; //act

            Assert.ThrowsException<InvalidCastException>(act); //assert
        }

        //30. == копія
        [TestMethod]
        public void Equality_CopyOfMatrix_ReturnsTrue()
        {
            //arrange
            Matrix first = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix second = new Matrix(first);

            bool actual = first == second; //act

            Assert.IsTrue(actual); //assert
        }

        //31. == в межах Epsilon
        [TestMethod]
        public void Equality_DifferenceWithinEpsilon_ReturnsTrue()
        {
            //arrange
            Matrix first = new Matrix(1, 2, 0.1 + 0.2, 1);
            Matrix second = new Matrix(1, 2, 0.3, 1);

            bool actual = first == second; //act

            Assert.IsTrue(actual); //assert
        }

        //32. != різні розміри
        [TestMethod]
        public void Inequality_SameElementsDifferentSizes_ReturnsTrue()
        {
            //arrange
            Matrix first = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);
            Matrix second = new Matrix(3, 2, 1, 2, 3, 4, 5, 6);

            bool actual = first != second; //act

            Assert.IsTrue(actual); //assert
        }

        //33. == null
        [TestMethod]
        public void Equality_MatrixAndNull_ReturnsFalse()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix nothing = null;

            bool actual = matrix == nothing; //act

            Assert.IsFalse(actual); //assert
        }

        //34. індексатор
        [TestMethod]
        public void Indexer_SetElement_ChangesOnlyThisElement()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix expected = new Matrix(2, 2, 1, 10, 3, 4);

            matrix[0, 1] = 10; //act

            Assert.AreEqual(expected, matrix); //assert
        }

        //35. індекс за межами
        [TestMethod]
        public void Indexer_RowOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, 2, 3, 4);

            Action act = () => { double value = matrix[2, 0]; }; //act

            Assert.ThrowsException<ArgumentOutOfRangeException>(act); //assert
        }

        //36. кількість елементів
        [TestMethod]
        public void Constructor_ElementsCountMismatch_ThrowsArgumentException()
        {
            //arrange
            double[] elements = { 1, 2, 3 };

            Action act = () => new Matrix(2, 2, elements); //act

            Assert.ThrowsException<ArgumentException>(act); //assert
        }

        //37. нуль рядків
        [TestMethod]
        public void Constructor_ZeroRows_ThrowsArgumentOutOfRangeException()
        {
            //arrange
            int rows = 0;

            Action act = () => new Matrix(rows, 2); //act

            Assert.ThrowsException<ArgumentOutOfRangeException>(act); //assert
        }

        //38. зміна масиву
        [TestMethod]
        public void Constructor_SourceArrayChangedAfterCreation_MatrixUnchanged()
        {
            //arrange
            double[] source = { 1, 2, 3, 4 };
            Matrix matrix = new Matrix(2, 2, source);
            Matrix expected = new Matrix(2, 2, 1, 2, 3, 4);

            source[0] = 100; //act

            Assert.AreEqual(expected, matrix); //assert
        }

        //39. зміна копії
        [TestMethod]
        public void CopyConstructor_CopyChanged_OriginalUnchanged()
        {
            //arrange
            Matrix original = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix copy = new Matrix(original);
            Matrix expected = new Matrix(2, 2, 1, 2, 3, 4);

            copy[0, 0] = 100; //act

            Assert.AreEqual(expected, original); //assert
        }

        //40. за замовчуванням
        [TestMethod]
        public void DefaultConstructor_NoArguments_CreatesZeroOneByOneMatrix()
        {
            //arrange
            Matrix expected = new Matrix(1, 1, 0);

            Matrix actual = new Matrix(); //act

            Assert.AreEqual(expected, actual); //assert
        }

        //41. ToString
        [TestMethod]
        public void ToString_Matrix_ReturnsRowsWithAlignedColumns()
        {
            //arrange
            Matrix matrix = new Matrix(2, 2, 1, -2.5, 30, 4);
            string expected = " 1   -2.5" + Environment.NewLine + "30      4";

            string actual = matrix.ToString(); //act

            Assert.AreEqual(expected, actual); //assert
        }

        //42. Input
        [TestMethod]
        public void Input_ValidData_ReplacesSizeAndElements()
        {
            //arrange
            Matrix matrix = new Matrix();
            StringReader reader = new StringReader("2\n3\n1 2 3\n4 5,5 6\n");
            Matrix expected = new Matrix(2, 3, 1, 2, 3, 4, 5.5, 6);

            matrix.Input(reader, TextWriter.Null); //act

            Assert.AreEqual(expected, matrix); //assert
        }

        //43. Input з помилками
        [TestMethod]
        public void Input_InvalidDataThenValid_RepeatsRequestAndReadsMatrix()
        {
            //arrange
            Matrix matrix = new Matrix();
            StringReader reader = new StringReader("0\n2\n2\nabc 1\n1 2\n3\n3 4\n");
            Matrix expected = new Matrix(2, 2, 1, 2, 3, 4);

            matrix.Input(reader, TextWriter.Null); //act

            Assert.AreEqual(expected, matrix); //assert
        }

        //44. Input, мало даних
        [TestMethod]
        public void Input_NotEnoughData_ThrowsEndOfStreamException()
        {
            //arrange
            Matrix matrix = new Matrix();
            StringReader reader = new StringReader("2\n2\n1 2\n");

            Action act = () => matrix.Input(reader, TextWriter.Null); //act

            Assert.ThrowsException<EndOfStreamException>(act); //assert
        }
    }
}
