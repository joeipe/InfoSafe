using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace InfoSafe.Write.Domain
{
    public class Address : Entity, IEntityTypeConfiguration<Address>
    {
        public int ContactId { get; set; }
        public string Address1 { get; set; } = null!;
        public string? Address2 { get; set; } = null!;
        public string? Address3 { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string PostalCode { get; set; } = null!;

        public Contact Contact { get; set; } = null!;

        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder
                .HasOne(e => e.Contact)
                .WithOne(e => e.Address)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.Address1)
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(e => e.Address2)
                .HasMaxLength(120);

            builder.Property(e => e.Address3)
                .HasMaxLength(120);

            builder.Property(e => e.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.State)
                .HasColumnType("CHAR(3)")
                .IsRequired();

            builder.Property(e => e.Country)
                .HasColumnType("CHAR(2)")
                .IsRequired();

            builder.Property(e => e.PostalCode)
                .HasMaxLength(16)
                .IsRequired();
        }
    }
}