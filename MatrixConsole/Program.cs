using System;
using System.Globalization;
using System.Text;
using MatrixLibrary;

namespace MatrixConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Matrix a = new Matrix(2, 2, 1, 2, 3, 4);
            Matrix b = new Matrix(2, 2, 2, 0, 1, 3);
            Matrix c = new Matrix(2, 3, 1, 0, 2, -1, 3, 1);
            Matrix d = new Matrix(3, 3, 2, -1, 0, 1, 3, 4, 0, 5, 1);
            Matrix s = new Matrix(2, 2, 1, 2, 2, 4);
            Matrix zero = new Matrix(2, 3);
            Matrix byDefault = new Matrix();
            Matrix copyOfA = new Matrix(a);

            Section("1. конструктори та ToString");
            Print("A", a);
            Print("B", b);
            Print("C", c);
            Print("D", d);
            Print("S", s);
            Print("нульова матриця 2x3", zero);
            Print("матриця за замовчуванням", byDefault);
            Print("копія A", copyOfA);
            Pause();

            Section("2. додавання матриць");
            Print("A + B", a + b);

            Section("3. множення матриць");
            Print("A * B", a * b);
            Print("A * C", a * c);

            Section("4. множення матриці на число");
            Print("A * 2.5", a * 2.5);
            Print("-1 * B", -1 * b);
            Pause();

            Section("5. визначник (~)");
            Console.WriteLine("~A = " + Format(~a));
            Console.WriteLine("~B = " + Format(~b));
            Console.WriteLine("~(A * B) = " + Format(~(a * b)) + ", ~A * ~B = " + Format(~a * ~b));
            Console.WriteLine("~D = " + Format(~d));
            Console.WriteLine("~S = " + Format(~s));
            Console.WriteLine();

            Section("6. операції true і false");
            CheckTruth("A", a);
            CheckTruth("S", s);
            CheckTruth("C", c);
            Console.WriteLine();

            Section("7. приведення типів");
            Matrix fromArray = new double[,] { { 4, 1 }, { 2, 3 } };
            Print("неявне приведення double[,] до Matrix", fromArray);
            Matrix single = new Matrix(1, 1, 7.5);
            double number = (double)single;
            Console.WriteLine("явне приведення матриці 1x1 до double: " + Format(number));
            Console.WriteLine();

            Section("8. порівняння");
            Matrix almostA = new Matrix(2, 2, 1 + 1e-12, 2, 3, 4);
            Console.WriteLine("A == копія A: " + ToText(a == copyOfA));
            Console.WriteLine("A == A', де A' відрізняється від A на 1e-12: " + ToText(a == almostA));
            Console.WriteLine("A != B: " + ToText(a != b));
            Console.WriteLine("A != C: " + ToText(a != c));
            Console.WriteLine();
            Pause();

            Section("9. індексатор");
            Console.WriteLine("A[1, 0] = " + Format(a[1, 0]));
            Console.WriteLine();
            copyOfA[0, 1] = 10;
            Print("копія A після copyOfA[0, 1] = 10", copyOfA);
            Console.WriteLine("A[0, 1] = " + Format(a[0, 1]));
            Console.WriteLine("A == копія A: " + ToText(a == copyOfA));
            Console.WriteLine();

            Section("10. помилкові операції");
            TryOperation("A + C", () => Console.WriteLine(a + c));
            TryOperation("C * A", () => Console.WriteLine(c * a));
            TryOperation("~C", () => Console.WriteLine(~c));
            TryOperation("(double)A", () => Console.WriteLine((double)a));
            TryOperation("A[2, 0]", () => Console.WriteLine(a[2, 0]));
            Console.WriteLine();

            Section("11. введення матриці з клавіатури");
            Matrix m = new Matrix();
            m.Input();
            Console.WriteLine();
            Print("M", m);
            if (m)
            {
                Console.WriteLine("M невироджена, ~M = " + Format(~m));
            }
            else if (m.IsSquare)
            {
                Console.WriteLine("M вироджена, ~M = 0");
            }
            else
            {
                Console.WriteLine("M не квадратна, визначник не існує");
            }

            Console.WriteLine();
            Console.Write("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }

        static void Section(string title)
        {
            Console.WriteLine();
            Console.WriteLine(title);
            Console.WriteLine();
        }

        static void Print(string name, Matrix matrix)
        {
            Console.WriteLine(name + ":");
            Console.WriteLine(matrix.ToString());
            Console.WriteLine();
        }

        static void Pause()
        {
            Console.Write("Натисніть Enter, щоб продовжити...");
            Console.ReadLine();
        }

        static void CheckTruth(string name, Matrix matrix)
        {
            if (matrix)
            {
                Console.WriteLine(name + ": true, матриця невироджена");
            }
            else
            {
                Console.WriteLine(name + ": false, матриця вироджена або не квадратна");
            }
        }

        static void TryOperation(string expression, Action operation)
        {
            try
            {
                operation();
            }
            catch (Exception ex)
            {
                Console.WriteLine(expression + " -> " + ex.GetType().Name + ": " + ex.Message.Split('\r', '\n')[0]);
            }
        }

        static string Format(double value)
        {
            string text = value.ToString("0.####", CultureInfo.InvariantCulture);
            return text == "-0" ? "0" : text;
        }

        static string ToText(bool value)
        {
            return value ? "true" : "false";
        }
    }
}
