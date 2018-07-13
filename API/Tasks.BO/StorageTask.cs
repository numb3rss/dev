using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tasks.BO
{
    public class StorageTask
    {
        [JsonProperty("id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("services")]
        public List<Service> Services { get; set; }

        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
    }
}
