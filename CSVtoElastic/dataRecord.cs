using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoElastic
{
    internal class dataRecord
    {
        public int Id { get; set; }
        public List<object> Data { get; set; }
    }
}
