using ModulesLibrary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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

namespace POEPart2
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            //setting focus to username for quick input
            usernameTb.Focus();
        }

        //method to validate user against the db
        public bool ValidateUser(string username, string password)
        {
            using (var context = new ApplicationDbContext())
            {
                //validating hashing as well
                bool userExists = context.Users.Any(u => u.Username == username && u.Password == password);

                if (userExists)
                {
                    // User exists
                    return true;
                }
                else
                {
                    // User does not exist
                    return false;
                }
            }
        }

        //login btn click
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Taking user input
            string enteredUsername = usernameTb.Text;
            string enteredPassword = passwordTb.Password;

            // If no username is entered
            if (string.IsNullOrWhiteSpace(usernameTb.Text))
            {
                MessageBox.Show("Please enter a username", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // If no password is entered
            if (string.IsNullOrWhiteSpace(enteredPassword))
            {
                MessageBox.Show("Please enter a password", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            //hashing password
            string hashedPassword = PasswordHash.HashPassword(enteredPassword);

            // Show the loading screen
            var loadingScreen = new LoadingScreen();
            loadingScreen.Show();

            await Task.Run(() =>
            {
                // Validate user credentials
                if (ValidateUser(enteredUsername, hashedPassword))
                {
                    using (var context = new ApplicationDbContext())
                    {
                        // Finding the user's ID for their specific info
                        Users foundUser = context.Users.FirstOrDefault(u => u.Username == enteredUsername && u.Password == hashedPassword);

                        // Successful login
                        Dispatcher.Invoke(() =>
                        {
                            // Hide the loading screen
                            loadingScreen.Close();

                            MessageBox.Show($"Welcome, {foundUser.Username}", "Success", MessageBoxButton.OK, MessageBoxImage.None);

                            // Parsing the UserID to MainWindow
                            MainWindow mw = new MainWindow(foundUser.UserId);
                            this.Close();
                            mw.Show();
                        });
                    }
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        // Hide the loading screen
                        loadingScreen.Close();

                        MessageBox.Show("Login Unsuccessful - User Not Found");
                    });
                }
            });


        }

        //back btn click
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //new Landing instance
            Landing landing = new Landing();
            this.Close();
            landing.Show();
        }
    }
}
