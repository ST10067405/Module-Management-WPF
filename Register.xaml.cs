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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();

            //setting focus to username for quick input
            usernameTb.Focus();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Landing landing = new Landing();
            this.Close();
            landing.Show();
        }

        private async void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            // Checking whether they click register without entering details
            if (!string.IsNullOrWhiteSpace(usernameTb.Text) && !string.IsNullOrWhiteSpace(passwordTb.Password))
            {
                string username = usernameTb.Text.Trim();

                //new loadingscreen object
                var loadingScreen = new LoadingScreen();
                loadingScreen.Show();

                bool userExists = await Task.Run(() =>
                {
                    //checking if the user exists
                    using (var context = new ApplicationDbContext())
                    {
                        return context.Users.Any(u => u.Username == username);
                    }
                });

                if (userExists)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("User with that username already exists", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        //closing loading screen
                        loadingScreen.Close();
                    });
                    //housekeeping
                    usernameTb.Focus();
                    //returning if a users exists
                    return;
                }
                
                //checking if they entered an email
                string email = emailTb.Text;
                if (!emailTb.Text.Contains("@"))
                {
                    email = "Not Provided";
                }
                string password = passwordTb.Password;
                string confirmedPassword = confirmPasswordTb.Password;

                // PASSWORD & CONFIRMEPASSWORD MUST BE THE SAME
                if (password == confirmedPassword)
                {
                    await Task.Run(() =>
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            // Creating a new user
                            Users newUser = new Users
                            {
                                Username = username,
                                Email = email,
                                Password = PasswordHash.HashPassword(password)
                            };

                            // Adding the new user to the database
                            context.Users.Add(newUser);
                            context.SaveChanges();
                        }
                    });

                    // Close the loading screen
                    loadingScreen.Close();

                    // Successful registration
                    MessageBox.Show("Registration Successful", "Success", MessageBoxButton.OK, MessageBoxImage.None);

                    // Open the new login screen once registered
                    Login login = new Login();
                    this.Close();
                    login.Show();
                }
                else
                {
                    // If passwords don't match
                    loadingScreen.Close();
                    MessageBox.Show("Password not the same - try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                // If no entries found
                MessageBox.Show("Please enter all required details before registering", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #region GotFocus & LostFocus
        private void emailTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (emailTb.Text == "optional")
            {
                // Object of Brush element
                SolidColorBrush foregroundBrush = new SolidColorBrush(Colors.Black);

                // Setting the foreground color of the emailTb
                emailTb.Foreground = foregroundBrush;

                emailTb.Text = "";
            }
        }

        private void emailTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(emailTb.Text))
            {
                emailTb.Text = "optional";

                // Object of Brush element
                SolidColorBrush foregroundBrush = new SolidColorBrush(Colors.LightGray);

                // Setting the foreground color of the emailTb
                emailTb.Foreground = foregroundBrush;
            }
        }
        #endregion
    }
}
