using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using ModulesLibrary;

namespace POEPart2
{
    public partial class MainWindow : Window
    {
        #region Global Variables
        // DispatchTimer to display current time on window
        private DispatcherTimer timer;

        // Temp List for the ModuleStudyRecord used to update the database
        private List<ModuleStudyRecord> StudiedHoursRecords = new List<ModuleStudyRecord>();

        // Current User
        private int currentUserId;
        #endregion

        #region MainWindow Constructor
        public MainWindow(int User)
        {
            InitializeComponent();

            // Setting the UserId for this instance
            currentUserId = User;

            // Starting timer
            CurrentTimer();

            // Invoking StartUp Method to update ComboBoxes and DataGrids
            StartUp();
        }
        #endregion

        #region Timer
        private void CurrentTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Update every second
            };
            timer.Tick += Timer_Tick;

            //Start the timer
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //Updating the TextBlock or Label with the current time
            currentTimeTb.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        #endregion

        #region StartUp
        private void StartUp()
        {
            //populating the comboboxes
            SemesterDisplayCb.Items.Clear();
            SemesterHWCb.Items.Clear();
            SemesterNameForModuleCb.Items.Clear();

            using (var context = new ApplicationDbContext())
            {
                // Retrieving the names of all semesters from the database
                var semesterNames = context.Semesters.Where(s => s.UserId == currentUserId).Select(s => s.Name).ToList();

                // Populating the combo boxes with semester names
                foreach (var semesterName in semesterNames)
                {
                    SemesterDisplayCb.Items.Add(semesterName);
                    SemesterHWCb.Items.Add(semesterName);
                    SemesterNameForModuleCb.Items.Add(semesterName);
                }

                // Populating ModuleStudy Records in datagrid
                var allModuleStudyRecords = context.ModuleStudyRecords.Where(u => u.UserId == currentUserId).ToList();
                hoursWorkedDataGrid.ItemsSource = null;
                hoursWorkedDataGrid.ItemsSource = allModuleStudyRecords;
            }
        }
        #endregion

        #region Logout
        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            // Back to landing for another user to login in or register
            Landing landing = new Landing();
            this.Close();
            landing.Show();
        }
        #endregion

        #region Add Semesters
        private void AddSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            //invoking AddSemester Method
            AddSemester();
        }

        private void AddSemester()
        {
            if (int.TryParse(numWeeksTb.Text, out int parsedWeeks) && semesterNameCb.SelectedItem != null)
            {
                string semesterName = semesterNameCb.Text;

                // Checking if a semester with the same name already exists
                int existingSemesterId = FindSemester(semesterName);

                if (existingSemesterId != -1)
                {
                    // Semester with the same name already exists
                    MessageBoxResult result = MessageBox.Show($"Semester '{semesterName}' already exists. Do you want to overwrite it?",
                        "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            // Retrieving the existing semester from the database
                            var existingSemester = context.Semesters.Find(existingSemesterId);

                            if (existingSemester != null)
                            {
                                // Updating the properties of the existing semester
                                existingSemester.Name = semesterName;
                                existingSemester.NumberOfWeeks = parsedWeeks;

                                // Temp variable for new start date
                                DateTime startDate = startDatePicker.SelectedDate.GetValueOrDefault(DateTime.Now);
                                if (startDate == DateTime.Now)
                                {
                                    MessageBox.Show("Please select a date", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }

                                existingSemester.StartDate = startDate;
                                existingSemester.EndDate = startDate.AddDays(7 * parsedWeeks);
                                existingSemester.UserId = currentUserId;

                                // Saving the changes to the database
                                context.SaveChanges();
                            }
                        }

                        // Clear textboxes
                        ClearBoxes();

                        // Updating Self-Study for all modules
                        UpdateSelfStudy(existingSemesterId);

                        // Updating datagrid with any new information for that semester
                        moduleDataGrid.ItemsSource = null;
                        using (var context = new ApplicationDbContext())
                        {
                            // Retrieving the modules for the specified semester
                            var modulesForSemester = context.Modules
                                .Where(m => m.SemesterId == existingSemesterId)
                                .ToList(); // ToList() is used to materialize the query

                            // Setting the ItemsSource of the moduleDataGrid to the fetched modules
                            moduleDataGrid.ItemsSource = modulesForSemester;
                        }

                        // Informing the user about the update
                        MessageBox.Show($"Semester '{semesterName}' has been updated.", "Successful");
                    }
                }
                else
                {
                    using (var context = new ApplicationDbContext())
                    {
                        // Creating a new Semester object and set its properties
                        DateTime startDate = startDatePicker.SelectedDate.GetValueOrDefault(DateTime.Now);
                        if (startDate == DateTime.Now)
                        {
                            MessageBox.Show("Please select a date", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        var newSemester = new Semester
                        {
                            Name = semesterName,
                            NumberOfWeeks = parsedWeeks,
                            StartDate = startDate,
                            EndDate = startDate.AddDays(7 * parsedWeeks),
                            UserId = currentUserId
                        };

                        // Adding the new semester to the context
                        context.Semesters.Add(newSemester);

                        // Saving the changes to the database
                        context.SaveChanges();


                    }

                    // Adding to ComboBoxes
                    StartUp();

                    // Clearing textboxes
                    ClearBoxes();

                    // Informing the user about the successful addition
                    MessageBox.Show($"Successfully added Semester '{semesterName}'", "Successful");
                }
            }
            else
            {
                // Handling the case when parsing fails or semester is not selected
                MessageBox.Show("Invalid variables entered. Please enter valid numbers and select a semester.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateSelfStudy(int existingSemesterId)
        {
            using (var context = new ApplicationDbContext())
            {
                // Retrieving the NumberOfWeeks for the specified semester
                int numberOfWeeks = context.Semesters
                    .Where(s => s.SemesterId == existingSemesterId)
                    .Select(s => s.NumberOfWeeks)
                    .FirstOrDefault();

                if (numberOfWeeks > 0)
                {
                    // Fetching the associated modules for the semester
                    var modulesForSemester = context.Modules
                        .Where(m => m.SemesterId == existingSemesterId)
                        .ToList();

                    foreach (var module in modulesForSemester)
                    {
                        int credits = module.Credits;
                        int classHoursPerWeek = module.ClassHoursPerWeek;

                        int selfStudy = ModuleManagement.SelfStudyCalc(credits, numberOfWeeks, classHoursPerWeek);

                        // Updating SelfStudyHoursPerWeek for the current module
                        module.SelfStudyHoursPerWeek = selfStudy;
                    }

                    // Saving the changes to the database
                    context.SaveChanges();
                }
            }
        }
        #endregion

        #region Add Module
        private void AddModuleButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoking the AddModule Method
            AddModule();
        }

        private void AddModule()
        {
            if (string.IsNullOrWhiteSpace(SemesterNameForModuleCb.Text))
            {
                // If user hasn't selected a semester
                MessageBox.Show("Please select a semester before adding a module.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Exit the method
            }

            if (int.TryParse(creditsTb.Text, out int credits) && int.TryParse(hrsTb.Text, out int hours))
            {
                // Finding and setting the newly selected semester
                int currentSemesterId = FindSemester(SemesterNameForModuleCb.Text);

                using (var context = new ApplicationDbContext())
                {
                    // Fetching numberOfWeeks
                    int numberOfWeeks = context.Semesters
                    .Where(s => s.SemesterId == currentSemesterId)
                    .Select(s => s.NumberOfWeeks)
                    .FirstOrDefault();

                    // Calculating SelfStudy Hours
                    int selfStudy = ModuleManagement.SelfStudyCalc(credits, numberOfWeeks, hours);

                    // Creating a new Module object and setting its properties
                    var newModule = new Module
                    {
                        Code = codeTb.Text,
                        Name = nameTb.Text,
                        Credits = credits,
                        ClassHoursPerWeek = hours,
                        SelfStudyHoursPerWeek = selfStudy,
                        UserId = currentUserId,
                        SemesterId = currentSemesterId
                    };

                    // Adding the new module to the context
                    context.Modules.Add(newModule);

                    // Saving the changes to the database
                    context.SaveChanges();
                }

                // Temporary local module name for messagebox
                string moduleName = nameTb.Text;

                // Clearing all the texboxes
                ClearBoxes();

                // Telling user it successfully added a semester
                MessageBox.Show($"Successfully added module: '{moduleName}'", "Successful");
            }
            else
            {
                // Handle the case when parsing fails
                MessageBox.Show("Invalid variables entered. Please enter a valid numbers.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        #endregion

        #region Find Semester
        private int FindSemester(string semesterName)
        {
            int semesterId = -1; // Initialized a default value or error value

            using (var context = new ApplicationDbContext())
            {
                // Using Entity Framework to query the database
                var semester = context.Semesters
                    .Where(s => s.Name == semesterName && s.UserId == currentUserId)
                    .Select(s => s.SemesterId)
                    .FirstOrDefault();

                if (semester != 0)
                {
                    semesterId = semester;
                }
            }

            return semesterId;
        }
        #endregion

        #region Semester Display
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Finding and setting the newly selected semester
            int currentSemesterId = FindSemester(SemesterDisplayCb.Text);
            if (currentSemesterId != -1)
            {
                if (SemesterDisplayCb.SelectedItem != null)
                {
                    // Updating datagrid with any new information for that semester
                    moduleDataGrid.ItemsSource = null;
                    using (var context = new ApplicationDbContext())
                    {
                        // Retrieving the modules for the specified semester
                        var modulesForSemester = context.Modules
                            .Where(m => m.SemesterId == currentSemesterId && m.UserId == currentUserId)
                            .ToList(); // ToList() is used to materialize the query

                        // Setting the ItemsSource of the moduleDataGrid to the fetched modules
                        moduleDataGrid.ItemsSource = modulesForSemester;
                    }
                }
                else
                {
                    // Handling the case when semester is not selected
                    MessageBox.Show("Semester not selected. Please select a semester.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                // Handling the case when semester is not found
                MessageBox.Show("Semester not found. Please enter a semester.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        #endregion

        #region Hours Worked
        private Module GetSelectedModule()
        {
            // Searching for semester id
            int currentSemesterId = FindSemester(SemesterHWCb.Text);

            using (var context = new ApplicationDbContext())
            {
                // Using currentSemesterId and code from the combobox to get selected module
                Module selectedModule = context.Modules
                    .FirstOrDefault(m => m.SemesterId == currentSemesterId && m.Code == ModuleHWCb.Text);
                return selectedModule;
            }

        }

        private void SemesterHWCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Checking if there are selected items
            if (e.AddedItems.Count > 0)
            {
                // Getting the selected item
                var semesterName = e.AddedItems[0] as string;

                if (semesterName != null)
                {
                    // Finding and setting the newly selected semester
                    int currentSemester = FindSemester(semesterName);

                    using (var context = new ApplicationDbContext())
                    {
                        // Using Entity Framework to query the database
                        var modules = context.Modules
                            .Where(m => m.SemesterId == currentSemester)
                            .Select(m => m.Code)
                            .ToList();

                        // Clearing items from combobox
                        ModuleHWCb.Items.Clear();

                        // Adding current semester modules to combobox list
                        foreach (var module in modules)
                        {
                            ModuleHWCb.Items.Add(module);
                        }
                    }
                }
            }
        }


        private void RecordHoursButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(hoursWorkedTb.Text, out int hoursWorked))
            {
                DateTime selectedDate = studyDatePicker.SelectedDate.GetValueOrDefault(DateTime.Now);
                if (selectedDate == DateTime.Now)
                {
                    MessageBox.Show("Please select a valid date", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                using (var context = new ApplicationDbContext())
                {
                    // Find the selected module
                    Module selectedModule = GetSelectedModule();

                    if (selectedModule != null)
                    {
                        int currentSemesterId = FindSemester(SemesterHWCb.Text);
                        Semester currentSemester = context.Semesters.FirstOrDefault(s => s.SemesterId == currentSemesterId);

                        if (currentSemester != null)
                        {
                            // Check if the selected date is within the semester's date range
                            if (selectedDate >= currentSemester.StartDate && selectedDate <= currentSemester.EndDate)
                            {
                                // Calculate the start and end dates of the current week
                                DateTime weekStartDate = selectedDate.Date.AddDays(-(int)selectedDate.DayOfWeek);
                                DateTime weekEndDate = weekStartDate.AddDays(6);

                                // Calculate the total hours worked for the current week
                                int totalHoursThisWeek = StudiedHoursRecords
                                    .Where(record => record.Date >= weekStartDate && record.Date <= weekEndDate && record.ModuleId == selectedModule.ModuleId)
                                    .Sum(record => record.HoursWorked);

                                // Calculate the remaining self-study hours for the current week
                                int remainingSelfStudyHours = selectedModule.SelfStudyHoursPerWeek - totalHoursThisWeek;

                                // Calculate the hours that will exceed the allowed self-study hours
                                int hoursExceedingLimit = remainingSelfStudyHours - hoursWorked;

                                // Check if adding the new record would exceed the allowed self-study hours
                                if (hoursExceedingLimit < 0)
                                {
                                    MessageBox.Show($"Adding {hoursWorked} hours would exceed the allowed self-study hours for this week. " +
                                        $"Remaining hours: {remainingSelfStudyHours}. You will exceed by {hoursExceedingLimit} hours.",
                                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                                }

                                // Adding a new ModuleStudyRecord to the list in selectedModule
                                StudiedHoursRecords.Add(new ModuleStudyRecord
                                {
                                    ModuleCode = selectedModule.Code,
                                    Date = selectedDate,
                                    HoursWorked = hoursWorked,
                                    // calculating hours left as per the POE
                                    HoursLeft = selectedModule.SelfStudyHoursPerWeek - totalHoursThisWeek - hoursWorked,
                                    ModuleId = selectedModule.ModuleId,
                                    UserId = currentUserId
                                });

                                //taking last record from the list and saving it to the database
                                context.ModuleStudyRecords.Add(StudiedHoursRecords.Last(r => r.UserId == currentUserId));

                                //saving changes
                                context.SaveChanges();

                                // Displaying all ModuleStudyRecords in the datagrid
                                var allModuleStudyRecords = context.ModuleStudyRecords.Where(u => u.UserId == currentUserId).ToList();
                                hoursWorkedDataGrid.ItemsSource = null;
                                hoursWorkedDataGrid.ItemsSource = allModuleStudyRecords;

                                // Clear ComboBoxes and TextBoxes
                                ClearBoxes();
                            }
                            else
                            {
                                MessageBox.Show($"Selected date is outside the semester's date range." +
                                    $"\nPlease select a date within: {currentSemester.StartDate:dd/MM/yyyy} & {currentSemester.EndDate:dd/MM/yyyy}",
                                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please select a module.",
                                "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid input for hours worked. Please enter a valid number.",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        #region Clear Textboxes
        private void ClearBoxes()
        {
            codeTb.Clear();
            nameTb.Clear();
            creditsTb.Clear();
            hrsTb.Clear();
            numWeeksTb.Clear();
            hoursWorkedTb.Clear();
            SemesterNameForModuleCb.SelectedItem = null;
            SemesterHWCb.SelectedItem = null;
            ModuleHWCb.SelectedItem = null;
            ModuleHWCb.Items.Clear();
            startDatePicker.SelectedDate = null;
            studyDatePicker.SelectedDate = null;
        }

        #endregion

    }
}
