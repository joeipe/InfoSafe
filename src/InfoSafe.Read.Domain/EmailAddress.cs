namespace InfoSafe.Read.Domain
{
    public class EmailAddress
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string Email { get; set; } = null!;
    }
}