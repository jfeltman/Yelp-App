using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;

namespace TeamTeamwork_Yelp_App
{
    /// <summary>
    /// Interaction logic for CheckinGraph.xaml
    /// </summary>
    public partial class CheckinGraph : Window
    {
        public CheckinGraph(string businessid)
        {
            InitializeComponent();
            createChart(businessid);
        }

        private string buildConnString()
        {
            return "Host=localhost; Username=postgres; Password=mypassword; Database=yelpdb";
        }

        private void createChart(string businessid)
        {
            List<KeyValuePair<string, int>> myChartData = new List<KeyValuePair<string, int>>();

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT checkinday, SUM(checkincount) FROM checkin WHERE businessid = '" + businessid + "' GROUP BY checkinday";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            myChartData.Add(new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1)));
                        }
                    }
                }
                conn.Close();
            }

            checkinChart.DataContext = myChartData;
        }
    }
}
