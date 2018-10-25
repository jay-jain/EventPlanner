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
    /// Interaction logic for LoadedEventObject.xaml
    /// </summary>
    public partial class TaskDisplay : UserControl
    {
        Task t;
        TimeSpan timespan;

        public TaskDisplay(Task t)
        {
            this.t = t;

            InitializeComponent();

            updateElements();
        }

        public void updateElements() // Update all task properties
        {
            taskName.Text = t.getTitle();
            taskNotes.Text = "Notes: " + t.getNotes();
            dateText.Text = t.getTime().ToShortDateString();
            timeText.Text = t.getTime().ToShortTimeString();
            categoryText.Text = "Category: " + t.getCategory();
            updateRelativeDueTime(DateTime.Now);
            Console.WriteLine(categoryText.Text);
            if(categoryText.Text =="High Priority")
            {
                panel.Background = new SolidColorBrush(Colors.LightPink);
            }else if(categoryText.Text == "Medium Priority")
            {
                panel.Background = new SolidColorBrush(Colors.LightYellow);
            }
            else
            {
                panel.Background = new SolidColorBrush(Colors.LightGreen);
            }
        }

        public void updateRelativeDueTime(DateTime now)
        {
            timespan = t.getTime() - now; // Calculate timespan between current time and when task is due
            relativeDueText.Text = "Due in: " + timespan.Days.ToString() + " Days, " + timespan.Hours.ToString() + " Hours and " + timespan.Minutes.ToString() + " minutes";
        }

        public Task getTask()
        {
            return t;
        }

        public void setTask(Task t)
        {
            this.t = t;
        }
    }
}
