using System;
using System.Diagnostics;

namespace Bankrablas_bb_13c
{
    /// <summary>
    /// Lenyege hogy szamolja a a futasidot
    /// </summary>
    public class Stopper
    {
        private Stopwatch stopwatch;

        public Stopper()
        {
            stopwatch = new Stopwatch();
        }

        /// <summary>
        /// stopper start
        /// </summary>
        public void Start()
        {
            stopwatch.Start();
        }

        /// <summary>
        /// stopper stop
        /// </summary>
        public void Stop()
        {
            stopwatch.Stop();
        }

        /// <summary>
        /// Eltelt ido megformazasa
        /// </summary>
        /// <returns>eletelt ido (hh:mm:ss)</returns>
        public string GetElapsedTime()
        {
            TimeSpan elapsed = stopwatch.Elapsed;
            return $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
        }
    }
}
