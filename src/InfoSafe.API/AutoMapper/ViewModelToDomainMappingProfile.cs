using AutoMapper;
using AutoMapper.EquivalencyExpression;
using InfoSafe.ViewModel;
using InfoSafe.Write.Domain;
using SharedKernel.Extensions;

namespace InfoSafe.API.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ContactVM, Contact>()
                .ForMember(dest => dest.DoB, opt => opt.MapFrom(src => src.DoB.ParseDate()));
            CreateMap<AddressVM, Address>();
            CreateMap<EmailAddressVM, EmailAddress>().EqualityComparison((vm, o) => vm.Id == o.Id);
            CreateMap<PhoneNumberVM, PhoneNumber>();
        }
    }
}