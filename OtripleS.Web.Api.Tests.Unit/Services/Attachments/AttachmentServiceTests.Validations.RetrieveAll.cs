﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using OtripleS.Web.Api.Models.Attachments;
using Xunit;

namespace OtripleS.Web.Api.Tests.Unit.Services.Attachments
{
    public partial class AttachmentServiceTests
    {
        [Fact]
        public void ShouldLogWarningOnRetrieveAllWhenAttachmentsWereEmptyAndLogIt()
        {
            // given
            IQueryable<Attachment> emptyStorageAttachments = new List<Attachment>().AsQueryable();
            IQueryable<Attachment> expectedAttachments = emptyStorageAttachments;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAttachments())
                    .Returns(expectedAttachments);

            // when
            IQueryable<Attachment> actualAttachments =
                this.attachmentService.RetrieveAllAttachments();

            // then
            actualAttachments.Should().BeEquivalentTo(emptyStorageAttachments);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAttachments(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogWarning("No attachments found in storage."),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
