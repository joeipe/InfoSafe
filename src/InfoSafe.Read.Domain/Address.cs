using System.ComponentModel.DataAnnotations.Schema;

namespace InfoSafe.Read.Domain
{
    [Table("Main.Addresses")]
    public class Address
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string Address1 { get; set; } = null!;
        public string? Address2 { get; set; } = null!;
        public string? Address3 { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
    }
}