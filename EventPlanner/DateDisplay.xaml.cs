using System.Windows.Controls;


namespace EventPlanner
{
    /// <summary>
    /// Interaction logic for DateDisplay.xaml
    /// </summary>
    public partial class DateDisplay : UserControl
    {
        // Constructor that takes a date as an argument to set for the display
        public DateDisplay(string date)
        {
            InitializeComponent();
            dateText.Text = date;
        }
    }
}
