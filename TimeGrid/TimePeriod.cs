using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Runtime.CompilerServices;
using ReactiveUI;
using ReactiveUI.Xaml;


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
            Hours = new ReactiveCollection<HourlyTimePeriod>();
            for (var day = 0; day < DaysPerWeek; day++)
            {
                for (var hour = 0; hour < HoursPerDay; hour++)
                {
                    Hours.Add(new HourlyTimePeriod(day, hour));
                }
            }
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

        private ReactiveCommand _reset;
        public ICommand Reset
        {
            get
            {
                if (_reset != null) return _reset;
                _reset = new ReactiveCommand(); // Can execute
                _reset.Subscribe(x => ResetTimeConfiguration());
                return _reset;
            }
        }


        public void ResetTimeConfiguration(bool[,] timegrid = null)
        {
            for (var day = 0; day < DaysPerWeek; day++)
            {
                for (var hour = 0; hour < HoursPerDay; hour++)
                {
                    var hourIndex = day * HoursPerDay + hour;
                    if (timegrid != null && timegrid.GetLongLength(0) > day)
                    {
                        Hours[hourIndex].Enabled = timegrid.GetLongLength(1) > hour && timegrid[day, hour];
                    }
                    else
                    {
                        Hours[hourIndex].Enabled = false;
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
