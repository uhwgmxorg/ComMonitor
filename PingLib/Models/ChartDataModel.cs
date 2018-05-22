using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PingLib.Models
{
    public class ChartDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime dateTime;
        public DateTime DateTime
        {
            get { return dateTime; }
            set { SetField(ref dateTime, value, nameof(DateTime)); }
        }
        private double value;
        public double Value
        {
            get { return value; }
            set { SetField(ref this.value, value, nameof(ChartDataModel.Value)); }
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}
