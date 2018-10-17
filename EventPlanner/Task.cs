using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlanner
{
    public class Task:IComparable<Task>
    {

        private string title;
        private DateTime time;
        private string notes;

        public Task(String title, DateTime time, String notes)
        {
            this.title = title;
            this.time = time;
            this.notes = notes;

        }

        public void setTitle(String title)
        {
            this.title = title;
        }

        public void setTime(DateTime time)
        {
            this.time = time;
        }

        public void setNotes(String notes)
        {
            this.notes = notes;
        }

        public string getTitle()
        {
            return this.title;
        }

        public DateTime getTime()
        {
            return this.time;
        }

        public string getNotes()
        {
            return this.notes;
        }

        public int CompareTo(Task next)
        {
            return time.CompareTo(next.time);
        }
    }
}
