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
    // Class used for filling in the friend reviews data grid
    public class SelectedBusinessFriendReview
    {
        public string reviewid { get; set; }
        public string userid { get; set; }
        public string businessid { get; set; }

        public string userName { get; set; }
        public string date { get; set; }
        public string text { get; set; }
    }

    class SelectedBusiness
    {
        private Business business;

        private string buildConnString()
        {
            return "Host=localhost; Username=postgres; Password=mypassword; Database=yelpdb";
        }

        public SelectedBusiness(Business selectedBusiness)
        {
            business = selectedBusiness;
        }

        public void addFavorite(string userID)
        {
            if (business == null)
            {
                return;
            }

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO favoritebusiness(businessid, userid) SELECT '" + business.businessid + "', '" + userID + "' WHERE NOT EXISTS( SELECT 1 FROM favoritebusiness WHERE businessid = '" + business.businessid + "' AND userid = '" + userID + "');";
                    //cmd.CommandText = "INSERT INTO public.favoritebusiness(businessid, userid) VALUES ('" + business.businessid + "', '" + userID + "');";
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            MessageBox.Show(business.name + " is a favorite!", "Yelp");
        }

        public void setFriendsReviews(string userID, DataGrid reviewGrid)
        {
            if (userID == null || userID == "" || business == null)
            {
                return;
            }
            reviewGrid.Items.Clear();

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT users.name, reviews.reviewdate, reviews.text FROM users, friends, businesses, writesfor, reviews WHERE businesses.businessid = writesfor.businessid AND writesfor.reviewid = reviews.reviewid AND friends.friendid = writesfor.userid AND friends.friendid = users.userid AND friends.userid = '" + userID + "' AND businesses.businessid = '" + business.businessid + "';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviewGrid.Items.Add(new SelectedBusinessFriendReview()
                            {
                                userName = reader.GetString(0),
                                date = reader.GetDate(1).ToString(),
                                text = reader.GetString(2),
                            });
                        }
                    }
                }
                conn.Close();
            }
        }

        public void setBusinessInfo(Label name, Label address, TextBox attributes, ListBox hours, ListBox categories)
        {
            // Set the easy stuff that we have stored in business (name and address)
            name.Content = business.name;
            address.Content = business.address + " " + business.city + ", " + business.state;

            // Now query to get attributes, hours, and categories (use business.businessid)
            setBusinessCategories(categories);
            setBusinessHours(hours);
            setAttributes(attributes);            
        }

        private void setBusinessCategories(ListBox categories)
        {
            if (business == null)
            {
                return;
            }

            categories.Items.Clear();

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT name FROM categories WHERE businessid = '" + business.businessid +"' ORDER BY name;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        private void setBusinessHours(ListBox hours)
        {
            if (business == null)
            {
                return;
            }

            // clear items if there are any
            hours.Items.Clear();

            // Get the current dates weekday
            string todaysDay = DateTime.Now.ToString("dddd");

            hours.Items.Add("Today ("+ todaysDay +")");

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT opentime, closetime FROM hours WHERE businessid = '" + business.businessid + "' AND weekday = '" + todaysDay +"';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            hours.Items.Add("Opens: " + reader.GetTimeSpan(0).ToString());
                            hours.Items.Add("Closes: " + reader.GetTimeSpan(1).ToString());
                        }
                    }
                }
                conn.Close();
            }
        }

        private void setAttributes(TextBox attributes)
        {
            if (business == null)
            {
                return;
            }

            string attributesString = "";

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT name, value FROM attributes WHERE businessid = '" + business.businessid + "' ORDER BY name;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attributesString = configureAttributes(attributesString, reader.GetString(0), reader.GetString(1));
                        }

                        // Cut ', ' from string
                        attributesString.Remove(attributesString.Length - 2);
                        // Set content
                        attributes.Text = attributesString;
                    }
                }
                conn.Close();
            }
        }

        private string configureAttributes(string attr, string name, string val)
        {
            if (val != "true" && val != "false")
            {
                // Custom attributes
                attr += name + "(" + val + "), ";
            }
            else
            {
                // attribute is true/false, now only find true ones
                if (val == "true")
                {
                    attr += name + ", ";
                }
            }

            return attr;
        }

        public void checkin()
        {
            if (business == null)
            {
                return;
            }

            string currDay = DateTime.Now.DayOfWeek.ToString();
            string currMilTime = DateTime.Now.ToString("HH") + ":00:00";
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =  "INSERT INTO checkin (businessid, checkinday, checkintime, checkincount) VALUES ('" + business.businessid + "', '" + currDay + "', '" + currMilTime + "', 1) ON CONFLICT (businessid, checkinday, checkintime) DO UPDATE SET checkincount = checkin.checkincount + 1;";
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            MessageBox.Show("Checked into " + business.name + "!", "Yelp");
        }

    }
}
