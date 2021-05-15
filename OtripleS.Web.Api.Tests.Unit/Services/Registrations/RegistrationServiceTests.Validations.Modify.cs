﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using Moq;
using OtripleS.Web.Api.Models.Registrations;
using OtripleS.Web.Api.Models.Registrations.Exceptions;
using Xunit;

namespace OtripleS.Web.Api.Tests.Unit.Services.Registrations
{
    public partial class RegistrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyWhenRegistrationIsNullAndLogItAsync()
        {
            //given
            Registration invalidRegistration = null;
            var nullRegistrationException = new NullRegistrationException();

            var expectedRegistrationValidationException =
                new RegistrationValidationException(nullRegistrationException);

            //when
            ValueTask<Registration> modifyRegistrationTask =
                this.registrationService.ModifyRegistrationAsync(invalidRegistration);

            //then
            await Assert.ThrowsAsync<RegistrationValidationException>(() =>
                modifyRegistrationTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedRegistrationValidationException))),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyWhenRegistrationIdIsInvalidAndLogItAsync()
        {
            //given
            Guid invalidRegistrationId = Guid.Empty;
            DateTimeOffset dateTime = GetRandomDateTime();
            Registration randomRegistration = CreateRandomRegistration(dateTime);
            Registration invalidRegistration = randomRegistration;
            invalidRegistration.Id = invalidRegistrationId;

            var invalidRegistrationException = new InvalidRegistrationException(
                parameterName: nameof(Registration.Id),
                parameterValue: invalidRegistration.Id);

            var expectedRegistrationValidationException =
                new RegistrationValidationException(invalidRegistrationException);

            //when
            ValueTask<Registration> modifyRegistrationTask =
                this.registrationService.ModifyRegistrationAsync(invalidRegistration);

            //then
            await Assert.ThrowsAsync<RegistrationValidationException>(() =>
                modifyRegistrationTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedRegistrationValidationException))),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnModifyWhenStudentNameIsInvalidAndLogItAsync(
            string invalidRegistrationStudentName)
        {
            // given
            DateTimeOffset datetime = GetRandomDateTime();
            Registration randomRegistration = CreateRandomRegistration(datetime);
            Registration invalidRegistration = randomRegistration;
            invalidRegistration.StudentName = invalidRegistrationStudentName;

            var invalidRegistrationInputException = new InvalidRegistrationException(
               parameterName: nameof(Registration.StudentName),
               parameterValue: invalidRegistration.StudentName);

            var expectedRegistrationValidationException =
                new RegistrationValidationException(invalidRegistrationInputException);

            // when
            ValueTask<Registration> modifyRegistrationTask =
                this.registrationService.ModifyRegistrationAsync(invalidRegistration);

            // then
            await Assert.ThrowsAsync<RegistrationValidationException>(() =>
                modifyRegistrationTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedRegistrationValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRegistrationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateRegistrationAsync(It.IsAny<Registration>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();

        }
    }
}