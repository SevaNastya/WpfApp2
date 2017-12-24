using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    static class SQLQueries
    {
        static public List<Events> MakeRequestRead(string req)
        {
            List<Events> hold = new List<Events>();
            using (SqlConnection db = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Calendar;
Integrated Security=True"))
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
        static public void MakeRequestWrite(string req)
        {
            using (SqlConnection db = new SqlConnection(Properties.Settings.Default.SQL))
            {
                SqlCommand query = new SqlCommand(req, db);
                try
                {
                    db.Open();
                    query.ExecuteNonQuery();
                    db.Close();
                }
                catch
                {
                    MessageBox.Show("Local database is inaccessible");
                }
            }
        }
    }
}
