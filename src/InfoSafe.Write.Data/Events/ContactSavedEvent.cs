﻿using SharedKernel.Interfaces;

namespace InfoSafe.Write.Data.Events
{
    public class ContactSavedEvent : IDomainEvent
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DoB { get; set; }

        public ContactSavedEvent(int id, string firstName, string lastName, DateTime doB)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            DoB = doB;
        }
    }
}