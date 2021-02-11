using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCompanyHelper.Models
{

    public class Time
    {
        private int _seconds;
        private int _minutes;
        private int _hours;
        private int _days;
        public int Seconds
        {
            get => _seconds;
            set
            {
                _seconds = value;
                while (_seconds >= 60)
                {
                    Minutes += _seconds / 60;
                    _seconds = _seconds - _seconds / 60 * 60;
                }
                UpdateStr();
            }
        }
        public int Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value;
                while (_minutes >= 60)
                {
                    Hours += _minutes / 60;
                    _minutes = _minutes - _minutes / 60 * 60;
                }
                UpdateStr();
            }
        }
        public int Hours
        {
            get => _hours;
            set
            {
                _hours = value;
                while (_canBeMoreThanDay && _hours >= 24)
                {
                    Days += _hours / 60;
                    _hours = _hours - _hours / 60 * 60;
                }
                UpdateStr();
            }
        }
        public int Days
        {
            get => _days;
            set
            {
                _days = value;
                UpdateStr();
            }
        }
        public string StrTime;
        private bool _canBeMoreThanDay;
        public Time(bool canBeMoreThanDay)
        {
            _canBeMoreThanDay = canBeMoreThanDay;
            Seconds = 0;
            Minutes = 0;
            Hours = 0;
            Days = 0;
            StrTime = canBeMoreThanDay ? "0д 00:00:00" : "00:00:00";
        }
        public Time(int timeInSeconds, bool canBeMoreThanDay)
        {
            _canBeMoreThanDay = canBeMoreThanDay;
            Minutes = 0;
            Hours = 0;
            Days = 0;
            Seconds = timeInSeconds;
        }
        private void UpdateStr()
        {
            if (_canBeMoreThanDay)
            {
                StrTime = Days.ToString() + "д " + Hours.ToString("D" + 2) + ":" + Minutes.ToString("D" + 2) + ":" + Seconds.ToString("D" + 2);
            }
            else
            {
                StrTime = Hours.ToString("D" + 2) + ":" + Minutes.ToString("D" + 2) + ":" + Seconds.ToString("D" + 2);
            }
        }
        public void Reset()
        {
            Seconds = 0;
            Minutes = 0;
            Hours = 0;
            Days = 0;
        }
        public void SetTime(int seconds)
        {
            Minutes = 0;
            Hours = 0;
            Days = 0;
            Seconds = seconds;
        }
        public int ToInt32()
        {
            return Days * 86400 + Hours * 3600 + Minutes * 60 + Seconds;
        }
        public static Time operator ++(Time a)
        {
            a.Seconds += 1;
            return a;
        }
    }
}
