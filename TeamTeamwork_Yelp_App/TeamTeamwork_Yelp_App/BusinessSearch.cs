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
    class BusinessSearch
    {
        private string buildConnString()
        {
            return "Host=localhost; Username=postgres; Password=mypassword; Database=yelpdb";
        }

        // Add the columns for the search results
        public void addSearchResultColumns(DataGrid grid)
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Business Name";
            col1.Binding = new Binding("name");
            col1.Width = 200;
            grid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Address";
            col2.Binding = new Binding("address");
            col2.Width = 200;
            grid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("city");
            grid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "State";
            col4.Binding = new Binding("state");
            grid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "Zip Code";
            col5.Binding = new Binding("zip");
            grid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Distance (miles)";
            col6.Binding = new Binding("distance");
            grid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "Stars";
            col7.Binding = new Binding("stars");
            grid.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Header = "# of Reviews";
            col8.Binding = new Binding("reviewCount");
            grid.Columns.Add(col8);

            DataGridTextColumn col9 = new DataGridTextColumn();
            col9.Header = "Total Checkins";
            col9.Binding = new Binding("checkinCount");
            grid.Columns.Add(col9);
        }

        public void addSortByValues(ComboBox sortByList)
        {
            sortByList.Items.Add("Name (Default");
            sortByList.Items.Add("Highest Rating");
            sortByList.Items.Add("Most Reviewed");
            sortByList.Items.Add("Most Check-ins");
            sortByList.Items.Add("Nearest");
        }

        public void addFriendReviewsColumns(DataGrid grid)
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "User Name";
            col1.Binding = new Binding("userName");
            grid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Date";
            col2.Binding = new Binding("date");
            grid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Text";
            col3.Binding = new Binding("text");
            grid.Columns.Add(col3);
        }

        // Fill in the states box with all the distinct states from the database
        public void addStates(ComboBox stateList)
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT stateabbrev FROM businesses ORDER BY stateabbrev;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stateList.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        // State was selected so populate city list
        public void stateListChanged(ComboBox stateList, ListBox cityList)
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT city FROM businesses WHERE stateabbrev = '" + stateList.SelectedItem.ToString() +"' ORDER BY city;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cityList.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        // State and city have been selected, so populate zip code list
        public void cityListChanged(ComboBox stateList, ListBox cityList, ListBox zipList)
        {
            // if state or city isnt selected, return
            if (stateList.SelectedItem == null || cityList.SelectedItem == null)
            {
                return;
            }

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT zip FROM businesses WHERE stateabbrev = '" 
                        + stateList.SelectedItem.ToString() + "' AND city = '" 
                        + cityList.SelectedItem.ToString() + "' ORDER BY zip;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            zipList.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        // Zip code has been selected, so populate all business categories within that zipcode
        public void zipListChanged(ListBox zipList, ListBox categoryList)
        {
            // if zip isnt selected, return
            if (zipList.SelectedItem == null)
            {
                return;
            }

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT categories.name FROM businesses, " +
                        "categories WHERE businesses.businessid = categories.businessid " +
                        "AND zip = '" + zipList.SelectedItem.ToString() + "' ORDER BY categories.name;";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categoryList.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        // Search for businesses based on zip code and/or categories selected
        public void searchForBusiness(ListBox zipList, ListBox selectedCategoryList, DataGrid grid, 
            List<CheckBox> attributes, ComboBox sortResults)
        {
            // if zip isnt selected, return
            if (zipList.SelectedItem == null)
            {
                return;
            }

            string cmdText = "SELECT DISTINCT name, street, city, stateabbrev, zip, starrating, " + 
                "reviewcount, checkincount, businessid FROM businesses " +
                "WHERE zip = '" + zipList.SelectedItem.ToString() + "'";
            
            if (selectedCategoryList.Items.IsEmpty == false)
            {
                cmdText = createCategoriesQuery(cmdText, selectedCategoryList);
            }

            cmdText = createAttributesQuery(cmdText, attributes);
            cmdText = addSorting(cmdText, sortResults);

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdText;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            grid.Items.Add(new Business()
                            {
                                name = reader.GetString(0),
                                address = reader.GetString(1),
                                city = reader.GetString(2),
                                state = reader.GetString(3),
                                zip = reader.GetInt32(4).ToString(),
                                distance = "0", // do this in milestone3
                                stars = reader.GetDouble(5).ToString(),
                                reviewCount = reader.GetInt32(6).ToString(),
                                checkinCount = reader.GetInt32(7).ToString(),
                                businessid = reader.GetString(8)
                            });
                        }
                    }
                }
                conn.Close();
            }
        }

        private string createCategoriesQuery(string cmdText, ListBox categoryList)
        {
            string catText = " AND businessid IN ";

            foreach (var item in categoryList.Items)
            {
                catText += "(SELECT businessid FROM categories WHERE name = '" + item.ToString() + "') AND businessid IN ";
            }

            catText = catText.Remove(catText.Length - 19);

            cmdText += catText;

            return cmdText;
        }

        private string createAttributesQuery(string cmdText, List<CheckBox> attributes)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name)
                {
                    case "price1":
                        cmdText += attributeText(attribute, "RestaurantsPriceRange2", "1");
                        break;
                    case "price2":
                        cmdText += attributeText(attribute, "RestaurantsPriceRange2", "2");
                        break;
                    case "price3":
                        cmdText += attributeText(attribute, "RestaurantsPriceRange2", "3");
                        break;
                    case "price4":
                        cmdText += attributeText(attribute, "RestaurantsPriceRange2", "4");
                        break;
                    case "takesCreditCards":
                        cmdText += attributeText(attribute, "BusinessAcceptsCreditCards", "true");
                        break;
                    case "takesReservations":
                        cmdText += attributeText(attribute, "RestaurantsReservations", "true");
                        break;
                    case "wheelchairAccess":
                        cmdText += attributeText(attribute, "WheelchairAccessible", "true");
                        break;
                    case "outdoorSeating":
                        cmdText += attributeText(attribute, "OutdoorSeating", "true");
                        break;
                    case "goodForKids":
                        cmdText += attributeText(attribute, "GoodForKids", "true");
                        break;
                    case "goodForGroups":
                        cmdText += attributeText(attribute, "RestaurantsGoodForGroups", "true");
                        break;
                    case "delivery":
                        cmdText += attributeText(attribute, "RestaurantsDelivery", "true");
                        break;
                    case "takeout":
                        cmdText += attributeText(attribute, "RestaurantsTakeOut", "true");
                        break;
                    case "freeWifi":
                        cmdText += attributeText(attribute, "WiFi", "free");
                        break;
                    case "bikeparking":
                        cmdText += attributeText(attribute, "BikeParking", "true");
                        break;
                    case "breakfast":
                        cmdText += attributeText(attribute, "breakfast", "true");
                        break;
                    case "brunch":
                        cmdText += attributeText(attribute, "brunch", "true");
                        break;
                    case "lunch":
                        cmdText += attributeText(attribute, "lunch", "true");
                        break;
                    case "dinner":
                        cmdText += attributeText(attribute, "dinner", "true");
                        break;
                    case "dessert":
                        cmdText += attributeText(attribute, "dessert", "true");
                        break;
                    case "latenight":
                        cmdText += attributeText(attribute, "latenight", "true");
                        break;
                    default:
                        break;
                }
            }

            return cmdText;
        }

        private string attributeText(CheckBox attribute, string name, string value)
        {
            string text = "";

            if (attribute.IsChecked == true)
            {
                text += "AND businessid IN (SELECT businessid FROM attributes WHERE name = '" + name + "' AND value = '" + value + "') ";

                return text;
            }
            else
            {
                return text;
            }
        }

        // TODO: ADD in current users lat and long to compute nearest sorting
        private string addSorting(string cmdText, ComboBox sortList)
        {
            if (sortList.SelectedItem == null)
            {
                // no sort selected, default to name
                cmdText += " ORDER BY name;";
            } else
            {
                switch (sortList.SelectedItem.ToString())
                {
                    case "Name (Default":
                        cmdText += " ORDER BY name;";
                        break;
                    case "Highest Rating":
                        cmdText += " ORDER BY starrating DESC;";
                        break;
                    case "Most Reviewed":
                        cmdText += " ORDER BY reviewcount DESC;";
                        break;
                    case "Most Check-ins":
                        cmdText += " ORDER BY checkincount DESC;";
                        break;
                    case "Nearest":
                        cmdText += " ORDER BY (POW((long - user.long), 2) + POW((lat - user.lat), 2));";
                        break;
                    default:
                        break;
                }
            }

            return cmdText;
        }
    }
}
