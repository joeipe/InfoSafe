﻿using System.ComponentModel.DataAnnotations.Schema;

namespace InfoSafe.Read.Domain
{
    [Table("Main.PhoneNumbers")]
    public class PhoneNumber
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string Mobile { get; set; } = null!;
        public string? Business { get; set; } = null!;
        public string? Work { get; set; } = null!;
    }
}