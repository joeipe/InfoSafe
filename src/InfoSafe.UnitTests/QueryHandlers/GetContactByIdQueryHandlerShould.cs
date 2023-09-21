using AutoMapper;
using FluentAssertions;
using InfoSafe.API.AutoMapper;
using InfoSafe.Read.Data.QueryHandlers;
using InfoSafe.Read.Data.Repositories.Interfaces;
using InfoSafe.Read.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel.Utils;
using static InfoSafe.Read.Data.Queries.Queries;

namespace InfoSafe.UnitTests.QueryHandlers
{
    public class GetContactByIdQueryHandlerShould
    {
        private static Mock<ILogger<GetContactByIdQueryHandler>> _mockLogger = null!;
        private static Mock<IContactRepository> _mockContactRepository = null!;
        private static IMapper _mapper = null!;

        public GetContactByIdQueryHandlerShould()
        {
            _mockLogger = new Mock<ILogger<GetContactByIdQueryHandler>>();
            _mockContactRepository = new Mock<IContactRepository>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfiles(new List<Profile> { new DomainToViewModelMappingProfile() });
            });
            _mapper = mappingConfig.CreateMapper();
        }

        [Fact]
        public async Task GetContactById()
        {
            //Arrange
            var data = JsonFileReader.Read<Contact>(@"contact_get.json", @"UnitTestsData\");

            var id = 1;
            _mockContactRepository.Setup(x => x.GetContactByIdAsync(id)).ReturnsAsync(data);

            //Act
            var sut = new GetContactByIdQueryHandler(_mockLogger.Object, _mockContactRepository.Object, _mapper);
            var result = await sut.Handle(new GetContactByIdQuery(id), new CancellationToken());

            //Assert
            _mockContactRepository.Verify(x => x.GetContactByIdAsync(id), Times.Once);
            result.Id.Should().Be(1);
            result.FirstName.Should().Be("Joe");
            result.LastName.Should().Be("Ipe");
            result.DoB.Should().Be("26/04/1981");
        }
    }
}