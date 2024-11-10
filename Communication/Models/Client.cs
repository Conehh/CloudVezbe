using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Models
{
    [DataContract]
    public class Client
    {
        [DataMember]
        public string? Id { get; set; }

        [DataMember]
        public string? Name { get; set; }

        [DataMember]
        public double? Balance { get; set; }
    }
}
