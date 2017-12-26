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
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

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
            if (evdate.Split('.').Length > 2) evdate = $"{evdate.Split('.')[0]}.{evdate.Split('.')[1]}";
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

    class MainWindowViewModel : INotifyPropertyChanged
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
        private List<Events> vklist_;
        public List<Events> VKlist
        {
            get { return vklist_; }
            set
            {
                vklist_ = value;
            }
        }
        private string _userid;
        public string UserID
        {

            get { return _userid; }
            set
            {
                _userid = value;
                OnPropertyChanged("UserID");
            }
        }

        public SevaCommand Insert { get; set; } = new SevaCommand();
        public SevaCommand Delete { get; set; } = new SevaCommand();
        public SevaCommand Change { get; set; } = new SevaCommand();
        public SevaCommand Month { get; set; } = new SevaCommand();
        public SevaCommand Year { get; set; } = new SevaCommand();
        public SevaCommand ListClick { get; set; } = new SevaCommand();
        public SevaCommand LoadFriends { get; set; } = new SevaCommand();

        private Events Clicked;
        private bool All = true;

        public MainWindowViewModel()
        {
            LoadFriends.Seva = DoLoad;
            Insert.Seva = InsertF;
            Delete.Seva = DeleteF;
            Change.Seva = ChangeF;
            Month.Seva = MonthF;
            Year.Seva = YearF;
            ListClick.Seva = ListClickF;
            Eventlist = new List<Events>(SQLQueries.MakeRequestRead("select [key], Event, [Date] from Calendar"));
        }

        private void DoLoad(object candy)

        {
            if (candy == null || !(candy is TextBox)) return;
            string input = (string)UserID;
            Regex r = new Regex("^[0-9]*$");
            if (string.IsNullOrEmpty(UserID)) return;
            if (!(r.IsMatch(input)))
            { MessageBox.Show("Неправильный ввод"); return; }
            string Friends;
            try
            {
                WebRequest request = WebRequest.Create($"https://api.vk.com/method/friends.get?user_id={UserID}&fields=bdate&name_case=nom&v=5.69");
                WebResponse response = request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    Friends = reader.ReadToEnd();
            }
            catch
            {
                MessageBox.Show("Ошибка подключения");
                return;
            }
            if (Friends == null)
            {
                MessageBox.Show("Друзья не найдены");
                return;
            }
            VKfriends cherry = null;
            try
            {
                cherry = JsonConvert.DeserializeObject<VKfriends>(Friends);
            }
            catch
            {
                MessageBox.Show("Десериализация не удалась");
            }
            if (cherry == null)
            {
                MessageBox.Show("Что-то пошло не так");
                return;
            }
            if (cherry.error != null)
            {
                MessageBox.Show(cherry.error.error_msg);
                return;
            }
            List<Events> hold = new List<Events>();
            foreach (Item i in cherry.response.items)
            {
                if (i.bdate == null) continue;
                hold.Add(new Events($"ДР {i.first_name} {i.last_name}", i.bdate, -1));
            }
            hold = hold.OrderBy((Events h) => { return h.days; }).ToList();
            VKlist = hold;
        }

        private void ListClickF(object obj)
        {
            if (obj == null || !(obj is Events)) return;
            Clicked = (Events)obj;
        }

        private void YearF(object obj)
        {
            Eventlist = new List<Events>(SQLQueries.MakeRequestRead("select [key], Event, [Date] from Calendar"));
            foreach (Events i in VKlist)
                Eventlist.Add(i);
            Eventlist = Eventlist.OrderBy((Events h) => { return h.days; }).ToList();
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
            foreach (Events i in VKlist)
            {
                if (DateTime.Now.Month == int.Parse(i.Date.Split('.')[1]))
                    Eventlist.Add(i);
            }
            Eventlist = Eventlist.OrderBy((Events h) => { return h.days; }).ToList();
            All = false;
        }

        private void DeleteF(object obj)
        {
            if (Clicked == null) return;
            SQLQueries.MakeRequestWrite($"delete from Calendar where [key] = {Clicked.key}");
            if (All) YearF(null);
            else MonthF(null);
        }

        private void ChangeF(object obj)
        {
            if (Clicked == null) return;
            Window w = new AddChangeWindow(Clicked);
            w.Show();
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
