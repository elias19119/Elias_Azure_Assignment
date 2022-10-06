using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideProgrammingAssignment.Model
{
    public class ColorApi
    {
        public ApiColorDTO Name { get; set; }
    }

    public class ApiColorDTO
    {
        public string Value { get; set; }
    }
}
