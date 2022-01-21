using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class SWebhook : ShopifyObject
    {

        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("fields")]
        public IEnumerable<string> Fields { get; set; }
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("metafield_namespaces")]
        public IEnumerable<string> MetafieldNamespaces { get; set; }
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
   
}
