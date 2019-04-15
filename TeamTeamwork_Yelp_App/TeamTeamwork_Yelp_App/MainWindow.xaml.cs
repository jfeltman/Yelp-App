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

        // Runs the application
        public void runApp()
        {
            numBusinessesLabel.Visibility = Visibility.Hidden;
            BS.addSearchResultColumns(searchResultsGrid);
            BS.addStates(stateList);
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
            BS.searchForBusiness(zipList, selectedCategoryList, searchResultsGrid);
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
                selectedBusinessName.Content = selectedBusiness.name;
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
    }
}
