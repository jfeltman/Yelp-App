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
    class SelectedBusiness
    {
        private Business business;

        public SelectedBusiness(Business selectedBusiness)
        {
            business = selectedBusiness;
        }

        public void setBusinessInfo(Label name, Label address, Label attributes, ListBox hours, ListBox categories)
        {
            // Set the easy stuff that we have stored in business (name and address)
            name.Content = business.name;
            address.Content = business.address + " " + business.city + ", " + business.state;

            // Now query to get attributes, hours, and categories (use business.businessid)
        }


    }
}
