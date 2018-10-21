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

namespace EventPlanner
{
    /// <summary>
    /// Interaction logic for UpcomingDisplay.xaml
    /// </summary>
    public partial class UpcomingDisplay : UserControl
    {
        Task t;

        public UpcomingDisplay(Task t) // Constructor that takes Task object; sets task name and updates relative due date
        {
            InitializeComponent();
            this.t = t;
            taskName.Text = t.getTitle();
            UpdateRelativeDueTime(DateTime.Now);
        }

        internal void UpdateRelativeDueTime(DateTime now)
        {
            TimeSpan timespan = t.getTime() - now; // Calculate time till task is due
            dueTime.Text = "Due: " + timespan.Days.ToString() + "d, " + timespan.Hours.ToString() + "h, " + timespan.Minutes.ToString() + "m";
        }
    }
}
