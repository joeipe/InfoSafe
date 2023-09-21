using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace InfoSafe.Write.Domain
{
    public class PhoneNumber : Entity, IEntityTypeConfiguration<PhoneNumber>
    {
        public int ContactId { get; set; }
        public string Mobile { get; set; } = null!;
        public string? Business { get; set; } = null!;
        public string? Work { get; set; } = null!;

        public Contact Contact { get; set; } = null!;

        public void Configure(EntityTypeBuilder<PhoneNumber> builder)
        {
            builder
                .HasOne(e => e.Contact)
                .WithOne(e => e.PhoneNumber)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.Mobile)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.Business)
                .HasMaxLength(20);

            builder.Property(e => e.Work)
                .HasMaxLength(20);
        }
    }
}