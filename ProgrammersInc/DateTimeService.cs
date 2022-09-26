using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace ProgrammersInc
{
    /// <summary>
    /// Clase que define los metodos para la manipulacion de objetos del
    /// tipo <see cref="System.DateTime"/>.
    /// </summary>
    public sealed class DateTimeService
    {
        /// <summary>
        /// Definicion de los intervalos del tiempo.
        /// </summary>
        public enum Intervals
        {
            /// <summary>
            /// Identifica la unidad de medida de tiempo: milisegundo.
            /// </summary>
            Milliseconds,
            /// <summary>
            /// Identifica la unidad de medida de tiempo: segundo.
            /// </summary>
            Seconds,
            /// <summary>
            /// Identifica la unidad de medida de tiempo: minuto.
            /// </summary>
            Minutes,
            /// <summary>
            /// Identifica la unidad de medida de tiempo: hora.
            /// </summary>
            Hours,
            /// <summary>
            /// Identifica la unidad de medida de tiempo: dia.
            /// </summary>
            Days,
            /// <summary>
            /// Identifica la unidad de medida de tiempo: semana.
            /// </summary>
            Weeks,
            /// <summary>
            /// Identifica la unidad de medida de tiempo: mes.
            /// </summary>
            Months,
            /// <summary>
            /// Identifica la unidad de medida de tiempo: trimestre.
            /// </summary>
            Quarters,
            /// <summary>
            /// Identifica la unidad de medida de tiempo: año.
            /// </summary>
            Years
        }

        /// <summary>
        /// Compara dos <see cref="System.DateTime"/> y segun el <see cref="Intervals"/> dado, 
        /// devuelve un numero <see cref="System.Int64"/> representado la diferencia entre estas.
        /// </summary>
        /// <param name="interval">Intervalo de tiempo que indicara el tipo de comparacion.</param>
        /// <param name="startDate">Valor <see cref="System.DateTime"/> inicial.</param>
        /// <param name="endDate">Valor <see cref="System.DateTime"/> final.</param>
        /// <returns>Un numero <see cref="System.Int64"/> representado la diferencia entre las dos fechas.</returns>
        public static long DateTimeDiff(Intervals interval, DateTime startDate, DateTime endDate)
        {
            TimeSpan timeSpan = new TimeSpan(endDate.Ticks - startDate.Ticks);

            switch (interval)
            {
                case Intervals.Milliseconds:
                    return (long)timeSpan.TotalMilliseconds;
                case Intervals.Seconds:
                    return (long)timeSpan.TotalSeconds;
                case Intervals.Minutes:
                    return (long)timeSpan.TotalMinutes;
                case Intervals.Hours:
                    return (long)timeSpan.TotalHours;
                case Intervals.Days:
                    return (long)timeSpan.Days;
                case Intervals.Weeks:
                    return (long)(timeSpan.Days / 7);
                case Intervals.Months:
                    return (long)(timeSpan.Days / 30);
                case Intervals.Quarters:
                    return (long)((timeSpan.Days / 30) / 3);
                case Intervals.Years:
                    return (long)(timeSpan.Days / 365);
            }

            return 0;
        }

        /// <summary>
        /// Obtiene los nombres de los dias segun el nombre de la referencia cultural del sistema de computo.
        /// </summary>
        /// <returns>Un array <see cref="System.String"/> con los nombres de los dias segun el nombre de la
        /// cultura dada.</returns>
        public static string[] GetDays()
        {
            return GetDays(Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Obtiene los nombres de los dias segun el nombre de la referencia cultural dada.
        /// </summary>
        /// <param name="cultureName">Referencia cultural de la cual se requieren los nombres de los dias.</param>
        /// <returns>Un array <see cref="System.String"/> con los nombres de los dias segun el nombre de
        /// la referencia cultural dada.</returns>
        public static string[] GetDays(string cultureName)
        {
            return GetDays(new CultureInfo(cultureName));
        }

        /// <summary>
        /// Obtiene los nombres de los dias segun el nombre de la referencia cultural dada.
        /// </summary>
        /// <param name="cultureInfo">Referencia cultural de la cual se requieren los nombres de los dias.</param>
        /// <returns>Un array <see cref="System.String"/> con los nombres de los dias segun el nombre de
        /// la referencia cultural dada.</returns>
        public static string[] GetDays(CultureInfo cultureInfo)
        {
            return cultureInfo.DateTimeFormat.DayNames;
        }

        /// <summary>
        /// Obtiene el nombre de un dia especifico segun la referencia cultural del sistema computo.
        /// </summary>
        /// <param name="day">Numero de dia de que se requiere el nombre del dia.</param>
        /// <returns>Un <see cref="System.String"/> representando el dia solicitado.</returns>
        public static string GetDayName(int day)
        {
            return GetDayName(Thread.CurrentThread.CurrentCulture, day);
        }

        /// <summary>
        /// Obtiene el nombre de un dia especifico segun la referencia cultural dada.
        /// </summary>
        /// <param name="cultureInfo">Referencia cultural a implementar.</param>
        /// <param name="day">Numero de dia de que se requiere el nombre del dia.</param>
        /// <returns>Un <see cref="System.String"/> representando el dia solicitado.</returns>
        public static string GetDayName(CultureInfo cultureInfo, int day)
        {
            return GetDays(cultureInfo)[day];
        }

        /// <summary>
        /// Obtiene el nombre de un dia especifico segun la referencia cultural dada.
        /// </summary>
        /// <param name="cultureName">Nombre de la referencia cultural a consultar.</param>
        /// <param name="day">Numero de dia de que se requiere el nombre del dia.</param>
        /// <returns>Un <see cref="System.String"/> representando el dia solicitado.</returns>
        public static string GetDayName(string cultureName, int day)
        {
            return GetDays(new CultureInfo(cultureName))[day];
        }

        /// <summary>
        /// Para el mes especificado, devuelve su nombre completo de un mes según la referencia
        /// cultural asociada al objeto System.Globalization.DateTimeFormatInfo actual.
        /// </summary>
        /// <param name="month">Un valor <see cref="System.Int32"/> de 1 a 12 que representa
        /// el nombre del mes que se va a recuperar.</param>
        /// <returns>El nombre completo específico de la referencia cultural del mes especificado
        /// representado por <paramref name="month"/>.</returns>
        public static string GetMonthName(int month)
        {
            return GetMonthName(Thread.CurrentThread.CurrentCulture, month);
        }

        /// <summary>
        /// Para el mes especificado, devuelve su nombre completo de un mes según la referencia
        /// cultural dada.
        /// </summary>
        /// <param name="cultureInfo">Referencia cultural a implementar.</param>
        /// <param name="month">Un valor <see cref="System.Int32"/> de 1 a 12 que representa
        /// el nombre del mes que se va a recuperar.</param>
        /// <returns>El nombre completo específico de la referencia cultural del mes especificado
        /// representado por <paramref name="month"/>.</returns>
        public static string GetMonthName(CultureInfo cultureInfo, int month)
        {
            try
            {
                if (month < 1 || month > 12)
                    throw new Exception("El mes específicado no es válido (valores válidos: 1 - 12).");

                return cultureInfo.DateTimeFormat.GetMonthName(month);
            }
            catch (Exception) { }

            return string.Empty;
        }

        /// <summary>
        /// Para el mes especificado, devuelve su nombre completo de un mes según la referencia
        /// cultural dada.
        /// </summary>
        /// <param name="cultureName">Nombre de la referencia cultural a implementar.</param>
        /// <param name="month">Un valor <see cref="System.Int32"/> de 1 a 12 que representa
        /// el nombre del mes que se va a recuperar.</param>
        /// <returns>El nombre completo específico de la referencia cultural del mes especificado
        /// representado por <paramref name="month"/>.</returns>
        public static string GetMonthName(string cultureName, int month)
        {
            return GetMonthName(new CultureInfo(cultureName), month);
        }

        /// <summary>
        /// Devuelve los nombres completos de los meses según la referencia
        /// cultural asociada al objeto System.Globalization.DateTimeFormatInfo actual.
        /// </summary>
        /// <returns>El nombre completo específico de la referencia cultural del mes especificado
        /// representado por <paramref name="month"/>.</returns>
        public static List<string> GetMonths()
        {
            return GetMonths(Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Devuelve los nombres completos de los meses según la referencia
        /// cultural asociada al objeto System.Globalization.DateTimeFormatInfo dada.
        /// </summary>
        /// <param name="cultureInfo">Referencia cultural a implementar.</param>
        /// <returns>Un array <see cref="System.String"/> con los nombres de los meses 
        /// del año segun el nombre de la referencia cultural dada.</returns>
        public static List<string> GetMonths(CultureInfo cultureInfo)
        {
            List<string> months = new List<string>();
            foreach (string month in cultureInfo.DateTimeFormat.MonthNames)
                months.Add(month.ToUpper());

            return months;
        }

        /// <summary>
        /// Devuelve los nombres completos de los meses según el nombre de la referencia
        /// cultural asociada al objeto System.Globalization.DateTimeFormatInfo dada.
        /// </summary>
        /// <param name="cultureName">Referencia cultural a implementar.</param>
        /// <returns>Un array <see cref="System.String"/> con los nombres de los meses 
        /// del año segun el nombre de la referencia cultural dada.</returns>
        public static string[] GetMonths(string cultureName)
        {
            return new CultureInfo(cultureName).DateTimeFormat.MonthNames;
        }
    }
}