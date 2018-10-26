using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


namespace EventPlanner
{
    /// <summary>
    /// Interaction logic for PrintDisplay.xaml
    /// </summary>
    public partial class PrintDisplay : Window
    {
        private List<Task> taskList;
        MainWindow mw;

        public PrintDisplay(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
            this.taskList = mw.getTaskList(); ;
        }

        /* Obtains tasks within a given date range*/
        private String getSpecifiedTasks(DateTime startDate, DateTime endDate)
        {
                String printMessage = "";
                printMessage += "Task Report between " + startDate.Date.ToShortDateString() + " and " + endDate.Date.ToShortDateString();
                printMessage += "\n\n";
                foreach (Task t in taskList) // Iterate throught tasks
                {
                    if (t.getTime().Date >= startDate.Date & t.getTime().Date <= endDate.Date)
                    {
                        // Create printMessage string
                        printMessage += "Task: " + t.getTitle();
                        printMessage += "\r\n";
                        printMessage += "Time: " + t.getTime().ToLongDateString() + " " + t.getTime().ToLongTimeString();
                        printMessage += "\r\n";
                        printMessage += "Notes: " + t.getNotes();
                        printMessage += "\r\n";
                        printMessage += "Category: " + t.getCategory();
                        printMessage += "\r\n";
                        printMessage += "\r\n";
                    }
                    
                }
            return printMessage;
        }

        /*Gets a list of all tasks*/
        private String getAllTasks()
        {
                String printMessage = "";
                printMessage += "Task Report for all future tasks";
                printMessage += "\n\n";
                foreach (Task t in taskList) // Iterate throught tasks
                {
                    // Create printMessage string
                    printMessage += "Task: " + t.getTitle();
                    printMessage += "\r\n";
                    printMessage += "Time: " + t.getTime().ToLongDateString() + " " + t.getTime().ToLongTimeString();
                    printMessage += "\r\n";
                    printMessage += "Notes: " + t.getNotes();
                    printMessage += "\r\n";
                    printMessage += "Category: " + t.getCategory();
                    printMessage += "\r\n";
                    printMessage += "\r\n";
                }
            return printMessage;
        }

        /* Gets a list of tasks from a given category*/
        private String getCatTasks()
        {
            String printMessage = "";
            printMessage += "Task Report for " + listBox.Text + " category." ;
            printMessage += "\n\n";
            foreach (Task t in taskList) // Iterate throught tasks
            {
                if (t.getCategory() == listBox.Text)
                {
                    // Create printMessage string
                    printMessage += "Task: " + t.getTitle();
                    printMessage += "\r\n";
                    printMessage += "Time: " + t.getTime().ToLongDateString() + " " + t.getTime().ToLongTimeString();
                    printMessage += "\r\n";
                    printMessage += "Notes: " + t.getNotes();
                    printMessage += "\r\n";
                    printMessage += "Category: " + t.getCategory();
                    printMessage += "\r\n";
                    printMessage += "\r\n";
                }
            }

                return printMessage;
        }


        // Prints a given report. 
        private void printReport(String report)
        {
                FlowDocument fd = new FlowDocument(new Paragraph(new Run(report)));
                fd.PagePadding = new Thickness(100);
                IDocumentPaginatorSource idpSource = fd;

                PrintDialog dialog = new PrintDialog();

                dialog.ShowDialog();
                dialog.PrintDocument(idpSource.DocumentPaginator, "Task Report");

        }

        // Prints the given report based on user input
        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRange.IsChecked == true) // Generates report for date range print option
            {
                if(startDate.SelectedDate == null | endDate.SelectedDate == null) // Checks for no date selection
                {
                    MessageBox.Show("Please select a valid start and end date.");
                }
                else
                {
                    printReport(getSpecifiedTasks(startDate.SelectedDate.Value, endDate.SelectedDate.Value));
                    this.Close();
                }
                
            }else if (allTasks.IsChecked == true){
                printReport(getAllTasks()); // Generates report for all tasks print option
                this.Close();
            }
            else if(printCat.IsChecked == true)
            {
                if(listBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a category.");
                }
                else
                {
                    printReport(getCatTasks());// Generates report for category print option
                    this.Close();
                }
                
            }
            else
            {
                MessageBox.Show("Please select a Print Report option.");
            }
        }
    }

    
}
