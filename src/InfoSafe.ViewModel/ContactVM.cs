namespace InfoSafe.ViewModel
{
    public class ContactVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string DoB { get; set; }

        public AddressVM? Address { get; set; }
        public List<EmailAddressVM>? EmailAddresses { get; set; }
        public PhoneNumberVM? PhoneNumber { get; set; }
    }
}