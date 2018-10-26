using System;

namespace EventPlanner
{
    public class Task:IComparable<Task>
    {
        // Task property variables
        private string title;
        private DateTime time;
        private string notes;
        private string category;

        // Constructor
        public Task(String title, DateTime time, String notes, String category)
        {
            this.title = title;
            this.time = time;
            this.notes = notes;
            this.category = category;
        }

        // Setter methods 
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

        public void setCategory(String category)
        {
            this.category = category;
        }

        // Getter methods
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

        public string getCategory()
        {
            return this.category;
        }

        
        public int CompareTo(Task next)
        {
            return time.CompareTo(next.time);
        }
    }
}
