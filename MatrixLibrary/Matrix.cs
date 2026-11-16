using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace MatrixLibrary
{
    public class Matrix
    {
        public const double Epsilon = 1e-9;

        private int rows;
        private int columns;
        private double[] elements;

        public Matrix()
            : this(1, 1)
        {
        }

        public Matrix(int rows, int columns)
        {
            CheckSize(rows, columns);
            this.rows = rows;
            this.columns = columns;
            elements = new double[rows * columns];
        }

        public Matrix(int rows, int columns, params double[] elements)
        {
            CheckSize(rows, columns);
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }
            if (elements.Length != rows * columns)
            {
                throw new ArgumentException("Кількість елементів не відповідає розмірності матриці.", nameof(elements));
            }
            for (int i = 0; i < elements.Length; i++)
            {
                CheckFinite(elements[i], nameof(elements));
            }
            this.rows = rows;
            this.columns = columns;
            this.elements = new double[elements.Length];
            Array.Copy(elements, this.elements, elements.Length);
        }

        public Matrix(Matrix other)
        {
            CheckNotNull(other, nameof(other));
            rows = other.rows;
            columns = other.columns;
            elements = new double[other.elements.Length];
            Array.Copy(other.elements, elements, elements.Length);
        }

        public int Rows
        {
            get { return rows; }
        }

        public int Columns
        {
            get { return columns; }
        }

        public bool IsSquare
        {
            get { return rows == columns; }
        }

        public double this[int row, int column]
        {
            get
            {
                CheckIndex(row, column);
                return elements[row * columns + column];
            }
            set
            {
                CheckIndex(row, column);
                CheckFinite(value, nameof(value));
                elements[row * columns + column] = value;
            }
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            CheckNotNull(left, nameof(left));
            CheckNotNull(right, nameof(right));
            if (left.rows != right.rows || left.columns != right.columns)
            {
                throw new ArgumentException("Для додавання матриці повинні мати однакову розмірність.");
            }
            double[] result = new double[left.elements.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = left.elements[i] + right.elements[i];
            }
            return CreateResult(left.rows, left.columns, result);
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            CheckNotNull(left, nameof(left));
            CheckNotNull(right, nameof(right));
            if (left.columns != right.rows)
            {
                throw new ArgumentException("Кількість стовпців першої матриці повинна дорівнювати кількості рядків другої.");
            }
            double[] result = new double[left.rows * right.columns];
            for (int i = 0; i < left.rows; i++)
            {
                for (int j = 0; j < right.columns; j++)
                {
                    double sum = 0.0;
                    for (int k = 0; k < left.columns; k++)
                    {
                        sum += left.elements[i * left.columns + k] * right.elements[k * right.columns + j];
                    }
                    result[i * right.columns + j] = sum;
                }
            }
            return CreateResult(left.rows, right.columns, result);
        }

        public static Matrix operator *(Matrix matrix, double number)
        {
            CheckNotNull(matrix, nameof(matrix));
            CheckFinite(number, nameof(number));
            double[] result = new double[matrix.elements.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = matrix.elements[i] * number;
            }
            return CreateResult(matrix.rows, matrix.columns, result);
        }

        public static Matrix operator *(double number, Matrix matrix)
        {
            return matrix * number;
        }

        public static double operator ~(Matrix matrix)
        {
            CheckNotNull(matrix, nameof(matrix));
            if (!matrix.IsSquare)
            {
                throw new InvalidOperationException("Визначник можна обчислити лише для квадратної матриці.");
            }
            int n = matrix.rows;
            double[] a = new double[matrix.elements.Length];
            Array.Copy(matrix.elements, a, a.Length);
            double determinant = 1.0;
            for (int k = 0; k < n; k++)
            {
                int pivotRow = k;
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(a[i * n + k]) > Math.Abs(a[pivotRow * n + k]))
                    {
                        pivotRow = i;
                    }
                }
                if (Math.Abs(a[pivotRow * n + k]) < Epsilon)
                {
                    return 0.0;
                }
                if (pivotRow != k)
                {
                    SwapRows(a, n, k, pivotRow);
                    determinant = -determinant;
                }
                double pivot = a[k * n + k];
                determinant *= pivot;
                for (int i = k + 1; i < n; i++)
                {
                    double factor = a[i * n + k] / pivot;
                    for (int j = k; j < n; j++)
                    {
                        a[i * n + j] -= factor * a[k * n + j];
                    }
                }
            }
            if (double.IsInfinity(determinant) || double.IsNaN(determinant))
            {
                throw new OverflowException("Визначник виходить за межі типу double.");
            }
            return determinant;
        }

        public static bool operator true(Matrix matrix)
        {
            return IsNonSingular(matrix);
        }

        public static bool operator false(Matrix matrix)
        {
            return !IsNonSingular(matrix);
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (left is null || right is null)
            {
                return false;
            }
            if (left.rows != right.rows || left.columns != right.columns)
            {
                return false;
            }
            for (int i = 0; i < left.elements.Length; i++)
            {
                if (Math.Abs(left.elements[i] - right.elements[i]) > Epsilon)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            return !(left == right);
        }

        public static implicit operator Matrix(double[,] array)
        {
            if (array == null)
            {
                return null;
            }
            int arrayRows = array.GetLength(0);
            int arrayColumns = array.GetLength(1);
            double[] flat = new double[arrayRows * arrayColumns];
            for (int i = 0; i < arrayRows; i++)
            {
                for (int j = 0; j < arrayColumns; j++)
                {
                    flat[i * arrayColumns + j] = array[i, j];
                }
            }
            return new Matrix(arrayRows, arrayColumns, flat);
        }

        public static explicit operator double(Matrix matrix)
        {
            CheckNotNull(matrix, nameof(matrix));
            if (matrix.rows != 1 || matrix.columns != 1)
            {
                throw new InvalidCastException("До числа можна привести лише матрицю розміру 1x1.");
            }
            return matrix.elements[0];
        }

        public void Input()
        {
            Input(Console.In, Console.Out);
        }

        public void Input(TextReader reader, TextWriter writer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            int newRows = ReadPositiveInt(reader, writer, "Кількість рядків: ");
            int newColumns = ReadPositiveInt(reader, writer, "Кількість стовпців: ");
            double[] newElements = new double[newRows * newColumns];
            for (int i = 0; i < newRows; i++)
            {
                double[] row = ReadRow(reader, writer, i + 1, newColumns);
                Array.Copy(row, 0, newElements, i * newColumns, newColumns);
            }
            rows = newRows;
            columns = newColumns;
            elements = newElements;
        }

        public override bool Equals(object obj)
        {
            return this == (obj as Matrix);
        }

        public override int GetHashCode()
        {
            return (rows * 397) ^ columns;
        }

        public override string ToString()
        {
            string[] cells = new string[elements.Length];
            int[] widths = new int[columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    string cell = FormatNumber(elements[i * columns + j]);
                    cells[i * columns + j] = cell;
                    widths[j] = Math.Max(widths[j], cell.Length);
                }
            }
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                if (i > 0)
                {
                    text.AppendLine();
                }
                for (int j = 0; j < columns; j++)
                {
                    if (j > 0)
                    {
                        text.Append("   ");
                    }
                    text.Append(cells[i * columns + j].PadLeft(widths[j]));
                }
            }
            return text.ToString();
        }

        private static bool IsNonSingular(Matrix matrix)
        {
            return !(matrix is null) && matrix.IsSquare && Math.Abs(~matrix) >= Epsilon;
        }

        private static Matrix CreateResult(int rows, int columns, double[] result)
        {
            for (int i = 0; i < result.Length; i++)
            {
                if (double.IsInfinity(result[i]) || double.IsNaN(result[i]))
                {
                    throw new OverflowException("Елемент результату виходить за межі типу double.");
                }
            }
            return new Matrix(rows, columns, result);
        }

        private static void SwapRows(double[] a, int n, int first, int second)
        {
            for (int j = 0; j < n; j++)
            {
                double temp = a[first * n + j];
                a[first * n + j] = a[second * n + j];
                a[second * n + j] = temp;
            }
        }

        private void CheckIndex(int row, int column)
        {
            if (row < 0 || row >= rows)
            {
                throw new ArgumentOutOfRangeException(nameof(row), "Номер рядка виходить за межі матриці.");
            }
            if (column < 0 || column >= columns)
            {
                throw new ArgumentOutOfRangeException(nameof(column), "Номер стовпця виходить за межі матриці.");
            }
        }

        private static void CheckSize(int rows, int columns)
        {
            if (rows <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), "Кількість рядків повинна бути додатною.");
            }
            if (columns <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), "Кількість стовпців повинна бути додатною.");
            }
        }

        private static void CheckNotNull(Matrix matrix, string paramName)
        {
            if (matrix is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void CheckFinite(double number, string paramName)
        {
            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                throw new ArgumentException("Число повинно бути скінченним.", paramName);
            }
        }

        private static string FormatNumber(double value)
        {
            if (Math.Abs(value) < Epsilon)
            {
                return "0";
            }
            string text = value.ToString("0.####", CultureInfo.InvariantCulture);
            return text == "-0" ? "0" : text;
        }

        private static int ReadPositiveInt(TextReader reader, TextWriter writer, string prompt)
        {
            while (true)
            {
                writer.Write(prompt);
                int value;
                if (int.TryParse(ReadLineOrThrow(reader).Trim(), out value) && value > 0)
                {
                    return value;
                }
                writer.WriteLine("Помилка: потрібно ввести ціле додатне число.");
            }
        }

        private static double[] ReadRow(TextReader reader, TextWriter writer, int rowNumber, int count)
        {
            while (true)
            {
                writer.Write("Рядок " + rowNumber + ": ");
                string[] parts = ReadLineOrThrow(reader).Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == count)
                {
                    double[] row = new double[count];
                    bool isValid = true;
                    for (int k = 0; k < count && isValid; k++)
                    {
                        isValid = TryParseNumber(parts[k], out row[k]);
                    }
                    if (isValid)
                    {
                        return row;
                    }
                }
                writer.WriteLine("Помилка: введіть числа через пробіл, кількість чисел - " + count + ".");
            }
        }

        private static bool TryParseNumber(string text, out double value)
        {
            return double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static string ReadLineOrThrow(TextReader reader)
        {
            string line = reader.ReadLine();
            if (line == null)
            {
                throw new EndOfStreamException("Введення матриці перервано: дані закінчилися.");
            }
            return line;
        }
    }
}
