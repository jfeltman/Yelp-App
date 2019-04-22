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
    public class UserFriend
    {
        public string userid { get; set; }
        public string friendid { get; set; }

        public string friendName { get; set; }
        public string friendStars { get; set; }
        public string friendYelpingSince { get; set; }
    }

    public class friendReview
    {
        public string userid { get; set; }
        public string friendid { get; set; }

        public string friendRName { get; set; }
        public string reviewBusiness { get; set; }
        public string reviewBusinessCity { get; set; }
        public string reviewText { get; set; }
    }

    public class UserFavBusiness
    {
        public string userid { get; set; }
        public string businessid { get; set; }

        public string busName { get; set; }
        public string busStars { get; set; }
        public string busCity { get; set; }
        public string busZip { get; set; }
        public string busAddress { get; set; }
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
            if (userId == null || userId == "")
            {
                return;
            }

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
            if (userId == null || userId == "")
            {
                return;
            }

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

        public void addBusinessColumns(DataGrid grid)
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Business";
            col1.Binding = new Binding("busName");
            grid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Stars";
            col2.Binding = new Binding("busStars");
            grid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("busCity");
            grid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Zip";
            col4.Binding = new Binding("busZip");
            grid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "Address";
            col5.Binding = new Binding("busAddress");
            grid.Columns.Add(col5);
        }

        public void addreviewingColumns(DataGrid grid)
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Name";
            col1.Binding = new Binding("friendRName");
            grid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Business";
            col2.Binding = new Binding("reviewBusiness");
            grid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("reviewBusinessCity");
            grid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Text";
            col4.Binding = new Binding("reviewText");
            grid.Columns.Add(col4);
        }

        public void setFavoriteBusinesses(string userID, DataGrid usersBusinessGrid)
        {
            if (userID == null || userID == "")
            {
                return;
            }
            usersBusinessGrid.Items.Clear();

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select businesses.name, businesses.starrating, businesses.city, businesses.zip, businesses.street, businesses.businessid from favoritebusiness, businesses where businesses.businessid = favoritebusiness.businessid and favoritebusiness.userid = '" + userID + "';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usersBusinessGrid.Items.Add(new UserFavBusiness()
                            {
                                busName = reader.GetString(0),
                                busStars = reader.GetDouble(1).ToString(),
                                busCity = reader.GetString(2),
                                busZip = reader.GetInt32(3).ToString(),
                                busAddress = reader.GetString(4),
                                businessid = reader.GetString(5),
                            });
                        }
                    }
                }
                conn.Close();
            }
        }

        public void setFriends(string userID, DataGrid usersFriendsGrid)
        {
            if (userID == null || userID == "")
            {
                return;
            }
            usersFriendsGrid.Items.Clear();

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select users.name, users.averagestars, users.yelpingsince from users, friends where friends.friendid = users.userid and friends.userid = '" + userID + "'; ";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usersFriendsGrid.Items.Add(new UserFriend()
                            {
                                friendName = reader.GetString(0),
                                friendStars = reader.GetDouble(1).ToString(),
                                friendYelpingSince = reader.GetDate(2).ToString(),
                            });
                        }
                    }
                }
                conn.Close();
            }
        }

        public void setfriendsreviews(string userID, DataGrid friendReviewsGrid)
        {
            if (userID == null || userID == "")
            {
                return;
            }
            friendReviewsGrid.Items.Clear();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT ON(users.userid) users.name, businesses.name, businesses.city," +
                        " reviews.text FROM users, friends, reviews, writesfor, businesses " +
                        "WHERE friends.userid = '"+ userID +"' AND friends.friendid = users.userid AND " +
                        "friends.friendid = writesfor.userid AND writesfor.reviewid = reviews.reviewid AND " +
                        "writesfor.businessid = businesses.businessid ORDER by users.userid, reviews.reviewdate DESC;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            friendReviewsGrid.Items.Add(new friendReview()
                            {
                                friendRName = reader.GetString(0),
                                reviewBusiness = reader.GetString(1),
                                reviewBusinessCity = reader.GetString(2),
                                reviewText = reader.GetString(3)
                                
                            });
                        }
                    }
                }
                conn.Close();
            }
        }

        public void removeFavorite(string userID, string bid)
        {
            if (userID == null || userID == "" || bid == "" || bid == null)
            {
                return;
            }

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from favoritebusiness where businessid = '" + bid + "' and userid = '" + userID + "';";
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
    }
}
