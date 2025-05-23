using PersonRegistration.Domain.Models;
using PersonRegistration.Domain.Validators;

namespace PersonRegistration.Application.Validators
{
    public class PersonValidator : IPersonValidator
    {
        public bool IsEligibleForRegistration(Person person)
        {
            if (person.Age < 16)
                return false;

            if (person.Age < 18 && person.ParentalAuthorization != true)
                return false;

            return true;
        }
    }
}
