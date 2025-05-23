using PersonRegistration.Application.Validators;
using PersonRegistration.Domain.Models;
using System;
using Xunit;

namespace PersonRegistration.Tests
{
    public class PersonValidatorTests
    {
        private readonly PersonValidator _validator = new();
        
        [Fact]
        public void Person_FutureBirthdate_IsNotEligible()
        {
            var person = new Person { DateOfBirth = DateTime.Today.AddDays(1) };
            Assert.False(_validator.IsEligibleForRegistration(person));
        }

        [Fact]
        public void Person_Under16_IsNotEligible()
        {
            var person = new Person
            {
                FirstName = "Tom",
                Surname = "Thumb",
                DateOfBirth = DateTime.Today.AddYears(-15),
            };

            var result = _validator.IsEligibleForRegistration(person);

            Assert.False(result);
        }

        [Fact]
        public void Person_Between16And18_NoParentalAuth_IsNotEligible()
        {
            var person = new Person
            {
                FirstName = "Jane",
                Surname = "Doe",
                DateOfBirth = DateTime.Today.AddYears(-17),
                ParentalAuthorization = false
            };

            var result = _validator.IsEligibleForRegistration(person);

            Assert.False(result);
        }

        [Fact]
        public void Person_Between16And18_WithParentalAuth_IsEligible()
        {
            var person = new Person
            {
                FirstName = "John",
                Surname = "Doe",
                DateOfBirth = DateTime.Today.AddYears(-17),
                ParentalAuthorization = true
            };

            var result = _validator.IsEligibleForRegistration(person);

            Assert.True(result);
        }

        [Fact]
        public void Person_Over18_IsEligible()
        {
            var person = new Person
            {
                FirstName = "Alice",
                Surname = "Smith",
                DateOfBirth = DateTime.Today.AddYears(-25)
            };

            var result = _validator.IsEligibleForRegistration(person);

            Assert.True(result);
        }

        [Fact]
        public void Person_Between16And18_ParentalAuth_Null_IsNotEligible()
        {
            var person = new Person
            {
                FirstName = "Tim",
                Surname = "Taylor",
                DateOfBirth = DateTime.Today.AddYears(-17),
                ParentalAuthorization = null
            };

            var result = _validator.IsEligibleForRegistration(person);

            Assert.False(result);
        }

        [Fact]
        public void Person_Exactly16_NoParentalAuth_IsNotEligible()
        {
            var person = new Person
            {
                FirstName = "Lilly",
                Surname = "Rose",
                DateOfBirth = DateTime.Today.AddYears(-16),
                ParentalAuthorization = false
            };

            var result = _validator.IsEligibleForRegistration(person);

            Assert.False(result);
        }

        [Fact]
        public void Person_Exactly18_IsEligible()
        {
            var person = new Person
            {
                FirstName = "Ben",
                Surname = "Miller",
                DateOfBirth = DateTime.Today.AddYears(-18)
            };

            var result = _validator.IsEligibleForRegistration(person);

            Assert.True(result);
        }
    }
}
