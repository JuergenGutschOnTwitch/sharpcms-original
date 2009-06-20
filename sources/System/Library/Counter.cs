//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

namespace InventIt.SiteSystem.Library
{
    /// <summary>
    /// Counter class for measuring elapsed times.
    /// Uses the NT performance counters
    /// </summary>
    public class Counter
    {
        /*	long elapsedCount = 0;
		long startCount = 0;

		/// <summary>
		/// Start timing
		/// </summary>
		public void Start()
		{
			startCount = 0;
			QueryPerformanceCounter(ref startCount);
		}

		/// <summary>
		/// Stop timing
		/// </summary>
		public void Stop()
		{
			long stopCount = 0;
			QueryPerformanceCounter(ref stopCount);

			elapsedCount += (stopCount - startCount);
		}

		/// <summary>
		/// Clear the timer
		/// </summary>
		public void Clear()
		{
			elapsedCount = 0;
		}

		/// <summary>
		/// The elapsed time measured in seconds.
		/// </summary>
		public float Seconds
		{
			get
			{
				long freq = 0;
				QueryPerformanceFrequency(ref freq);
				return ((float)elapsedCount / (float)freq);
			}
		}

		/// <summary>
		/// The elapsed time measured in milliseconds.
		/// </summary>
		public float Milliseconds
		{
			get
			{
				return Seconds * 1000;
			}
		}

		/// <summary>
		/// Convert to a string
		/// </summary>
		/// <returns>The current elapsed time in seconds</returns>
		public override string ToString()
		{
			return String.Format("{0} seconds", Seconds);
		}

		static long Frequency
		{
			get
			{
				long freq = 0;
				QueryPerformanceFrequency(ref freq);
				return freq;
			}
		}

		static long Value
		{
			get
			{
				long count = 0;
				QueryPerformanceCounter(ref count);
				return count;
			}
		}

		[System.Runtime.InteropServices.DllImport("KERNEL32")]
		private static extern bool QueryPerformanceCounter(ref long
		lpPerformanceCount);

		[System.Runtime.InteropServices.DllImport("KERNEL32")]
		private static extern bool QueryPerformanceFrequency(ref long
		lpFrequency);*/
    }
}