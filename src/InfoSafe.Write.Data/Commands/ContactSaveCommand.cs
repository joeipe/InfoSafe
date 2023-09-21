using CSharpFunctionalExtensions;
using InfoSafe.ViewModel;
using SharedKernel.Extensions;
using SharedKernel.Interfaces;

namespace InfoSafe.Write.Data.Commands
{
    public record ContactSaveCommand(ContactVM Contact) : ICommand
    {
        public Result ValidationResult { get; set; }

        public void Validate()
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(Contact.FirstName))
            {
                errors.Add("FirstName should not be empty");
            }
            if (string.IsNullOrEmpty(Contact.LastName))
            {
                errors.Add("LastName should not be empty");
            }
            if (string.IsNullOrEmpty(Contact.DoB))
            {
                errors.Add("DoB should not be empty");
            }
            if (!Contact.DoB.IsValidDate())
            {
                errors.Add("DoB should be 'dd/MM/yyyy' format");
            }

            ValidationResult = errors.Count != 0 ? Result.Failure(string.Join(",", errors)) : Result.Success();
        }
    }
}