using AutoMapper;

namespace VotingSystem.WebAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAutomapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());

            try
            {
                mapperConfig.AssertConfigurationIsValid();
            }
            catch (AutoMapperConfigurationException ex)
            {
                Console.WriteLine("AutoMapper configuration is invalid:");
                Console.WriteLine(ex.ToString());
                throw;
            }

            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
