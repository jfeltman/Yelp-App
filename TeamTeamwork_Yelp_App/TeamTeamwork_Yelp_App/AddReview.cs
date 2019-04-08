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
    class AddReview
    {
        private string buildConnString()
        {
            return "Host=localhost; Username=postgres; Password=mypassword; Database=yelpdb";
        }

        // Populate ratings box
        public void addRatings(ComboBox ratings)
        {
            ratings.Items.Add(5);
            ratings.Items.Add(4);
            ratings.Items.Add(3);
            ratings.Items.Add(2);
            ratings.Items.Add(1);
        }

        // Use a temp user id for now since we arent doing user login till next milestone
        private string tempUserId = "quGj2N9QclyEaZDxBjNXOw"; // some guy named steve

        private static Random random = new Random();

        // Create a string of random characters from a provided length
        // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void addReview(TextBox text, ComboBox rating, string businessid)
        {
            if (text.Text != null && rating.SelectedItem != null)
            {
                // do 23 characters to make sure its impossible for this id to be the same as an already existing id in the database
                string newReviewId = RandomString(23);

                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;

                        // First insert into reviews table
                        cmd.CommandText = "INSERT INTO reviews (reviewid, reviewdate, starcount, text, " +
                            "votesuseful, votescool, votesfunny) VALUES (@reviewid, @date, @rating, @text, 0, 0, 0)";
                        cmd.Parameters.AddWithValue("reviewid", newReviewId);
                        cmd.Parameters.AddWithValue("date", DateTime.Today);
                        cmd.Parameters.AddWithValue("rating", rating.SelectedItem);
                        cmd.Parameters.AddWithValue("text", text.Text);
                        cmd.ExecuteNonQuery();

                        // Second insert into writesfor table connection review, user, and business
                        cmd.CommandText = "INSERT INTO writesfor(reviewid, userid, businessid) " +
                            "VALUES (@reviewid, @userid, @businessid)";
                        cmd.Parameters.AddWithValue("userid", tempUserId);
                        cmd.Parameters.AddWithValue("businessid", businessid);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();

                    // inserts didn't fail
                    text.Text = "";
                    rating.SelectedIndex = -1;
                    MessageBox.Show("Review Added!", "Yelp");
                }
            }
        }
    }
}
