using System;
using System.Collections.Generic;

namespace AlgorithmsTimeTest
{
    public static class Approximator
    {
        // МНК для одного коэффициента C: T(n) ≈ C * f(n)
        public static (double C, double MSE) Fit(List<(int N, double T)> data, string model)
        {
            var f = ModelFunction(model);

            double sumTF = 0, sumFF = 0;
            foreach (var p in data)
            {
                double fn = f(p.N);
                sumTF += p.T * fn;
                sumFF += fn * fn;
            }

            double C = sumFF == 0 ? 0 : sumTF / sumFF;

            double mse = 0;
            foreach (var p in data)
            {
                double diff = p.T - C * f(p.N);
                mse += diff * diff;
            }
            mse /= data.Count;

            return (C, mse);
        }

        // Функция f(n) по имени модели
        public static Func<int, double> ModelFunction(string model)
        {
            switch (model)
            {
                case "1": return n => 1.0;
                case "n": return n => (double)n;
                case "n log n": return n => n * Math.Log(n <= 1 ? 2 : n, 2);
                case "n^2": return n => (double)n * n;
                case "n^3": return n => (double)n * n * n;
                case "log n": return n => Math.Log(n <= 1 ? 2 : n, 2);

                // Для Штрассена: O(n^log2(7)) ≈ O(n^2.807)
                case "n^2.807": return n => Math.Pow(n, 2.807);

                // Для решета Эратосфена: O(n log log n)
                case "n log log n":
                    return n =>
                    {
                        int nn = n <= 2 ? 2 : n;
                        return nn * Math.Log(Math.Log(nn, 2), 2);
                    };

                default:
                    throw new ArgumentException("Неизвестная модель: " + model);
            }
        }

        // Автоподбор лучшей модели по минимальному MSE
        public static string BestModel(List<(int N, double T)> data,
                                       IEnumerable<string> candidates)
        {
            string best = null;
            double bestMse = double.MaxValue;

            foreach (var model in candidates)
            {
                var r = Fit(data, model);
                if (r.MSE < bestMse)
                {
                    bestMse = r.MSE;
                    best = model;
                }
            }
            return best;
        }
    }
}