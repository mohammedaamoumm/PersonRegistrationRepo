using PersonRegistration.Domain.Models;
using PersonRegistration.Domain.Validators;

namespace PersonRegistration.Application.Validators
{
    public class PersonValidator : IPersonValidator
    {
        public bool IsEligibleForRegistration(Person person)
        {
            var today = DateTime.Today;
            if (person.DateOfBirth > today)
                return false;

            int age = today.Year - person.DateOfBirth.Year;
            if (person.DateOfBirth.Date > today.AddYears(-age)) age--;

            if (age < 16)
                return false;
            if (age < 18 && person.ParentalAuthorization != true)
                return false;

            return true;
        }
    }
}
