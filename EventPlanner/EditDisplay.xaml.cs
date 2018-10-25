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

namespace EventPlanner
{
    /// <summary>
    /// Interaction logic for EditDisplay.xaml
    /// </summary>
    public partial class EditDisplay : Window
    {
        MainWindow main;
        Task t;
        public EditDisplay(MainWindow main) // Default constructor
        {
            InitializeComponent();
            this.main = main;
            t = null;
        }
        public EditDisplay(MainWindow main, Task t) // Constructor that takes MainWindow and Task objects
        {
            InitializeComponent();
            this.main = main;
            this.t = t;

            if (t.getTime().Hour == 0) // Handles AM time when hour is 0
            {
                hourEntry.Text = "12";
                ampmSelector.SelectedIndex = 0; //Sets to AM
            }
            else if (t.getTime().Hour > 12) // Handles PM times
            {
                hourEntry.Text = (t.getTime().Hour - 12).ToString();
                ampmSelector.SelectedIndex = 1; //Sets to PM
            }
            else // Handles all other AM times
            {
                hourEntry.Text = t.getTime().Hour.ToString();
                ampmSelector.SelectedIndex = 0; //Sets to AM
            }
            // Set time, date, task name, and task note values
            minuteEntry.Text = t.getTime().Minute.ToString();
            datePicker.SelectedDate = t.getTime();
            nameEntry.Text = t.getTitle();
            noteEntry.Text = t.getNotes();
            listBox.Text = t.getCategory();
        }
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            main.editToggle = false; // Turn edit mode off
            this.Close(); // Close the window
        }

        public void getSelectedBox(object sender, RoutedEventArgs e)
        {
            string selected;
            selected = listBox.SelectedItem.ToString();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan time;
            int hours;
            int minutes;
            DateTime date;
            string taskName;
            string taskNotes;
            string category;

            string errorMessage = "Please enter a task name, category, and a valid time where hours (between 1 and 12) and minutes (between 0 and 59).";

            try
            {
                if (minuteEntry.Text.ToString() == "") minuteEntry.Text = "00"; // When minute is empty, default to 0

                hours = int.Parse(hourEntry.Text.ToString());
                minutes = int.Parse(minuteEntry.Text.ToString());
                date = (DateTime)datePicker.SelectedDate;
                taskName = nameEntry.Text;
                taskNotes = noteEntry.Text;
                category = listBox.Text;
                
                Console.WriteLine("Category: " + category);

                if (taskName == "" || taskName.Contains("<<@>>") || taskNotes.Contains("<<@>>")) throw new FormatException(); // Prevent empty task names

                // Check for correct time format
                if ((hours < 1) || (hours > 12) || (minutes < 0) || (minutes > 59)) throw new FormatException();

                // Check if category has been filled
                if (category == "") throw new FormatException();

                // Convert time to military time
                if (hours == 12) hours = 0;

                // Add 12 to PM times to convert to military time
                if (ampmSelector.SelectedValue.ToString() == "PM") hours += 12;

                // Create new TimeSpan
                time = new TimeSpan(hours, minutes, 0);

                date = date.Add(time);

                //Console.Out.WriteLine(date.ToString());
                //Console.Out.WriteLine(taskName);
                //Console.Out.WriteLine(taskNotes);

                if (t == null) // If task is null, add task object to list
                {
                    main.addTaskToList(new Task(taskName, date, taskNotes,category));
                }
                else // Create new task object and update the view
                {
                    t.setTitle(taskName);
                    t.setTime(date);
                    t.setNotes(taskNotes);
                    t.setCategory(category);
                    main.updateView();
                }

                main.editToggle = false;
                Close();

            }
            catch (FormatException) // Handle FormatException
            {
                MessageBox.Show(errorMessage);
                return;
            }
            catch (OverflowException) // Handle OverflowException
            {
                MessageBox.Show(errorMessage);
                return;
            }
            catch (InvalidOperationException) // Handle Exception when there is no Date selected
            {
                MessageBox.Show("Please select a date.");
            }
        }

        private void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.editToggle = false; // Turn edit mode off when window is closed
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
