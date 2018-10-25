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



        private void printReport(String report)
        {
                FlowDocument fd = new FlowDocument(new Paragraph(new Run(report)));
                fd.PagePadding = new Thickness(100);
                IDocumentPaginatorSource idpSource = fd;

                PrintDialog dialog = new PrintDialog();

                dialog.ShowDialog();
                dialog.PrintDocument(idpSource.DocumentPaginator, "Task Report");
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRange.IsChecked == true)
            {
                printReport(getSpecifiedTasks(startDate.SelectedDate.Value,endDate.SelectedDate.Value));
            }else if (allTasks.IsChecked == true)
            {
                printReport(getAllTasks());
            }else if(printCat.IsChecked == true)
            {
                printReport(getCatTasks());
            }else
            {
                MessageBox.Show("Please select a Print Report option.");
            }
        }
    }

    
}
