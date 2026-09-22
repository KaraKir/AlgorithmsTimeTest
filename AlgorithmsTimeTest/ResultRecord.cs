using System;

namespace AlgorithmApp
{
    public class ResultRecord
    {
        public string AlgorithmName { get; set; }
        public double ElapsedMilliseconds { get; set; }
        public string InputFile { get; set; }
        public DateTime Timestamp { get; set; }
        public string ResultSummary { get; set; }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss}\t{AlgorithmName}\t" +
                   $"{ElapsedMilliseconds:F2} мс\t{InputFile}\t{ResultSummary}";
        }
    }
}