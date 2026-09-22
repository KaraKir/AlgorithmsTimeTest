using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AlgorithmApp
{
    public static class Algorithms
    {
        // Сортировка массива
        public static (string result, double ms) SortArray(int[] data)
        {
            var sw = Stopwatch.StartNew();
            Array.Sort(data);
            sw.Stop();
            return ($"Отсортировано {data.Length} элементов. Первые: " +
                    string.Join(", ", data.Take(5)), sw.Elapsed.TotalMilliseconds);
        }

        // Поиск простых чисел (решето Эратосфена)
        public static (string result, double ms) FindPrimes(int[] data)
        {
            var sw = Stopwatch.StartNew();
            int limit = data.Length > 0 ? data.Max() : 0;
            if (limit < 2)
            {
                sw.Stop();
                return ("Простых чисел не найдено", sw.Elapsed.TotalMilliseconds);
            }

            var sieve = new bool[limit + 1];
            for (int i = 2; i <= limit; i++) sieve[i] = true;

            for (int i = 2; i * i <= limit; i++)
            {
                if (sieve[i])
                {
                    for (int j = i * i; j <= limit; j += i)
                        sieve[j] = false;
                }
            }

            int count = sieve.Count(x => x);
            sw.Stop();
            return ($"Найдено простых чисел: {count}", sw.Elapsed.TotalMilliseconds);
        }

        // Вычисление суммы всех элементов
        public static (string result, double ms) SumArray(int[] data)
        {
            var sw = Stopwatch.StartNew();
            long sum = 0;
            foreach (var x in data) sum += x;
            sw.Stop();
            return ($"Сумма = {sum}", sw.Elapsed.TotalMilliseconds);
        }

        // Факториал (для демонстрации "тяжёлого" вычисления)
        public static (string result, double ms) Factorial(int[] data)
        {
            int n = data.Length > 0 ? Math.Min(data[0], 20) : 0;
            var sw = Stopwatch.StartNew();
            long result = 1;
            for (int i = 2; i <= n; i++) result *= i;
            sw.Stop();
            return ($"{n}! = {result}", sw.Elapsed.TotalMilliseconds);
        }
    }
}