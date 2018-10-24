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
        public bool editToggle;
        private Timer taskTimer; 
        private List<DateDisplay> dateDisplayList;
        private List<Task> taskList;
        private List<TaskDisplay> taskDisplayList;
        private int daysInFuture;
        private string printFile;


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

            ReadTasks(); 

            generateDates(daysInFuture, DateTime.Now);

            populateIncoming(DateTime.Now);

            CurrentTime.Text = DateTime.Now.ToShortDateString() + ": " + DateTime.Now.ToShortTimeString();

            ///TaskTimer = new Timer(60000);
            taskTimer = new Timer((60 - DateTime.Now.Second) * 1000 - DateTime.Now.Millisecond);
            taskTimer.Elapsed += new ElapsedEventHandler(timerAction);
            taskTimer.Start();
        }

        private void ReadTasks()
        {

            // Test if printFile exists. If it does not exist, then create it.
            try
            {
                string[] stringTasks = File.ReadAllLines(printFile); // Store tasks in array

                foreach (string line in stringTasks) // Iterate through tasks
                {
                    string[] tempInfo = Regex.Split(line, "<<@>>");
                    try
                    {
                        Task tempTask = new Task(tempInfo[0], DateTime.Parse(tempInfo[1]), tempInfo[2]); // Store task in temp variable
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

        private void WriteTasks() 
        {
            try
            {
                StreamWriter sw = new StreamWriter(printFile); // Create StreamWriter
                foreach (Task t in taskList) // Iterate through tasks
                {
                    string temp = t.getTitle() + "<<@>>" + t.getTime() + "<<@>>" + t.getNotes();// Store task in temp variable
                    sw.WriteLine(temp);// Write task to file
                }
                sw.Close(); // Close StreamWriter
            }
            catch (IOException ex) // Catch IOException
            {
                MessageBox.Show("Could not create task file. Nothing will be saved this session.  Error: " + ex.Message);
            }
        }

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
                            if (!editToggle) // If editing is false, because !false is true
                            {
                                EditDisplay w = new EditDisplay(this, t); // Open a new edit window
                                w.Show();
                                editToggle = true;
                            }
                        };
                        tempTaskDisplay.deleteButton.Click += (object sender, RoutedEventArgs e) => //When delete button clicked
                        {
                            taskList.Remove(t);
                            updateView();
                        };
                        tempDate.itemPanel.Children.Add(tempTaskDisplay);
                        taskDisplayList.Add(tempTaskDisplay);
                    }
                }

                now = now.AddDays(1);
            }
            Button moreDaysButton = new Button();
            moreDaysButton.Content = "Show More Days";
            moreDaysButton.Click += MoreDaysButton_Click;
            AllTasksDisplay.Children.Add(moreDaysButton);

        }

        private void MoreDaysButton_Click(object sender, RoutedEventArgs e)
        {
            daysInFuture += 30; // Cumulatively keep track of daysInFuture because the button can be clicked many times
            updateView();
        }

        private void populateIncoming(DateTime now) // Add upcoming tasks to the view
        {
            if (taskList.Count() > 0) UpcomingTasksDisplay.Children.Add(new UpcomingDisplay(taskList[0]));
            if (taskList.Count() > 1) UpcomingTasksDisplay.Children.Add(new UpcomingDisplay(taskList[1]));

        }

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
            //foreach (EventView ev in taskDisplayList)
            //{
            //    ev.updateRelativeDueTime(now);
            //}
            //foreach (IncomingView iv in dueListView.Children)
            //{
            //    iv.updateRelativeDueTime(now);
            //}
            updateView();
        }

        private void timerIntervalUpdate()
        {
            taskTimer.Interval = (60 - DateTime.Now.Second) * 1000 - DateTime.Now.Millisecond;
            //Console.Out.WriteLine(TaskTimer.Interval);
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (!editToggle)
            {
                EditDisplay w = new EditDisplay(this);
                w.Show();
                editToggle = true;
            }
        }

        public void addTaskToList(Task t)
        {
            if (t.getTime() > DateTime.Now)
            {
                taskList.Add(t);
                //taskList.Sort();
                //WriteTasks();
                updateView();
            }
            else
            {
                MessageBox.Show("Cannot add tasks before current datetime.");
            }
        }

        public void updateView()
        {
            taskTimer.Stop();
            taskDisplayList.Clear();
            dateDisplayList.Clear();
            AllTasksDisplay.Children.Clear();
            UpcomingTasksDisplay.Children.Clear();

            ClearPastTasks();
            taskList.Sort();
            WriteTasks();

            generateDates(daysInFuture, DateTime.Now);
            populateIncoming(DateTime.Now);
            timerIntervalUpdate();
            taskTimer.Start();
        }

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

        private void PrintReport_Click(object sender,RoutedEventArgs e)
        {
            PrintDisplay p = new PrintDisplay(this);
            p.Show();
        }

        public List<Task> getTaskList()
        {
            return taskList;
        }

        //private void PrintReport_Click(object sender, RoutedEventArgs e)
        //{
        //    //try {
        //    //    StreamWriter sw = new StreamWriter("EventReport.txt", true);
        //    //    sw.WriteLine("Event Report as of " + DateTime.Now.ToLongDateString());
        //    //    sw.WriteLine("");
        //    //    foreach (DEvent de in taskList)
        //    //    {
        //    //        //string temp = de.getName() + "<<@>>" + de.getTime() + "<<@>>" + de.getNotes();
        //    //        //sw.WriteLine(temp);
        //    //        sw.WriteLine("Event: " + de.getName());
        //    //        sw.WriteLine("Occurs: " + de.getTime().ToLongDateString() + " " + de.getTime().ToLongTimeString());
        //    //        sw.WriteLine("Notes: " + de.getNotes());
        //    //        sw.WriteLine("");
        //    //    }
        //    //    sw.Close();
        //    //    MessageBox.Show("File EventReport.txt saved to same directory as program.");
        //    //} catch (IOException ex)
        //    //{
        //    //    MessageBox.Show("Cound not write file.  Error:" + ex.Message);
        //    //}

        //    String printMessage = "";
        //    printMessage += "Task Report on " + DateTime.Now.ToLongDateString();
        //    printMessage += "\r\n";
        //    foreach (Task t in taskList) // Iterate throught tasks
        //    {
        //        //string temp = de.getName() + "<<@>>" + de.getTime() + "<<@>>" + de.getNotes();
        //        //sw.WriteLine(temp);
        //        // Create printMessage string
        //        printMessage += "Task: " + t.getTitle();
        //        printMessage += "\r\n";
        //        printMessage += "Time: " + t.getTime().ToLongDateString() + " " + t.getTime().ToLongTimeString();
        //        printMessage += "\r\n";
        //        printMessage += "Notes: " + t.getNotes();
        //        printMessage += "\r\n";
        //        printMessage += "\r\n";
        //    }

        //    FlowDocument fd = new FlowDocument(new Paragraph(new Run(printMessage)));
        //    fd.PagePadding = new Thickness(100);
        //    IDocumentPaginatorSource idpSource = fd;

        //    PrintDialog dialog = new PrintDialog();

        //    dialog.ShowDialog();
        //    dialog.PrintDocument(idpSource.DocumentPaginator, "Task Report");

        //}

    }
}
