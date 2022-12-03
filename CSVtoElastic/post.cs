using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CSVtoElastic
{
    public class Posts
    {

        static int nextID;
        private int postID;
        private string ?text;
        private DateTime createdDate;
        private string ?rubrics;

        public string Text { get { return text; } set { text = value; } }
        public DateTime CreatedDate { get { return createdDate; } set { createdDate = value; } }
        public string Rubrics { get { return rubrics; } set { rubrics = value; } }

        public int PostID { get; private set; }
        
        public Posts()
        {
            PostID = Interlocked.Increment(ref nextID);           
        }
    }

}
