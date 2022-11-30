using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoElastic
{
    public class Post
    {

        //private int _id;
        private int fieldsCount;
        private List<int> fieldsToIndex;
        private List<object> fields;

        public static List<bool> FieldsToIndex;
        public static int FieldsCount;
        public static List<string> namesOfFields;
        public static List<Type> typesOfFields;

        public int PostID { get; private set; }

        public List<object> Fields { get; set; }

        public Post()
        {
            //PostID = Interlocked.Increment(ref _id);
            Fields = new List<object>();            
        }
    }
}
