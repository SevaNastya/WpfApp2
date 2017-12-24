using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace WpfApp2
{
    public class Events
    {
        public string EventTitle { get; set; }
        public string Date { get; set; }
        public long key;
        public int days;
        public int month;
        public Events(string evtitle, string evdate, long k)
        {
            EventTitle = evtitle;
            Date = evdate;
            key = k;
            try
            {
                days = int.Parse(evdate.Split('.')[0]) + int.Parse(evdate.Split('.')[1]) * 30;
                month = int.Parse(evdate.Split('.')[1]);
            }
            catch
            {
                days = 0;
                month = 0;
            }
        }
    }
    class MainWindowViewModel: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string NewProp)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(NewProp));
        }

        private List<Events> event_;
        public List<Events> Eventlist
        {
            get { return event_; }
            set
            {
                event_ = value;
                OnPropertyChanged("Eventlist");
            }
        }
        public SevaCommand Insert { get; set; } = new SevaCommand();
        public SevaCommand Delete { get; set; } = new SevaCommand();
        public SevaCommand Change { get; set; } = new SevaCommand();
        public SevaCommand Month { get; set; } = new SevaCommand();
        public SevaCommand Year { get; set; } = new SevaCommand();
        public SevaCommand ListClick { get; set; } = new SevaCommand();
        private Events Clicked;
        private bool All = true;
        public MainWindowViewModel()
        {
            Insert.Seva = InsertF;
            Delete.Seva = DeleteF;
            Change.Seva = ChangeF;
            Month.Seva = MonthF;
            Year.Seva = YearF;
            ListClick.Seva = ListClickF;
            Eventlist = new List<Events>(SQLQueries.MakeRequestRead("select [key], Event, [Date] from Calendar"));
        }

        private void ListClickF(object obj)
        {
            if (obj == null || !(obj is Events)) return;
            Clicked = (Events)obj;
        }

        private void YearF(object obj)
        {
            Eventlist = new List<Events>(SQLQueries.MakeRequestRead("select [key], Event, [Date] from Calendar"));
            All = true;
        }

        private void MonthF(object obj)
        {
            List<Events> all = new List<Events>(SQLQueries.MakeRequestRead("select [key], Event, [Date] from Calendar"));
            List<Events> hold = new List<Events>();
            foreach (Events e in all)
                if (DateTime.Now.Month == e.month)
                    hold.Add(e);
            Eventlist = new List<Events>(hold);
            All = false;
        }

        private void ChangeF(object obj)
        {
            if (Clicked == null) return;
            Window w = new AddChangeWindow(Clicked);
            w.Show();
        }

        private void DeleteF(object obj)
        {
            if (Clicked == null) return;
            SQLQueries.MakeRequestWrite($"delete from Calendar where [key] = {Clicked.key}");
            if (All) YearF(null);
            else MonthF(null);
        }

        private void InsertF(object obj)
        {
            Window w = new AddChangeWindow();
            w.Show();
        }

        
        
    }
    public class SevaCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Predicate<object> CanExecuteFunc { get; set; }
        public Action<object> Seva { get; set; }
        public bool CanExecute(object parameter) { return true; }
        public void Execute(object parameter)
        {
            Seva(parameter);
        }
    }
}
