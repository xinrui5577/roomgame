/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

namespace Org.BouncyCastle.Utilities.Date
{
	public class DateTimeUtilities
	{
		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

		private DateTimeUtilities()
		{
		}

		/// <summary>
		/// Return the number of milliseconds since the Unix epoch (1 Jan., 1970 UTC) for a given DateTime value.
		/// </summary>
		/// <param name="dateTime">A UTC DateTime value not before epoch.</param>
		/// <returns>Number of whole milliseconds after epoch.</returns>
		/// <exception cref="ArgumentException">'dateTime' is before epoch.</exception>
		public static long DateTimeToUnixMs(
			DateTime dateTime)
		{
			if (dateTime.CompareTo(UnixEpoch) < 0)
				throw new ArgumentException("DateTime value may not be before the epoch", "dateTime");

			return (dateTime.Ticks - UnixEpoch.Ticks) / TimeSpan.TicksPerMillisecond;
		}

		/// <summary>
		/// Create a DateTime value from the number of milliseconds since the Unix epoch (1 Jan., 1970 UTC).
		/// </summary>
		/// <param name="unixMs">Number of milliseconds since the epoch.</param>
		/// <returns>A UTC DateTime value</returns>
		public static DateTime UnixMsToDateTime(
			long unixMs)
		{
			return new DateTime(unixMs * TimeSpan.TicksPerMillisecond + UnixEpoch.Ticks);
		}

		/// <summary>
		/// Return the current number of milliseconds since the Unix epoch (1 Jan., 1970 UTC).
		/// </summary>
		public static long CurrentUnixMs()
		{
			return DateTimeToUnixMs(DateTime.UtcNow);
		}
	}
}

#endif
