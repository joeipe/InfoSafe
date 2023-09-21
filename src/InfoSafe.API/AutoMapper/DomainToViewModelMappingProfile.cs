using AutoMapper;
using InfoSafe.Read.Domain;
using InfoSafe.ViewModel;
using SharedKernel.Extensions;

namespace InfoSafe.API.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Contact, ContactVM>()
                .ForMember(dest => dest.DoB, opt => opt.MapFrom(src => src.DoB.ParseDate()));
            CreateMap<Address, AddressVM>();
            CreateMap<EmailAddress, EmailAddressVM>();
            CreateMap<PhoneNumber, PhoneNumberVM>();
        }
    }
}