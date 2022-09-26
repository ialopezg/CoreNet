using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;

namespace ProgrammersInc.Utility
{
	/// <summary>
	/// Useful extra functions for DateTime
	/// </summary>
	public static class DateTimeEx
	{
		public static string ToRfc822( DateTime dt )
		{
			string rfc822DateTime = dt.ToString( @"ddd, dd MMM yyyy HH:mm:ss zzzz", System.Globalization.DateTimeFormatInfo.InvariantInfo );

			// remove the time separator in the timezone because zzzz will produce +01:00 for instance

			rfc822DateTime = rfc822DateTime.Remove( rfc822DateTime.LastIndexOf( ':' ), 1 );
			return rfc822DateTime;
		}

		/// <summary>
		/// Parses RFC 822 based date time strings.
		/// Example '10 Jun 2003 04:00:00 GMT'
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static DateTime? ParseRfc822( string s )
		{
			return ParseRfc822( s, true );
		}
		/// <summary>
		/// Parses ISO 8601 http://www.w3.org/TR/NOTE-datetime based date time strings.
		/// Example '1994-11-05T08:15:30-05:00'
		/// Example '1994-11-05T13:15:30Z'
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static DateTime? ParseIso8601( string s )
		{
			return ParseIso8601( s, true );
		}

		private static DateTime? ParseIso8601( string s, bool tryOtherMethods )
		{
			if( s == null || s.Trim() == string.Empty )
			{
				return null;
			}

			Match match = dateTimeRegexIso8601.Match( s );

			if( !match.Success )
			{
				if( tryOtherMethods )
				{
					DateTime dtFallback;

					if( !DateTime.TryParse( s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dtFallback ) )
					{
						dtFallback = DateTimeEx.ParseRfc822( s, false ).Value;
					}
					return dtFallback;
				}
				throw new FormatException( "Can't parse Date" );
			}

			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			int year = Handle2DigitYearValues( int.Parse( match.Groups[1].Value ) );
			int month = int.Parse( match.Groups[2].Value );
			int day = int.Parse( match.Groups[3].Value );
			int hours = 0;
			int minutes = 0;
			int seconds = 0;
			int minutesOffset = 0;

			if( !string.IsNullOrEmpty( match.Groups[4].Value ) )
			{
				hours = int.Parse( match.Groups[5].Value );
				minutes = int.Parse( match.Groups[6].Value );
				seconds = int.Parse( match.Groups[7].Value );

				int direction = match.Groups[10].Value == "+" ? -1 : 1; // May seem odd but we want to go the opposite direction to bring back to UTC
				string hourOffsetText = match.Groups[11].Value;
				string minutesOffsetText = match.Groups[12].Value;

				if( !string.IsNullOrEmpty( hourOffsetText ) )
				{
					minutesOffset += int.Parse( hourOffsetText ) * 60;
				}
				if( !string.IsNullOrEmpty( minutesOffsetText ) )
				{
					minutesOffset += int.Parse( minutesOffsetText );
				}
				minutesOffset *= direction;
			}

			//
			// Parse hour and time
			DateTime dt = new DateTime( year, month, day, hours, minutes, seconds, DateTimeKind.Utc );
			return dt.AddMinutes( minutesOffset );
		}

		private static DateTime? ParseRfc822( string s, bool tryOtherMethods )
		{
			if( s == null || s.Trim() == string.Empty )
			{
				return null;
			}

			s = RemoveOptionalDay( s );

			OffSetInfo offsetInfo = ExtractTimeZoneInfo( ref s );

			try
			{
				Match match = dateTimeRegexRfc822.Match( s );

				if( !match.Success )
				{
					if( tryOtherMethods )
					{
						DateTime dtFallback;

						if( !DateTime.TryParse( s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dtFallback ) )
						{
							dtFallback = DateTimeEx.ParseIso8601( s, false ).Value;
						}
						return dtFallback;
					}
					throw new FormatException( "Can't parse Date" );
				}

				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				int dayOfMonth = int.Parse( match.Groups[1].Value );
				
				string monthName = HandleDodgyMonthName( match.Groups[2].Value ).ToLower();
				List<string> monthNames = new List<string>();

				foreach( string mn in invariantCulture.DateTimeFormat.AbbreviatedMonthNames )
				{
					monthNames.Add( mn.ToLower() );
				}

				int month = monthNames.IndexOf( monthName ) + 1;
				int year = Handle2DigitYearValues( int.Parse( match.Groups[3].Value ) );
				int hours = int.Parse( match.Groups[4].Value );

				if( offsetInfo.HourType == HourType.PM  && hours < 12 )
				{
					hours += 12;
				}

				int minutes = int.Parse( match.Groups[5].Value );
				int seconds = int.Parse( match.Groups[6].Value );

				//
				// Parse hour and time
				DateTime dt = new DateTime( year, month, dayOfMonth, hours, minutes, seconds, DateTimeKind.Utc );

				dt = dt.AddMinutes( offsetInfo.MinutesOffset ); // move back to GMT

				return new DateTime( dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, DateTimeKind.Utc );
			}
			catch( Exception  )
			{
				//
				// last ditch attempt
				return DateTime.Parse( s );
			}
		}

		private static string HandleDodgyMonthName( string monthName )
		{
			if( monthName == "Sept" )
			{
				monthName = "Sep";
			}

			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			int monthIndex = Array.IndexOf( invariantCulture.DateTimeFormat.MonthNames, monthName );

			if( monthIndex != -1 )
			{
				monthName = invariantCulture.DateTimeFormat.AbbreviatedMonthNames[monthIndex];
			}

			return monthName;
		}

		/// <summary>
		/// Returns the full year given a 2 or 3 digit value. Pattern
		/// of behaviour from http://www.faqs.org/rfcs/rfc2822.html
		/// </summary>
		/// <param name="year"></param>
		/// <returns></returns>
		private static int Handle2DigitYearValues( int year )
		{
			if( year < 0 )
			{
				throw new FormatException( "Year values cannot be less than zero" );
			}

			if( year < 50 )
			{
				year += 2000;
			}
			else if( year < 1000 )
			{
				year += 1900;
			}
			return year;
		}

		/// <summary>
		/// Removes the optional day parameter from the supplied
		/// date.
		/// </summary>
		/// <param name="s"></param>
		private static string  RemoveOptionalDay( string s )
		{
			int startFrom = s.IndexOf( ',' );

			if( startFrom != -1 )
			{
				++startFrom;
				s = s.Substring( startFrom, s.Length - startFrom );
			}
			return s.Trim();
		}

		/// <summary>
		/// Returns the offset from UTC thed date supplied is. If
		/// none given it returns 0.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="allowZoneNames"></param>
		/// <returns>Number of minutes offset or and Hour type (AM/PM)_</returns>
		private static OffSetInfo ExtractTimeZoneInfo( ref string s )
		{
			int offset = 0;
			int foundAlphaTimeZoneIndex = -1;
			int foundNumericTimeZoneIndex = -1;
			int i = s.Length;
			bool stop = false;

			while( !stop && i-- > 0 )
			{
				switch( s[i] )
				{
 					case '+':
					case '-':
						if( foundNumericTimeZoneIndex == -1 )
						{
							foundNumericTimeZoneIndex = i;
						}
						break;
					case ':':
						stop = true;
						break;

				}
				if( char.IsLetter( s[i] ) )
				{
					foundAlphaTimeZoneIndex = i;
				}
			}

			HourType hourType = HourType.NotApplicable;

			if( stop )
			{
				if( foundNumericTimeZoneIndex != -1 )
				{
					string zoneInfo = s.Substring( foundNumericTimeZoneIndex, s.Length - foundNumericTimeZoneIndex ).Trim();
					int sign = zoneInfo[0] == '+' ? 1 : -1;

					if( zoneInfo.Length < 5 )
					{
						throw new FormatException( "Invalid zone information" );
					}

					int hours = int.Parse( zoneInfo.Substring( 1, 2 ) );
					int minutes = int.Parse( zoneInfo.Substring( 3, 2 ) );

					offset = hours * 60 + minutes;
				}
				else if( foundAlphaTimeZoneIndex != -1 )
				{
					string zoneInfo = s.Substring( foundAlphaTimeZoneIndex, s.Length - foundAlphaTimeZoneIndex ).Trim();
					OffSetInfo offsetInfo = ConvertFromZoneId( zoneInfo );

					offset = offsetInfo.MinutesOffset;
					hourType = offsetInfo.HourType;
				}

				s = s.Substring( 0, i + 3 ).Trim();
			}

			return new OffSetInfo( offset, hourType );
		}

		public enum HourType { NotApplicable, AM, PM };

		private struct OffSetInfo
		{
			public OffSetInfo( int minutesOffset )
			{
				this.MinutesOffset = minutesOffset;
				this.HourType = HourType.NotApplicable;
			}

			public OffSetInfo( int minutesOffset, HourType hourType )
			{
				this.MinutesOffset = minutesOffset;
				this.HourType = hourType;
			}

			public static OffSetInfo FromHours( int hours )
			{
				return new OffSetInfo( hours * 60, HourType.NotApplicable );
			}
			public static OffSetInfo FromHours( int hours, HourType hourType )
			{
				return new OffSetInfo( hours * 60, hourType );
			}

			public readonly int MinutesOffset;
			public readonly HourType HourType;
		}

		private static OffSetInfo ConvertFromZoneId( string id )
		{
			HourType hourType = HourType.NotApplicable;

			id = id.ToUpper().Trim();

			if( id.StartsWith( "PM" ) )
			{
				hourType = HourType.PM;
				id = id.Substring( 2 ).Trim();
			}
			else if( id.StartsWith( "AM" ) )
			{
				hourType = HourType.AM;
				id = id.Substring( 2 ).Trim();
			}

			switch( id.ToUpper() )
			{
				case "":
					if( hourType == HourType.NotApplicable )
					{
						throw new FormatException( "Unknown zone idenfier" );
					}
					else
					{
						return new OffSetInfo( 0, hourType );
					}
				case "BST":
					return OffSetInfo.FromHours( 1, hourType );
				case "Z":
				case "UT":
				case "UTC":
				case "GMT":
					return OffSetInfo.FromHours( 0, hourType );

				case "EST":
					return OffSetInfo.FromHours( -5, hourType );
				case "EDT":
					return OffSetInfo.FromHours( -4, hourType );
				case "CST":
					return OffSetInfo.FromHours( -6, hourType );
				case "CDT":
					return OffSetInfo.FromHours( -5, hourType );
				case "MST":
					return OffSetInfo.FromHours( -7, hourType );
				case "MDT":
					return OffSetInfo.FromHours( -6, hourType );
				case "PST":
					return OffSetInfo.FromHours( -8, hourType );
				case "PDT":
					return OffSetInfo.FromHours( -7, hourType );
				default:
					if( id.Length == 1 )
					{
						string positive = "NOPQRSTUVWXY";
						string negative = "ABCDEFGHIKLM"; // J not used and Z is handled in switch.
						int zoneFound = positive.IndexOf( id[0] );

						if( zoneFound != -1 )
						{
							return OffSetInfo.FromHours( zoneFound + 1, hourType );
						}

						zoneFound = negative.IndexOf( id[0] );

						if( zoneFound != -1 )
						{
							return OffSetInfo.FromHours( -zoneFound - 1 );
						}
					}
					throw new FormatException( "Unknown zone idenfier" );
			}
		}

		private static Regex dateTimeRegexRfc822 = new Regex( @"\s*(\d{1,2})\s+(Jan|January|Feb|February||Mar|March|Apr|April|May|Jun|June|Jul|July|Aug|August|Sep|Sept|September|Oct|October|Nov|November|Dec|December)\s+(\d{1,4})\s+(\d{1,2})\s*:\s*(\d{1,2})\s*:\s*(\d{1,2})", RegexOptions.Compiled | RegexOptions.IgnoreCase );
		private static Regex dateTimeRegexIso8601 = new Regex( @"\s*(\d{1,4})-(\d{1,2})-(\d{1,2})T((\d{1,2}):(\d{1,2}):(\d{1,2})(.\d{1,3})?((?<PlusOrMinus>(-|\+))(\d{1,2}):(\d{1,2})|Z))*", RegexOptions.Compiled | RegexOptions.IgnoreCase );
	}
}
