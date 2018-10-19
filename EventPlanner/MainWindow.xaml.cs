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
        private List<Task> eventList;
        private List<TaskDisplay> taskDisplayList;
        private int daysInFuture;
        private string ioFile;


        public MainWindow()
        {
            ioFile = "events.txt";
            daysInFuture = 30;
            dateDisplayList = new List<DateDisplay>();
            eventList = new List<Task>();
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
                string[] stringEvents = File.ReadAllLines(ioFile);

                foreach (string line in stringEvents)
                {
                    string[] tempInfo = Regex.Split(line, "<<@>>");
                    try
                    {
                        Task tempEvent = new Task(tempInfo[0], DateTime.Parse(tempInfo[1]), tempInfo[2]);
                        if (tempEvent.getTime() > DateTime.Now)
                        {
                            eventList.Add(tempEvent);
                        }
                    }
                    catch (System.FormatException)
                    {
                        MessageBox.Show("events.txt Data file has been corrupted.  Please erase or fix file and restart program");
                        Close();
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        MessageBox.Show("events.txt Data file has been corrupted.  Please erase or fix file and restart program");
                        Close();
                    }
                }
                eventList.Sort();
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
                foreach (Task t in eventList)
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

                foreach (Task t in eventList)
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
                            eventList.Remove(t);
                            updateView();
                        };
                        tempDate.itemPanel.Children.Add(tempTaskDisplay);
                        taskDisplayList.Add(tempTaskDisplay);
                    }
                }

                now = now.AddDays(1);
            }
            Button moreDaysButton = new Button();
            moreDaysButton.Content = "More days...";
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
            if (eventList.Count() > 0) UpcomingTasksDisplay.Children.Add(new UpcomingDisplay(eventList[0]));
            if (eventList.Count() > 1) UpcomingTasksDisplay.Children.Add(new UpcomingDisplay(eventList[1]));

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

        public void addEventToList(Task t)
        {
            if (t.getTime() > DateTime.Now)
            {
                eventList.Add(t);
                //eventList.Sort();
                //writeEvents();
                updateView();
            }
            else
            {
                MessageBox.Show("Cannot add events before current date-time");
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
            eventList.Sort();
            WriteEvents();

            generateDates(daysInFuture, DateTime.Now);
            populateIncoming(DateTime.Now);
            timerIntervalUpdate();
            taskTimer.Start();
        }

        private void ClearPastEvents()
        {
            for (int i = eventList.Count - 1; i >= 0; i--)
            {
                if (eventList[i].getTime() < DateTime.Now)
                {
                    eventList.RemoveAt(i);
                }
            }
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            //try {
            //    StreamWriter sw = new StreamWriter("EventReport.txt", true);
            //    sw.WriteLine("Event Report as of " + DateTime.Now.ToLongDateString());
            //    sw.WriteLine("");
            //    foreach (DEvent de in eventList)
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
            foreach (Task t in eventList)
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

            //Console.Out.WriteLine("Print Pressed");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
