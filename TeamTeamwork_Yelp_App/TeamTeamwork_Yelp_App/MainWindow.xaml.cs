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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace TeamTeamwork_Yelp_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            runApp();
        }

        private BusinessSearch BS = new BusinessSearch();
        private AddReview AR = new AddReview();
        private UserInformation UsI = new UserInformation();
        private String currentUser = "";

        // Runs the application
        public void runApp()
        {
            numBusinessesLabel.Visibility = Visibility.Hidden;
            BS.addSearchResultColumns(searchResultsGrid);
            BS.addStates(stateList);
            BS.addSortByValues(sortResultsComboBox);
            BS.addFriendReviewsColumns(selectedBusinessFriendReviewsGrid);
            AR.addRatings(addReviewRatingBox);
            selectedBusinessGrid.Visibility = Visibility.Hidden;
        }

        // State from statelist was selected
        private void stateList_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // First clear everything
            cityList.Items.Clear();
            BS.stateListChanged(stateList, cityList);
        }

        // City from city list was selected
        private void cityList_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // First clear everthing
            zipList.Items.Clear();
            BS.cityListChanged(stateList, cityList, zipList);
        }

        // Zip from zip list was selected
        private void zipList_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // clear everything
            categoryList.Items.Clear();
            selectedCategoryList.Items.Clear();
            selectedBusinessGrid.Visibility = Visibility.Hidden;
            BS.zipListChanged(zipList, categoryList);
        }

        // Add category to the selected category list
        private void addCatBtnClicked(object sender, RoutedEventArgs e)
        {
            if (!selectedCategoryList.Items.Contains(categoryList.SelectedItem))
            {
                selectedCategoryList.Items.Add(categoryList.SelectedItem);
            }
        }

        // Remove selected category from the current selected category list
        private void removeCatBtnClicked(object sender, RoutedEventArgs e)
        {
            selectedCategoryList.Items.Remove(selectedCategoryList.SelectedItem);
        }

        // Search For Businesses button clicked
        private void searchBusinessesClicked(object sender, RoutedEventArgs e)
        {
            // remove all previous search results
            searchResultsGrid.Items.Clear();

            List<CheckBox> checkBoxes = createCheckBoxList();
            BS.searchForBusiness(zipList, selectedCategoryList, searchResultsGrid, checkBoxes, sortResultsComboBox);

            numBusinessesLabel.Content = "# of Businesses: " + searchResultsGrid.Items.Count;
            numBusinessesLabel.Visibility = Visibility.Visible;
        }

        // User clicks on a business from the results
        private void resultsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchResultsGrid.SelectedItem != null)
            {
                // show selected business info
                selectedBusinessGrid.Visibility = Visibility.Visible;
                Business selectedBusiness = (Business)searchResultsGrid.SelectedItem;
                SelectedBusiness SB = new SelectedBusiness(selectedBusiness);
                SB.setBusinessInfo(selectedBusinessName, selectedBusinessAddress, selectedBusinessAttributes, selectedBusinessHoursList, selectedBusinessCategoriesList);
                SB.setFriendsReviews(currentUser, selectedBusinessFriendReviewsGrid);
            }
        }

        // Show the reviews for a specifc business
        private void showReviewsBtnClicked(object sender, RoutedEventArgs e)
        {
            if (searchResultsGrid.SelectedItem != null)
            {
                Business selectedBusiness = (Business)searchResultsGrid.SelectedItem;
                ReviewsWindow rw = new ReviewsWindow(selectedBusiness.businessid);
                rw.Show();
            }
        }

        // Add a new review for the selected business
        private void addReviewBtnClicked(object sender, RoutedEventArgs e)
        {
            if (searchResultsGrid.SelectedItem != null)
            {
                Business selectedBusiness = (Business)searchResultsGrid.SelectedItem;
                AR.addReview(addReviewTextBox, addReviewRatingBox, selectedBusiness.businessid);
            }
        }

        private void SearchNameButtonClick(object sender, RoutedEventArgs e)
        {
            userIdListbox.Items.Clear();
            UsI.searchUser(EnterNameBox, userIdListbox);
        }

        private void checkInButton_Clicked(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void showCheckinsBtnClicked(object sender, RoutedEventArgs e)
        {
            if (searchResultsGrid.SelectedItem != null)
            {
                Business selectedBusiness = (Business)searchResultsGrid.SelectedItem;
                CheckinGraph graph = new CheckinGraph(selectedBusiness.businessid);
                graph.Show();
            }
        }

        private void addToFavoritesBtnClicked(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void UpdateUsI_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<CheckBox> createCheckBoxList()
        {
            List<CheckBox> checkBoxes = new List<CheckBox>();
            // price range
            checkBoxes.Add(price1);
            checkBoxes.Add(price2);
            checkBoxes.Add(price3);
            checkBoxes.Add(price4);
            
            // attributes
            checkBoxes.Add(takesCreditCards);
            checkBoxes.Add(takesReservations);
            checkBoxes.Add(wheelchairAccess);
            checkBoxes.Add(outdoorSeating);
            checkBoxes.Add(goodForKids);
            checkBoxes.Add(goodForGroups);
            checkBoxes.Add(delivery);
            checkBoxes.Add(takeOut);
            checkBoxes.Add(freeWifi);
            checkBoxes.Add(bikeParking);

            // meal
            checkBoxes.Add(breakfast);
            checkBoxes.Add(brunch);
            checkBoxes.Add(lunch);
            checkBoxes.Add(dinner);
            checkBoxes.Add(dessert);
            checkBoxes.Add(lateNight);

            return checkBoxes;
        }

        private void userIdListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentUser = userIdListbox.SelectedItem.ToString();
        }
    }
}
