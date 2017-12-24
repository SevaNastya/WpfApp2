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
    public struct Events
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
    class ViewModel: INotifyPropertyChanged
    {
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
        private Events Clicked;
        public ViewModel()
        {
            Insert.Seva = InsertF;
            Delete.Seva = DeleteF;
            Change.Seva = ChangeF;
            Month.Seva = MonthF;
            Year.Seva = YearF;
            Eventlist = new List<Events>(MakeRequest("select [key], Event, [Date] from Calendar"));
        }

        private void YearF(object obj)
        {
            Eventlist = new List<Events>(MakeRequest("select [key], Event, [Date] from Calendar"));
        }

        private void MonthF(object obj)
        {
            List<Events> all = new List<Events>(MakeRequest("select [key], Event, [Date] from Calendar"));
            List<Events> hold = new List<Events>();
            foreach (Events e in all)
                if (DateTime.Now.Month == e.month)
                    hold.Add(e);
            Eventlist = new List<Events>(hold);
        }

        private void ChangeF(object obj)
        {
            throw new NotImplementedException();
        }

        private void DeleteF(object obj)
        {
            using (SqlConnection db = new SqlConnection()) //connnectionstring
            {
                SqlCommand cmd = new SqlCommand($"delete from Table where key={Clicked.key}");
                try
                {
                    db.Open();
                    cmd.ExecuteNonQuery();
                    db.Close();
                }
                catch
                {
                    MessageBox.Show("Db is inaccessible");
                }
            }
        }

        private void InsertF(object obj)
        {
            throw new NotImplementedException();
        }

        private List<Events> MakeRequest(string req)
        {
            List<Events> hold = new List<Events>();
            SqlConnectionStringBuilder constr = new SqlConnectionStringBuilder();
            constr.IntegratedSecurity = true;
            constr.DataSource = Environment.MachineName + "\\SQLEXPRESS";
            constr.InitialCatalog = "Calendar";
            using (SqlConnection db = new SqlConnection(constr.ConnectionString))
            {
                SqlCommand query = new SqlCommand(req, db);
                SqlDataReader reader;
                try
                {
                    db.Open();
                    reader = query.ExecuteReader();
                    SqlDataAdapter adapter = new SqlDataAdapter(query);
                    DataTable table = new DataTable();
                    db.Close();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                        hold.Add(new Events((string)row.ItemArray[1], (string)row.ItemArray[2], (long)row.ItemArray[0]));
                    hold = hold.OrderBy((Events h) => { return h.days; }).ToList();
                }
                catch
                {
                    MessageBox.Show("Local database is inaccessible");
                }
            }
            return hold;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string NewProp)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(NewProp));
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
