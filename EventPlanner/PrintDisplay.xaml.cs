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

        public PrintDisplay(MainWindow mw)
        {
            InitializeComponent();
        }

        private List<Task> getSpecifiedTasks(DateTime startDate, DateTime endDate)
        {
            return taskList;
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void allTasks_Checked(object sender, RoutedEventArgs e)
        {

        }
    }

    
}
