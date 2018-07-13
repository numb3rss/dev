using System;
using System.Collections.Generic;
using System.Text;

namespace Tasks.BO
{
    public class AppSettingsModel
    {
        public string ConnectionString { get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string StorageQueueName { get; set; }
    }
}
