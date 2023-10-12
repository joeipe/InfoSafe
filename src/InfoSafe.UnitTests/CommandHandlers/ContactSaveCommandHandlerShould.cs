using AutoMapper;
using CSharpFunctionalExtensions;
using FluentAssertions;
using InfoSafe.API.AutoMapper;
using InfoSafe.ViewModel;
using InfoSafe.Write.Data.CommandHandlers;
using InfoSafe.Write.Data.Commands;
using InfoSafe.Write.Data.EventDispatchers;
using InfoSafe.Write.Data.Repositories.Interfaces;
using InfoSafe.Write.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel.Interfaces;
using SharedKernel.Utils;

namespace InfoSafe.UnitTests.CommandHandlers
{
    public class ContactSaveCommandHandlerShould
    {
        private static Mock<ILogger<ContactSaveCommandHandler>> _mockLogger = null!;
        private static Mock<IContactRepository> _mockContactRepository = null!;
        private static Mock<EventDispatcher> _mockEventDispatcher = null!;
        private static IMapper _mapper = null!;

        public ContactSaveCommandHandlerShould()
        {
            _mockLogger = new Mock<ILogger<ContactSaveCommandHandler>>();
            _mockContactRepository = new Mock<IContactRepository>();
            _mockEventDispatcher = new Mock<EventDispatcher>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfiles(new List<Profile> { new ViewModelToDomainMappingProfile() });
            });
            _mapper = mappingConfig.CreateMapper();
        }

        [Fact]
        public async Task FailValidation()
        {
            //Arrange
            var data = JsonFileReader.Read<ContactVM>(@"contact_invalid.json", @"UnitTestsData\");

            //Act
            var sut = new ContactSaveCommandHandler(_mockLogger.Object, _mapper, _mockContactRepository.Object, _mockEventDispatcher.Object);
            var result = await sut.Handle(new ContactSaveCommand(data), new CancellationToken());

            //Assert
            result.Should().Be(Result.Failure("FirstName should not be empty,LastName should not be empty"));
            _mockContactRepository.Verify(x => x.Create(It.IsAny<Contact>()), Times.Never);
            _mockContactRepository.Verify(x => x.GetContactByIdAsync(It.IsAny<int>()), Times.Never);
            _mockContactRepository.Verify(x => x.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateNewContact()
        {
            //Arrange
            var data = JsonFileReader.Read<ContactVM>(@"contact_add.json", @"UnitTestsData\");

            //Act
            var sut = new ContactSaveCommandHandler(_mockLogger.Object, _mapper, _mockContactRepository.Object, _mockEventDispatcher.Object);
            var result = await sut.Handle(new ContactSaveCommand(data), new CancellationToken());

            //Assert
            result.Should().Be(Result.Success());
            _mockContactRepository.Verify(x => x.Create(It.IsAny<Contact>()), Times.Once);
            _mockContactRepository.Verify(x => x.GetContactByIdAsync(It.IsAny<int>()), Times.Never);
            _mockContactRepository.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateContact()
        {
            //Arrange
            var data = JsonFileReader.Read<ContactVM>(@"contact_edit.json", @"UnitTestsData\");

            _mockContactRepository.Setup(x => x.GetContactByIdAsync(It.IsAny<int>())).ReturnsAsync(It.IsAny<Contact>());

            //Act
            var sut = new ContactSaveCommandHandler(_mockLogger.Object, _mapper, _mockContactRepository.Object, _mockEventDispatcher.Object);
            var result = await sut.Handle(new ContactSaveCommand(data), new CancellationToken());

            //Assert
            result.Should().Be(Result.Success());
            _mockContactRepository.Verify(x => x.Create(It.IsAny<Contact>()), Times.Never);
            _mockContactRepository.Verify(x => x.GetContactByIdAsync(It.IsAny<int>()), Times.Once);
            _mockContactRepository.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}