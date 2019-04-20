using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Npgsql;

namespace TeamTeamwork_Yelp_App
{
    public class UserFriends
    {
        public string userid { get; set; }
        public string friendid { get; set; }

        public string friendName { get; set; }
        public string friendStars { get; set; }
        public string friendYelpingSince { get; set; }
    }

    class UserInformation
    {
        private string buildConnString()
        {
            return "Host=localhost; Username=postgres; Password=mypassword; Database=yelpdb";
        }

        public void searchUser(TextBox Uname, ListBox userIdList)
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select users.userid from users where users.name = '" + Uname.Text.ToString() + "';";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userIdList.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        public void addFriendColumns(DataGrid grid)
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Name";
            col1.Binding = new Binding("friendName");
            grid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Rating";
            col2.Binding = new Binding("friendStars");
            grid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Yelping Since";
            col3.Binding = new Binding("friendYelpingSince");
            grid.Columns.Add(col3);
        }
    }
}
