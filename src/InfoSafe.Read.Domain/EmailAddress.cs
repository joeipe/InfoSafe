using System.ComponentModel.DataAnnotations.Schema;

namespace InfoSafe.Read.Domain
{
    [Table("Main.EmailAddresses")]
    public class EmailAddress
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string Email { get; set; } = null!;
    }
}