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
    /// Interaction logic for ReviewsWindow.xaml
    /// </summary>
    public partial class ReviewsWindow : Window
    {
        public class Review
        {
            public string reviewid { get; set; }
            public string userid { get; set; }
            public string businessid { get; set; }
            public string stars { get; set; }
            public string date { get; set; }
            public string text { get; set; }
            public string funny { get; set; }
            public string useful { get; set; }
            public string cool { get; set; }
            public string userName { get; set; }
        }

        public ReviewsWindow(string businessid)
        {
            InitializeComponent();
            addColumns();
            getReviews(businessid);
        }

        private string buildConnString()
        {
            return "Host=localhost; Username=postgres; Password=mypassword; Database=yelpdb";
        }

        // Add review header columns
        private void addColumns()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Date";
            col1.Binding = new Binding("date");
            reviewsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "User";
            col2.Binding = new Binding("userName");
            reviewsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Stars";
            col3.Binding = new Binding("stars");
            reviewsGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Text";
            col4.Binding = new Binding("text");
            col4.Width = 500;
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
            style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            col4.ElementStyle = style;
            reviewsGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "Funny";
            col5.Binding = new Binding("funny");
            reviewsGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Useful";
            col6.Binding = new Binding("useful");
            reviewsGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "Cool";
            col7.Binding = new Binding("cool");
            reviewsGrid.Columns.Add(col7);
        }

        // Get all reviews from a selected businessid
        private void getReviews(string businessid)
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT reviews.reviewid, writesfor.userid, writesfor.businessid, " +
                        "reviewdate, users.name, reviews.starcount, text, reviews.votesuseful, reviews.votescool, " +
                        "reviews.votesfunny FROM reviews, writesfor, users " +
                        "WHERE reviews.reviewid = writesfor.reviewid AND writesfor.userid = users.userid AND " +
                        "writesfor.businessid = '" + businessid + "' ORDER BY reviews.starcount DESC;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviewsGrid.Items.Add(new Review
                            {
                                reviewid = reader.GetString(0),
                                userid = reader.GetString(1),
                                businessid = reader.GetString(2),
                                date = reader.GetDate(3).ToString(),
                                userName = reader.GetString(4),
                                stars = reader.GetInt32(5).ToString(),
                                text = reader.GetString(6),
                                useful = reader.GetInt32(7).ToString(),
                                cool = reader.GetInt32(8).ToString(),
                                funny = reader.GetInt32(9).ToString()
                            });
                        }
                    }
                }
                conn.Close();
            }
        }
    }
}
