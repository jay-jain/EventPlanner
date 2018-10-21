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
        public bool editToggle;
        private Timer taskTimer; 
        private List<DateDisplay> dateDisplayList;
        private List<Task> taskList;
        private List<TaskDisplay> taskDisplayList;
        private int daysInFuture;
        private string ioFile;


        public MainWindow()
        {
            ioFile = "Tasks.txt";
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

            //need to test for ioFile existing here, create it if it doesn't.
            try
            {
                string[] stringTasks = File.ReadAllLines(ioFile);

                foreach (string line in stringTasks)
                {
                    string[] tempInfo = Regex.Split(line, "<<@>>");
                    try
                    {
                        Task tempTask = new Task(tempInfo[0], DateTime.Parse(tempInfo[1]), tempInfo[2]);
                        if (tempTask.getTime() > DateTime.Now)
                        {
                            taskList.Add(tempTask);
                        }
                    }
                    catch (System.FormatException)
                    {
                        MessageBox.Show("The Tasks.txt file has been corrupted. Please erase or fix the file and restart the program");
                        Close();
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        MessageBox.Show("The Tasks.txt file has been corrupted. Please erase or fix the file and restart the program");
                        Close();
                    }
                }
                taskList.Sort();
            }
            catch (FileNotFoundException)
            {
                StreamWriter sw = new StreamWriter(ioFile);
                sw.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void WriteEvents()
        {
            try
            {
                StreamWriter sw = new StreamWriter(ioFile);
                foreach (Task t in taskList)
                {
                    string temp = t.getTitle() + "<<@>>" + t.getTime() + "<<@>>" + t.getNotes();
                    sw.WriteLine(temp);
                }
                sw.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Could not create task file, nothing will be saved this session.  Error: " + ex.Message);
            }
        }

        private void generateDates(int amount, DateTime now)
        {
            for (int i = 0; i <= amount; i++)
            {
                DateDisplay tempDate = new DateDisplay(now.ToShortDateString());
                tempDate.Width = 250;
                dateDisplayList.Add(tempDate);
                AllTasksDisplay.Children.Add(tempDate);

                foreach (Task t in taskList)
                {
                    if (t.getTime().ToShortDateString() == now.ToShortDateString())
                    {
                        TaskDisplay tempTaskDisplay = new TaskDisplay(t);
                        tempTaskDisplay.editButton.Click += (object sender, RoutedEventArgs e) =>
                        {
                            if (!editToggle)
                            {
                                EditDisplay w = new EditDisplay(this, t);
                                w.Show();
                                editToggle = true;
                            }
                        };
                        tempTaskDisplay.deleteButton.Click += (object sender, RoutedEventArgs e) =>
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
            daysInFuture += 30;
            updateView();
        }

        private void populateIncoming(DateTime now)
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
                //writeEvents();
                updateView();
            }
            else
            {
                MessageBox.Show("Cannot add tasks before current date-time");
            }
        }

        public void updateView()
        {
            taskTimer.Stop();
            taskDisplayList.Clear();
            dateDisplayList.Clear();
            AllTasksDisplay.Children.Clear();
            UpcomingTasksDisplay.Children.Clear();

            ClearPastEvents();
            taskList.Sort();
            WriteEvents();

            generateDates(daysInFuture, DateTime.Now);
            populateIncoming(DateTime.Now);
            timerIntervalUpdate();
            taskTimer.Start();
        }

        private void ClearPastEvents()
        {
            for (int i = taskList.Count - 1; i >= 0; i--)
            {
                if (taskList[i].getTime() < DateTime.Now)
                {
                    taskList.RemoveAt(i);
                }
            }
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            //try {
            //    StreamWriter sw = new StreamWriter("EventReport.txt", true);
            //    sw.WriteLine("Event Report as of " + DateTime.Now.ToLongDateString());
            //    sw.WriteLine("");
            //    foreach (DEvent de in taskList)
            //    {
            //        //string temp = de.getName() + "<<@>>" + de.getTime() + "<<@>>" + de.getNotes();
            //        //sw.WriteLine(temp);
            //        sw.WriteLine("Event: " + de.getName());
            //        sw.WriteLine("Occurs: " + de.getTime().ToLongDateString() + " " + de.getTime().ToLongTimeString());
            //        sw.WriteLine("Notes: " + de.getNotes());
            //        sw.WriteLine("");
            //    }
            //    sw.Close();
            //    MessageBox.Show("File EventReport.txt saved to same directory as program.");
            //} catch (IOException ex)
            //{
            //    MessageBox.Show("Cound not write file.  Error:" + ex.Message);
            //}

            String printMessage = "";
            printMessage += "Task Report on " + DateTime.Now.ToLongDateString();
            printMessage += "\r\n";
            foreach (Task t in taskList)
            {
                //string temp = de.getName() + "<<@>>" + de.getTime() + "<<@>>" + de.getNotes();
                //sw.WriteLine(temp);
                printMessage += "Task: " + t.getTitle();
                printMessage += "\r\n";
                printMessage += "Time: " + t.getTime().ToLongDateString() + " " + t.getTime().ToLongTimeString();
                printMessage += "\r\n";
                printMessage += "Notes: " + t.getNotes();
                printMessage += "\r\n";
                printMessage += "\r\n";
            }

            FlowDocument fd = new FlowDocument(new Paragraph(new Run(printMessage)));
            fd.PagePadding = new Thickness(100);
            IDocumentPaginatorSource idpSource = fd;

            PrintDialog dialog = new PrintDialog();

            dialog.ShowDialog();
            dialog.PrintDocument(idpSource.DocumentPaginator, "Task Report");

        }

    }
}
