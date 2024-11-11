using System.Runtime.Serialization;

namespace Client.Models
{
    [DataContract]
    public class BookViewModel
    {

        [DataMember]
        public string? Id { get; set; }
        [DataMember]
        public string? Name { get; set; }

        [DataMember]
        public double? Price { get; set; }

        [DataMember]
        public uint? Quantity { get; set; }
    }
}
