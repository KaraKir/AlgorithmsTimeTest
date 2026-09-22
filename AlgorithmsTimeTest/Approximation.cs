using System;

namespace AlgorithmsTimeTest
{
    public class Approximation
    {
        public long Id { get; set; }
        public string Algorithm { get; set; }
        public string Model { get; set; }       // "n", "n log n", "n^2", "n^3"
        public double C { get; set; }
        public double MSE { get; set; }
        public DateTime Timestamp { get; set; }
    }
}