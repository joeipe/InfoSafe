using AutoMapper;
using AutoMapper.EquivalencyExpression;

namespace InfoSafe.API.AutoMapper
{
    public class AutoMapperProfileSetup
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddCollectionMappers();
                cfg.AddProfile(new DomainToViewModelMappingProfile());
                cfg.AddProfile(new ViewModelToDomainMappingProfile());
            });
        }
    }
}