﻿using System;
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

        public void populateUserInfo(string userId, TextBox name, TextBox stars, TextBox fans, TextBox yps, TextBox funny, TextBox cool, TextBox useful, TextBox latitude, TextBox longitude)
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT users.name, users.averagestars, users.fanscount, users.yelpingsince,users.votesfunny, users.votescool, users.votesuseful, users.lat, users.long FROM users WHERE users.userid = '" + userId + "'; ";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            name.Text = reader.GetString(0);
                            stars.Text = reader.GetDouble(1).ToString();
                            fans.Text = reader.GetInt16(2).ToString();
                            yps.Text = reader.GetDate(3).ToString();
                            funny.Text = reader.GetInt16(4).ToString();
                            cool.Text = reader.GetInt16(5).ToString();
                            useful.Text = reader.GetInt16(6).ToString();

                            if (reader.IsDBNull(7) == false)
                            {
                                latitude.Text = reader.GetDouble(7).ToString();
                            }
                            else
                            {
                                latitude.Text = "";
                            }
                            if (reader.IsDBNull(8) == false)
                            {
                                longitude.Text = reader.GetDouble(8).ToString();
                            }
                            else
                            {
                                longitude.Text = "";
                            }

                        }
                    }
                }
                conn.Close();
        }
    }

        public void updateLocation(string userId, double latitude, double longitude)
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE users SET lat = " + latitude + ", long = " + longitude + " WHERE users.userid = '" + userId + "'; ";
                    cmd.ExecuteReader();
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
