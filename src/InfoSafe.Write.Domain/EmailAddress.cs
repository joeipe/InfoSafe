using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace InfoSafe.Write.Domain
{
    public class EmailAddress : Entity, IEntityTypeConfiguration<EmailAddress>
    {
        public int ContactId { get; set; }
        public string Email { get; set; } = null!;

        public Contact Contact { get; set; } = null!;

        public void Configure(EntityTypeBuilder<EmailAddress> builder)
        {
            builder
                .HasOne(e => e.Contact)
                .WithMany(e => e.EmailAddresses)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.Email)
                .HasMaxLength(320)
                .IsRequired();
        }
    }
}