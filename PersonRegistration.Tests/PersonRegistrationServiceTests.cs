﻿using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using PersonRegistration.Application.Services;
using PersonRegistration.Domain.Enums;
using PersonRegistration.Domain.Interfaces;
using PersonRegistration.Domain.Models;
using PersonRegistration.Domain.Validators;
using Xunit;

namespace PersonRegistration.Tests.Services
{
    public class PersonRegistrationServiceTests
    {
        private readonly Mock<IPersonRepository> _repositoryMock;
        private readonly Mock<IPersonValidator> _validatorMock;
        private readonly Mock<ILogger<PersonRegistrationService>> _loggerMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly PersonRegistrationService _service;

        public PersonRegistrationServiceTests()
        {
            _repositoryMock = new Mock<IPersonRepository>();
            _validatorMock = new Mock<IPersonValidator>();
            _loggerMock = new Mock<ILogger<PersonRegistrationService>>();
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["BasePath"]).Returns("c:/people"); // default base path

            _service = new PersonRegistrationService(
                _repositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object,
                _configMock.Object);
        }

        [Fact]
        public void RegisterPerson_ValidationFails_ReturnsFalse()
        {
            var person = new Person { FirstName = "Test", Surname = "User", DateOfBirth = DateTime.Today.AddYears(-15), MaritalStatus = MaritalStatus.Single };
            _validatorMock.Setup(v => v.IsEligibleForRegistration(person)).Returns(false);

            bool result = _service.RegisterPerson(person, null, null);

            Assert.False(result);
            _repositoryMock.Verify(r => r.SavePerson(It.IsAny<Person>()), Times.Never);
        }

        [Fact]
        public void RegisterPerson_WithSpouse_SavesSpouseAndPerson()
        {
            var person = new Person { FirstName = "John", Surname = "Doe", DateOfBirth = DateTime.Today.AddYears(-30), MaritalStatus = MaritalStatus.Single };
            var spouse = new Spouse { FirstName = "Jane", Surname = "Doe", DateOfBirth = DateTime.Today.AddYears(-28), MaritalStatus = MaritalStatus.Married };
            var spouseFilePath = "c:/people/spouses/Jane_Doe.txt";

            _validatorMock.Setup(v => v.IsEligibleForRegistration(person)).Returns(true);

            var result = _service.RegisterPerson(person, spouse, spouseFilePath);

            Assert.True(result);
            _repositoryMock.Verify(r => r.SaveSpouse(spouse, spouseFilePath), Times.Once);
            _repositoryMock.Verify(r => r.SavePerson(person), Times.Once);
            Assert.Equal(spouseFilePath, person.SpouseFilePath);
        }
    }
}
