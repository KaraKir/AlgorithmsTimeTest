using System;
using System.Collections.Generic;

namespace AlgorithmsTimeTest
{
    public static class AlgorithmRegistry
    {
        public class AlgorithmInfo
        {
            public string Key;
            public string Display;
            public string TheoreticalModel;
            public Func<int, object> GenerateInput;
            public Action<object> Run;

            // true — алгоритм принимает int[] и годится для «серии по файлу»
            public bool FileSeriesCompatible;
        }

        public static readonly List<AlgorithmInfo> All = new List<AlgorithmInfo>
        {
            new AlgorithmInfo
            {
                Key = "Const", Display = "Постоянная f(v) = 1",
                TheoreticalModel = "1",
                GenerateInput = n => GenerateVector(n),
                Run = input => { },
                FileSeriesCompatible = true
            },
            new AlgorithmInfo
            {
                Key = "SumVector", Display = "Сумма элементов",
                TheoreticalModel = "n",
                GenerateInput = n => GenerateVector(n),
                Run = input => SumVector((int[])input),
                FileSeriesCompatible = true
            },
            new AlgorithmInfo
            {
                Key = "ProductVector", Display = "Произведение элементов",
                TheoreticalModel = "n",
                GenerateInput = n => GenerateVector(n),
                Run = input => ProductVector((int[])input),
                FileSeriesCompatible = true
            },
            new AlgorithmInfo
            {
                Key = "PolyNaive", Display = "Полином (наивно)",
                TheoreticalModel = "n^2",
                GenerateInput = n => new object[] { GenerateVector(n), 1.5 },
                Run = input =>
                {
                    var arr = (object[])input;
                    PolyNaive((int[])arr[0], (double)arr[1]);
                },
                FileSeriesCompatible = false
            },
            new AlgorithmInfo
            {
                Key = "PolyHorner", Display = "Полином (Горнер)",
                TheoreticalModel = "n",
                GenerateInput = n => new object[] { GenerateVector(n), 1.5 },
                Run = input =>
                {
                    var arr = (object[])input;
                    PolyHorner((int[])arr[0], (double)arr[1]);
                },
                FileSeriesCompatible = false
            },
            new AlgorithmInfo
            {
                Key = "BubbleSort", Display = "Сортировка пузырьком",
                TheoreticalModel = "n^2",
                GenerateInput = n => GenerateVector(n),
                Run = input => BubbleSort((int[])input),
                FileSeriesCompatible = true
            },
            new AlgorithmInfo
            {
                Key = "QuickSort", Display = "Быстрая сортировка",
                TheoreticalModel = "n log n",
                GenerateInput = n => GenerateVector(n),
                Run = input =>
                {
                    var v = (int[])input;
                    QuickSort(v, 0, v.Length - 1);
                },
                FileSeriesCompatible = true
            },
            new AlgorithmInfo
            {
                Key = "Timsort", Display = "Timsort (стандартная сортировка)",
                TheoreticalModel = "n log n",
                GenerateInput = n => GenerateVector(n),
                Run = input => Array.Sort((int[])input),
                FileSeriesCompatible = true
            },
            new AlgorithmInfo
            {
                Key = "TimsortReal", Display = "Timsort (собственная реализация)",
                TheoreticalModel = "n log n",
                GenerateInput = n => GenerateVector(n),
                Run = input => Timsort((int[])input),
                FileSeriesCompatible = true
            },
            new AlgorithmInfo
            {
                Key = "MergeSort", Display = "Сортировка слиянием",
                TheoreticalModel = "n log n",
                GenerateInput = n => GenerateVector(n),
                Run = input =>
                {
                    var v = (int[])input;
                    MergeSort(v, 0, v.Length - 1);
                },
                FileSeriesCompatible = true
            },
            new AlgorithmInfo
            {
                Key = "MatrixMul", Display = "Умножение матриц (классическое)",
                TheoreticalModel = "n^3",
                GenerateInput = n => new object[] { GenerateMatrix(n, n), GenerateMatrix(n, n) },
                Run = input =>
                {
                    var arr = (object[])input;
                    MatrixMul((int[,])arr[0], (int[,])arr[1]);
                },
                FileSeriesCompatible = false
            },
            new AlgorithmInfo
            {
                Key = "Strassen", Display = "Алгоритм Штрассена (умножение матриц)",
                TheoreticalModel = "n^2.807",
                GenerateInput = n => new object[] { GenerateMatrix(n, n), GenerateMatrix(n, n) },
                Run = input =>
                {
                    var arr = (object[])input;
                    Strassen((int[,])arr[0], (int[,])arr[1]);
                },
                FileSeriesCompatible = false
            },
            new AlgorithmInfo
            {
                Key = "Dijkstra", Display = "Алгоритм Дейкстры (матрица смежности)",
                TheoreticalModel = "n^2",
                GenerateInput = n => GenerateGraph(n),
                Run = input => Dijkstra((int[,])input, 0),
                FileSeriesCompatible = false
            },
            new AlgorithmInfo
            {
                Key = "PowNaive", Display = "Возведение в степень (наивно)",
                TheoreticalModel = "n",
                GenerateInput = n => n,
                Run = input => { },
                FileSeriesCompatible = false
            },
            new AlgorithmInfo
            {
                Key = "PowRecursive", Display = "Возведение в степень (рекурсия)",
                TheoreticalModel = "n",
                GenerateInput = n => n,
                Run = input => { },
                FileSeriesCompatible = false
            },
            new AlgorithmInfo
            {
                Key = "PowFast", Display = "Возведение в степень (быстрое)",
                TheoreticalModel = "log n",
                GenerateInput = n => n,
                Run = input => { },
                FileSeriesCompatible = false
            }
        };

        // =================== Генераторы данных ===================

        public static int[] GenerateVector(int n)
        {
            var rnd = new Random(42);
            var v = new int[n];
            for (int i = 0; i < n; i++) v[i] = rnd.Next(1, 1000);
            return v;
        }

        public static int[,] GenerateMatrix(int rows, int cols)
        {
            var rnd = new Random(42);
            var m = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    m[i, j] = rnd.Next(1, 10);
            return m;
        }

        // Генератор связного графа в виде матрицы смежности.
        // 0 означает отсутствие ребра.
        public static int[,] GenerateGraph(int n)
        {
            var rnd = new Random(42);
            var g = new int[n, n];

            // Гарантируем связность: цепочка 0 → 1 → 2 → ... → n-1
            for (int i = 0; i < n - 1; i++)
                g[i, i + 1] = rnd.Next(1, 10);

            // Добавляем случайные рёбра (плотность ~30%)
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i != j && rnd.NextDouble() < 0.3)
                        g[i, j] = rnd.Next(1, 10);

            return g;
        }

        // =================== Алгоритмы ===================

        public static long SumVector(int[] v)
        {
            long s = 0;
            foreach (var x in v) s += x;
            return s;
        }

        public static long ProductVector(int[] v)
        {
            long p = 1;
            foreach (var x in v) p *= x;
            return p;
        }

        public static double PolyNaive(int[] v, double x)
        {
            double sum = 0;
            for (int k = 0; k < v.Length; k++)
                sum += v[k] * Math.Pow(x, k);
            return sum;
        }

        public static double PolyHorner(int[] v, double x)
        {
            double p = 0;
            for (int k = v.Length - 1; k >= 0; k--)
                p = p * x + v[k];
            return p;
        }

        public static void BubbleSort(int[] v)
        {
            for (int i = 0; i < v.Length - 1; i++)
                for (int j = 0; j < v.Length - 1 - i; j++)
                    if (v[j] > v[j + 1])
                    {
                        int t = v[j]; v[j] = v[j + 1]; v[j + 1] = t;
                    }
        }

        public static void QuickSort(int[] v, int lo, int hi)
        {
            if (lo >= hi) return;
            int p = v[(lo + hi) / 2], i = lo, j = hi;
            while (i <= j)
            {
                while (v[i] < p) i++;
                while (v[j] > p) j--;
                if (i <= j)
                {
                    int t = v[i]; v[i] = v[j]; v[j] = t;
                    i++; j--;
                }
            }
            QuickSort(v, lo, j);
            QuickSort(v, i, hi);
        }

        public static int[,] MatrixMul(int[,] A, int[,] B)
        {
            int n = A.GetLength(0), m = B.GetLength(1), k = B.GetLength(0);
            var C = new int[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                {
                    int s = 0;
                    for (int t = 0; t < k; t++)
                        s += A[i, t] * B[t, j];
                    C[i, j] = s;
                }
            return C;
        }

        // ============ Timsort (собственная реализация) ============

        private const int RUN = 32;

        public static void Timsort(int[] arr)
        {
            int n = arr.Length;
            for (int i = 0; i < n; i += RUN)
                InsertionSort(arr, i, Math.Min(i + RUN - 1, n - 1));

            for (int size = RUN; size < n; size = 2 * size)
            {
                for (int left = 0; left < n; left += 2 * size)
                {
                    int mid = left + size - 1;
                    int right = Math.Min(left + 2 * size - 1, n - 1);
                    if (mid < right)
                        Merge(arr, left, mid, right);
                }
            }
        }

        private static void InsertionSort(int[] arr, int left, int right)
        {
            for (int i = left + 1; i <= right; i++)
            {
                int temp = arr[i];
                int j = i - 1;
                while (j >= left && arr[j] > temp)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = temp;
            }
        }

        private static void Merge(int[] arr, int l, int m, int r)
        {
            int len1 = m - l + 1, len2 = r - m;
            var left = new int[len1];
            var right = new int[len2];
            for (int i = 0; i < len1; i++) left[i] = arr[l + i];
            for (int i = 0; i < len2; i++) right[i] = arr[m + 1 + i];

            int a = 0, b = 0, k = l;
            while (a < len1 && b < len2)
            {
                if (left[a] <= right[b]) arr[k++] = left[a++];
                else arr[k++] = right[b++];
            }
            while (a < len1) arr[k++] = left[a++];
            while (b < len2) arr[k++] = right[b++];
        }

        // ============ Сортировка слиянием ============

        public static void MergeSort(int[] arr, int left, int right)
        {
            if (left >= right) return;
            int mid = (left + right) / 2;
            MergeSort(arr, left, mid);
            MergeSort(arr, mid + 1, right);
            Merge(arr, left, mid, right);
        }

        // ============ Алгоритм Штрассена ============

        public static int[,] Strassen(int[,] A, int[,] B)
        {
            int n = A.GetLength(0);

            int m = 1;
            while (m < n) m *= 2;

            if (m != n)
            {
                var Ap = new int[m, m];
                var Bp = new int[m, m];
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        Ap[i, j] = A[i, j];
                        Bp[i, j] = B[i, j];
                    }
                A = Ap; B = Bp;
            }

            var C = StrassenRec(A, B);

            var result = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    result[i, j] = C[i, j];
            return result;
        }

        private static int[,] StrassenRec(int[,] A, int[,] B)
        {
            int n = A.GetLength(0);

            if (n <= 2) return MatrixMul(A, B);

            int h = n / 2;
            var a11 = Slice(A, 0, 0, h); var a12 = Slice(A, 0, h, h);
            var a21 = Slice(A, h, 0, h); var a22 = Slice(A, h, h, h);
            var b11 = Slice(B, 0, 0, h); var b12 = Slice(B, 0, h, h);
            var b21 = Slice(B, h, 0, h); var b22 = Slice(B, h, h, h);

            var m1 = StrassenRec(AddMatrix(a11, a22), AddMatrix(b11, b22));
            var m2 = StrassenRec(AddMatrix(a21, a22), b11);
            var m3 = StrassenRec(a11, SubMatrix(b12, b22));
            var m4 = StrassenRec(a22, SubMatrix(b21, b11));
            var m5 = StrassenRec(AddMatrix(a11, a12), b22);
            var m6 = StrassenRec(SubMatrix(a21, a11), AddMatrix(b11, b12));
            var m7 = StrassenRec(SubMatrix(a12, a22), AddMatrix(b21, b22));

            var c11 = AddMatrix(SubMatrix(AddMatrix(m1, m4), m5), m7);
            var c12 = AddMatrix(m3, m5);
            var c21 = AddMatrix(m2, m4);
            var c22 = AddMatrix(SubMatrix(AddMatrix(m1, m3), m2), m6);

            return Combine(c11, c12, c21, c22);
        }

        private static int[,] Slice(int[,] M, int r, int c, int h)
        {
            var R = new int[h, h];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < h; j++)
                    R[i, j] = M[r + i, c + j];
            return R;
        }

        private static int[,] AddMatrix(int[,] A, int[,] B)
        {
            int n = A.GetLength(0);
            var R = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    R[i, j] = A[i, j] + B[i, j];
            return R;
        }

        private static int[,] SubMatrix(int[,] A, int[,] B)
        {
            int n = A.GetLength(0);
            var R = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    R[i, j] = A[i, j] - B[i, j];
            return R;
        }

        private static int[,] Combine(int[,] c11, int[,] c12, int[,] c21, int[,] c22)
        {
            int h = c11.GetLength(0);
            var R = new int[2 * h, 2 * h];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < h; j++)
                {
                    R[i, j] = c11[i, j];
                    R[i, j + h] = c12[i, j];
                    R[i + h, j] = c21[i, j];
                    R[i + h, j + h] = c22[i, j];
                }
            return R;
        }

        // ============ Алгоритм Дейкстры (матрица смежности) ============

        public static long[] Dijkstra(int[,] graph, int source)
        {
            int n = graph.GetLength(0);
            var dist = new long[n];
            var visited = new bool[n];

            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            dist[source] = 0;

            for (int iter = 0; iter < n; iter++)
            {
                int u = -1;
                long best = long.MaxValue;
                for (int i = 0; i < n; i++)
                    if (!visited[i] && dist[i] < best) { best = dist[i]; u = i; }

                if (u == -1) break;
                visited[u] = true;

                for (int v = 0; v < n; v++)
                {
                    if (graph[u, v] > 0 && dist[u] != long.MaxValue &&
                        dist[u] + graph[u, v] < dist[v])
                    {
                        dist[v] = dist[u] + graph[u, v];
                    }
                }
            }

            return dist;
        }

        // =================== Часть IV: подсчёт шагов ===================

        public static long PowNaiveSteps(double x, int n)
        {
            long steps = 0;
            double r = 1;
            for (int i = 0; i < n; i++) { r *= x; steps++; }
            return steps;
        }

        public static long PowRecursiveSteps(double x, int n)
        {
            if (n == 0) return 0;
            return 1 + PowRecursiveSteps(x, n - 1);
        }

        public static long PowFastSteps(double x, int n)
        {
            if (n == 0) return 0;
            if (n % 2 == 0)
            {
                long half = PowFastSteps(x, n / 2);
                return 1 + 2 * half;
            }
            return 1 + PowFastSteps(x, n - 1);
        }
    }
}