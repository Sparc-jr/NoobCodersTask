using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Noobcoders
{
    public class Post
    {
        
        private int _id;
        private int fieldsCount;
        private List<object> fields;

        public static int FieldsCount;
        public static List<string> namesOfFields;
        public static List<Type> typesOfFields;

        public int PostID { get; private set; }
        
        public List<object> Fields { get; set; }
              
        public Post()
        {
            PostID = Interlocked.Increment(ref _id);
            Fields = new List<object>();
        }
    }
}
