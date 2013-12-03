using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Runtime.CompilerServices;
using ReactiveUI;

namespace TimeGrid
{
    public class HourlyTimePeriod : ReactiveObject
    {
        public HourlyTimePeriod(int dayOfWeek, int hourOfDay, bool defaultValue = false)
        {
            DayOfWeek = dayOfWeek;
            HourOfDay = hourOfDay;
            Enabled = defaultValue;
        }
        public int DayOfWeek { get; set; }
        public int HourOfDay { get; set; }
        #region Property Enabled
        private bool _enabled = default(bool);
        public bool Enabled
        {
            get { return _enabled; }
            set { this.RaiseAndSetIfChanged(ref _enabled, value); }
        }
        #endregion
    }

    public class WeeklyTimePeriod : ReactiveObject
    {
        private const int DaysPerWeek = 7;
        private const int HoursPerDay = 24;
        public WeeklyTimePeriod(bool[,] timegrid = null)
        {
            ResetTimeConfiguration(timegrid);
        }

        #region Property Hours
        private ReactiveCollection<HourlyTimePeriod> _hours = default(ReactiveCollection<HourlyTimePeriod>);
        public ReactiveCollection<HourlyTimePeriod> Hours
        {
            get { return _hours; }
            set { this.RaiseAndSetIfChanged(ref _hours, value); }
        }
        #endregion

        public bool[,] ToArray()
        {
            var result = new bool[DaysPerWeek,HoursPerDay];
            
            for (var day = 0; day < DaysPerWeek; day++)
            {
                for (var hour = 0; hour < HoursPerDay; hour++)
                {
                    result[day, hour] = Hours[(day * HoursPerDay) + hour].Enabled;
                }
            }
            return result;
        }

        public void ResetTimeConfiguration(bool[,] timegrid = null)
        {

            Hours = new ReactiveCollection<HourlyTimePeriod>();
            for (var day = 0; day < 7; day++)
            {
                for (var hour = 0; hour < 24; hour++)
                {
                    if (timegrid != null && timegrid.GetLongLength(0) > day)
                    {
                        Hours.Add((new HourlyTimePeriod(day, hour, timegrid.GetLongLength(1) > hour && timegrid[day, hour])));
                    }
                    else
                    {
                        Hours.Add(day < 5 ? new HourlyTimePeriod(day, hour, hour > 8 && hour < 19) : new HourlyTimePeriod(day,hour));
                    }
                }
            }
        }

        HourlyTimePeriod[] Day(int index)
        {
            var result = new HourlyTimePeriod[HoursPerDay];
            for (var i = 0; i < HoursPerDay; i++)
            {
                result[i] = Hours[(index * HoursPerDay) + i];
            }
            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Monday    [{0}]{1}", FormatDay(Day(0)), Environment.NewLine);
            sb.AppendFormat("Tuesday   [{0}]{1}", FormatDay(Day(1)), Environment.NewLine);
            sb.AppendFormat("Wednesday [{0}]{1}", FormatDay(Day(2)), Environment.NewLine);
            sb.AppendFormat("Thursday  [{0}]{1}", FormatDay(Day(3)), Environment.NewLine);
            sb.AppendFormat("Friday    [{0}]{1}", FormatDay(Day(4)), Environment.NewLine);
            sb.AppendFormat("Saturday  [{0}]{1}", FormatDay(Day(5)), Environment.NewLine);
            sb.AppendFormat("Sunday    [{0}]{1}", FormatDay(Day(6)), Environment.NewLine);
            return sb.ToString();
        }

        static string FormatDay(HourlyTimePeriod[] hours )
        {
            var sb = new StringBuilder();
            for(var i = 0; i < hours.Length; i++)
            {
                var displayHour = hours[i].HourOfDay;
                if (hours[i].Enabled)
                {
                    sb.AppendFormat("{0}{1}", displayHour, (i + 1 < hours.Length) ? ", " : "");
                }
            }
            return sb.ToString();
        }
    }
}
