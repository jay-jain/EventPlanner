using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
using System.IO;
using System.Text.RegularExpressions;

namespace EventPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Class Variables
        public bool editToggle; // Determines if task is available for editing
        private Timer taskTimer; 
        private List<DateDisplay> dateDisplayList;
        private List<Task> taskList;
        private List<TaskDisplay> taskDisplayList;
        private int daysInFuture;
        private string printFile;

        // MainWindow constructor
        public MainWindow()
        {
            // Initialize variables
            printFile = "Tasks.txt";
            daysInFuture = 30;
            dateDisplayList = new List<DateDisplay>();
            taskList = new List<Task>();
            taskDisplayList = new List<TaskDisplay>();
            editToggle = false;

            InitializeComponent();

            readTasks(); // Reads tasks saved from previous sessions

            generateDates(daysInFuture, DateTime.Now); // Generates the calendar

            CurrentTime.Text = DateTime.Now.ToShortDateString() + ": " + DateTime.Now.ToShortTimeString(); // Generates current time
            
            // Handles timer which updates current time in the app
            taskTimer = new Timer((60 - DateTime.Now.Second) * 1000 - DateTime.Now.Millisecond);
            taskTimer.Elapsed += new ElapsedEventHandler(timerAction);
            taskTimer.Start();
        }

        // Reads previously saved tasks from the text file
        private void readTasks()
        {

            // Test if printFile exists. If it does not exist, then create it.
            try
            {
                string[] stringTasks = File.ReadAllLines(printFile); // Store tasks in array from the text file

                foreach (string line in stringTasks) // Iterate through tasks
                {
                    string[] tempInfo = Regex.Split(line, "<<@>>"); // Separator for task properties
                    try
                    {
                        Task tempTask = new Task(tempInfo[0], DateTime.Parse(tempInfo[1]), tempInfo[2], tempInfo[3]); // Store task in temp variable
                        if (tempTask.getTime() > DateTime.Now) // If task is in the future, then add to taskList
                        {
                            taskList.Add(tempTask);
                        }
                    }
                    catch (System.FormatException) // Catch FormatException
                    {
                        MessageBox.Show("The Tasks.txt file has been corrupted. Please erase or fix the file and restart the program");
                        Close();
                    }
                    catch (System.IndexOutOfRangeException) // catch IndexOutOfRangeException
                    {
                        MessageBox.Show("The Tasks.txt file has been corrupted. Please erase or fix the file and restart the program");
                        Close();
                    }
                }
                taskList.Sort(); // Sort tasks
            }
            catch (FileNotFoundException) // If file does not exist, create StreamWriter
            {
                StreamWriter sw = new StreamWriter(printFile);
                sw.Close();
            }
            catch (IOException ex) // Catch IO exception
            {
                MessageBox.Show(ex.Message);
            }

        }

        // Saves task to text file
        private void writeTasks() 
        {
            try
            {
                StreamWriter sw = new StreamWriter(printFile); // Create StreamWriter
                foreach (Task t in taskList) // Iterate through tasks
                {
                    string temp = t.getTitle() + "<<@>>" + t.getTime() + "<<@>>" + t.getNotes() + "<<@>>" + t.getCategory();// Store task in temp variable
                    sw.WriteLine(temp);// Write task to file
                }
                sw.Close(); // Close StreamWriter
            }
            catch (IOException ex) // Input Output exception
            {
                MessageBox.Show("Could not create task file. Nothing will be saved this session.  Error: " + ex.Message);
            }
        }

        // Generate next 30 calendar days for the calendar
        private void generateDates(int amount, DateTime now)
        {
            for (int i = 0; i <= amount; i++) // Iterate over next 30 days
            {
                DateDisplay tempDate = new DateDisplay(now.ToShortDateString()); // Create temp date
                tempDate.Width = 250;
                dateDisplayList.Add(tempDate); // Add to display list
                AllTasksDisplay.Children.Add(tempDate); //Add to all tasks

                foreach (Task t in taskList) // Iterate through tasks
                {
                    if (t.getTime().ToShortDateString() == now.ToShortDateString()) // If current time is equal to task time
                    {
                        TaskDisplay tempTaskDisplay = new TaskDisplay(t);
                        tempTaskDisplay.editButton.Click += (object sender, RoutedEventArgs e) => // When edit button has been clicked
                        {
                            if (!editToggle) // If editToggle is false, this block executes because !false is true
                            {
                                EditDisplay w = new EditDisplay(this, t); // Open a new edit window
                                w.Show();
                                editToggle = true; // Turn editing on
                            }
                        };
                        tempTaskDisplay.deleteButton.Click += (object sender, RoutedEventArgs e) => //When delete button clicked
                        {
                            taskList.Remove(t); // Remove the task from the list
                            updateView(); // Update the view
                        };
                        tempDate.itemPanel.Children.Add(tempTaskDisplay);
                        taskDisplayList.Add(tempTaskDisplay);
                    }
                }

                now = now.AddDays(1);
            }
            Button moreDaysButton = new Button(); // Button that shows more days in the calendar view
            moreDaysButton.Content = "Show More Days";
            moreDaysButton.Click += MoreDaysButton_Click;
            AllTasksDisplay.Children.Add(moreDaysButton);

        }

        // Adds more days to calendar upon button click
        private void MoreDaysButton_Click(object sender, RoutedEventArgs e)
        {
            daysInFuture += 30; // Cumulatively keep track of daysInFuture because the button can be clicked as many times as desired
            updateView();
        }

        // Handles the timer which updates automatically
        private void timerAction(object source, ElapsedEventArgs e)
        {
            try
            {
                CurrentTime.Dispatcher.Invoke(timerUpdate);
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void timerUpdate()
        {
            DateTime now = DateTime.Now;
            CurrentTime.Text = now.ToShortDateString() + ": " + now.ToShortTimeString();

            updateView();
        }

        // Determines interval for when the timer will update next
        private void timerIntervalUpdate()
        {
            taskTimer.Interval = (60 - DateTime.Now.Second) * 1000 - DateTime.Now.Millisecond;            
        }

        // Brings up new task window when add task button is clicked
        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (!editToggle)
            {
                EditDisplay w = new EditDisplay(this);
                w.Show();
                editToggle = true;
            }
        }

        // Adds the task to a list if the time specified is in the future
        public void addTaskToList(Task t)
        {
            if (t.getTime() > DateTime.Now)
            {
                taskList.Add(t);
                updateView();
            }
            else
            {
                MessageBox.Show("Cannot add tasks before current datetime.");
            }
        }

        // Stops timer, clears lists, clears past tasks, writes tasks to window, generates new calendar, and restarts timer
        public void updateView()
        {
            taskTimer.Stop();
            taskDisplayList.Clear();
            dateDisplayList.Clear();
            AllTasksDisplay.Children.Clear();            

            ClearPastTasks();
            taskList.Sort();
            writeTasks();

            generateDates(daysInFuture, DateTime.Now);           
            timerIntervalUpdate();
            taskTimer.Start();
        }

        // Iterates through task list and removes all task objects
        private void ClearPastTasks()
        {
            for (int i = taskList.Count - 1; i >= 0; i--)
            {
                if (taskList[i].getTime() < DateTime.Now)
                {
                    taskList.RemoveAt(i);
                }
            }
        }

        // Prints report on button click
        private void PrintReport_Click(object sender,RoutedEventArgs e)
        {
            PrintDisplay p = new PrintDisplay(this);
            p.Show();
        }

        public List<Task> getTaskList()
        {
            return taskList;
        }

    }
}
