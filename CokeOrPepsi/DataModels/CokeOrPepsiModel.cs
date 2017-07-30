using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CokeOrPepsi.DataModels
{
    class CokeOrPepsiModel
    {
        [Newtonsoft.Json.JsonProperty("Id")]
        public string Id { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }

        public DateTime DateNZ { get; set; }

        public string Product { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string DateDisplay { get { return DateNZ.ToLocalTime().ToString("d"); } }

        [Newtonsoft.Json.JsonIgnore]
        public string TimeDisplay { get { return DateNZ.ToLocalTime().ToString("t"); } }
    }
}
