using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    class AddChangeViewModel : INotifyPropertyChanged
    {
        private Events a;
        private string _name;
        private string _date;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChange("Name");
            }
        }
        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChange("Date");
            }
        }
        private bool _canedit;
        public bool CanEdit
        {
            get { return _canedit; }
            set
            {
                _canedit = value;
                OnPropertyChange("CanEdit");
            }
        }
        public SevaCommand Save { get; set; } = new SevaCommand();
        public AddChangeViewModel()
        {
            Save.Seva = AddF;
            CanEdit = true;
        }

        private void AddF(object obj)
        {
            Regex r = new Regex("^[0-9]*$");
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Date)) return;
            if (Date.Split('.').Length != 2 || !r.IsMatch(Date.Split('.')[0]) || !r.IsMatch(Date.Split('.')[1])|| int.Parse(Date.Split('.')[0]) > 31 || int.Parse(Date.Split('.')[1]) > 12)
            { MessageBox.Show("Неправильный ввод"); return; }
            SQLQueries.MakeRequestWrite($"insert into Calendar (Event, [Date]) values (\'{Name}\', \'{Date}\')");
            Name = string.Empty;
            Date = string.Empty;
        }

        public AddChangeViewModel(Events _a)
        {
            a = _a;
            Save.Seva = EditF;
            Name = a.EventTitle;
            Date = a.Date;
            CanEdit = true;
        }

        private void EditF(object obj)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Date) || a == null) return;
            SQLQueries.MakeRequestWrite($"update Calendar set Event = \'{Name}\', [Date] = \'{Date}\' where [key]={a.key}");
            a = null;
            CanEdit = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string a)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(a));
        }
    }
}
