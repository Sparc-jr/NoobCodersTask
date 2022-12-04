using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoElastic
{
    public class Post
    {
        private int fieldsCount;
        private List<int> fieldsToIndex;
        private List<object> fields;

        public static List<bool> FieldsToIndex;
        public static int FieldsCount;
        public static List<string> namesOfFields;
        public static List<Type> typesOfFields;
        public List<object> Fields { get; set; }

        public Post()
        {
            Fields = new List<object>();            
        }
    }

    public class Record
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Record(int n, string text)
        {
            Id = n;
            Text = text;
        }
    }
}
