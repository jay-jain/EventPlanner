using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace EventPlanner
{
    /// <summary>
    /// Interaction logic for LoadedEventObject.xaml
    /// </summary>
    public partial class TaskDisplay : UserControl
    {
        Task t;
        TimeSpan timespan;

        // TaskDisplay constructor
        public TaskDisplay(Task t)
        {
            this.t = t;
            InitializeComponent();
            updateElements();
        }

        // Updates elements when task has been 
        public void updateElements() 
        {
            // Sets task properties
            taskName.Text = t.getTitle();
            taskNotes.Text = "Notes: " + t.getNotes();
            dateText.Text = t.getTime().ToShortDateString();
            timeText.Text = t.getTime().ToShortTimeString();
            categoryText.Text = t.getCategory();
            
            updateRelativeDueTime(DateTime.Now);

            // Changes task color based on the category type
            if(categoryText.Text == "High Priority")
            {
                panel.Background = new SolidColorBrush(Colors.LightPink);
            }else if(categoryText.Text == "Medium Priority")
            {
                panel.Background = new SolidColorBrush(Colors.LightYellow);
            }
            else if(categoryText.Text == "Low Priority")
            {
                panel.Background = new SolidColorBrush(Colors.LightGreen);
            }
            categoryText.Text = "Category: " + t.getCategory(); // Updates category string for more readable format
        }

        // Updates relative time based on current time and the time the task is due
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
