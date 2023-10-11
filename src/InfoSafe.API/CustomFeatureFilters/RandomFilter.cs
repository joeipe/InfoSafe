using Microsoft.FeatureManagement;

namespace InfoSafe.API.CustomFeatureFilters
{
    //[FilterAlias("RandomFilter")]
    public class RandomFilter : IFeatureFilter
    {
        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var settings = context.Parameters.Get<RandomFilterSettings>();

            if (settings != null & settings.Method == "Even")
            {
                return Task.FromResult(DateTime.Now.Ticks % 2 == 0);
            }

            if (settings != null & settings.Method == "Odd")
            {
                return Task.FromResult(DateTime.Now.Ticks % 2 != 0);
            }

            throw new Exception($"Random feature filter configured value '{settings.Method}'");
        }
    }
}