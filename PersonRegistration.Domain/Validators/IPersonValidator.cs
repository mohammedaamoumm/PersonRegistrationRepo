using PersonRegistration.Domain.Models;

namespace PersonRegistration.Domain.Validators
{
    public interface IPersonValidator
    {
        bool IsEligibleForRegistration(Person person);

    }
}
