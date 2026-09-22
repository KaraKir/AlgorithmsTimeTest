using System;

namespace AlgorithmsTimeTest
{
    public class Measurement
    {
        public long Id { get; set; }
        public string Algorithm { get; set; }   // ключ, напр. "BubbleSort"
        public int N { get; set; }              // размер входа
        public double Value { get; set; }       // мс или число шагов
        public int RunIndex { get; set; }       // 1..5
        public DateTime Timestamp { get; set; }
    }
}