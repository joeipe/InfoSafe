using System.ComponentModel.DataAnnotations.Schema;

namespace InfoSafe.Read.Domain
{
    [Table("Main.Contacts")]
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DoB { get; set; }

        public Address? Address { get; set; }
        public List<EmailAddress>? EmailAddresses { get; set; }
        public PhoneNumber? PhoneNumber { get; set; }
    }
}