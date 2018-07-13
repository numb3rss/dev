using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class StorageTask
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public List<Service> Services { get; set; }
        
        public bool IsSuccess { get; set; }
    }
}
