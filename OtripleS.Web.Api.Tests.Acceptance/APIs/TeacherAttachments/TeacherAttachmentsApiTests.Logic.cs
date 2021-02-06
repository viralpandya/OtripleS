﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using OtripleS.Web.Api.Tests.Acceptance.Models.TeacherAttachments;
using RESTFulSense.Exceptions;
using Xunit;

namespace OtripleS.Web.Api.Tests.Acceptance.APIs.TeacherAttachments
{
    public partial class TeacherAttachmentsApiTests
    {
        [Fact]
        public async Task ShouldPostTeacherAttachmentAsync()
        {
            // given
            TeacherAttachment randomTeacherAttachment = await CreateRandomTeacherAttachment();
            TeacherAttachment inputTeacherAttachment = randomTeacherAttachment;
            TeacherAttachment expectedTeacherAttachment = inputTeacherAttachment;

            // when             
            TeacherAttachment actualTeacherAttachment =
                await this.otripleSApiBroker.PostTeacherAttachmentAsync(inputTeacherAttachment);

            TeacherAttachment retrievedTeacherAttachment =
                await this.otripleSApiBroker.GetTeacherAttachmentByIdsAsync(
                    inputTeacherAttachment.TeacherId,
                    inputTeacherAttachment.AttachmentId);

            // then
            actualTeacherAttachment.Should().BeEquivalentTo(expectedTeacherAttachment);
            retrievedTeacherAttachment.Should().BeEquivalentTo(expectedTeacherAttachment);
            await DeleteTeacherAttachmentAsync(actualTeacherAttachment);
        }

        [Fact]
        public async Task ShouldGetAllTeacherAttachmentsAsync()
        {
            // given
            var randomTeacherAttachments = new List<TeacherAttachment>();

            for (var i = 0; i <= GetRandomNumber(); i++)
            {
                TeacherAttachment randomTeacherAttachment = await PostTeacherAttachmentAsync();
                randomTeacherAttachments.Add(randomTeacherAttachment);
            }

            List<TeacherAttachment> inputTeacherAttachments = randomTeacherAttachments;
            List<TeacherAttachment> expectedTeacherAttachments = inputTeacherAttachments;

            // when
            List<TeacherAttachment> actualTeacherAttachments =
                await this.otripleSApiBroker.GetAllTeacherAttachmentsAsync();

            // then
            foreach (TeacherAttachment expectedTeacherAttachment in expectedTeacherAttachments)
            {
                TeacherAttachment actualTeacherAttachment =
                    actualTeacherAttachments.Single(studentAttachment =>
                        studentAttachment.TeacherId == expectedTeacherAttachment.TeacherId);

                actualTeacherAttachment.Should().BeEquivalentTo(expectedTeacherAttachment);

                await DeleteTeacherAttachmentAsync(actualTeacherAttachment);
            }
        }

        [Fact]
        public async Task ShouldDeleteTeacherAttachmentAsync()
        {
            // given
            TeacherAttachment randomTeacherAttachment = await PostTeacherAttachmentAsync();
            TeacherAttachment inputTeacherAttachment = randomTeacherAttachment;
            TeacherAttachment expectedTeacherAttachment = inputTeacherAttachment;

            // when 
            TeacherAttachment deletedTeacherAttachment =
                await DeleteTeacherAttachmentAsync(inputTeacherAttachment);

            ValueTask<TeacherAttachment> getTeacherAttachmentByIdTask =
                this.otripleSApiBroker.GetTeacherAttachmentByIdsAsync(
                    inputTeacherAttachment.TeacherId,
                    inputTeacherAttachment.AttachmentId);

            // then
            deletedTeacherAttachment.Should().BeEquivalentTo(expectedTeacherAttachment);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(() =>
               getTeacherAttachmentByIdTask.AsTask());
        }
    }
}
