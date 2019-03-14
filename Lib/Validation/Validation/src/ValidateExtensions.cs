using Microsoft.Extensions.Options;

namespace Qoden.Validation
{
    public interface IValidate
    {
        void Validate(IValidator validator);
    }

    public static class ValidateExtensions
    {
        public static void Valid(this IValidate v)
        {
            v.Validate(ImmediateValidator.Instance);
        }

        public static T Valid<T>(this IOptions<T> v) where T : class, IValidate, new()
        {
            v.Value.Validate(ImmediateValidator.Instance);
            return v.Value;
        }
    }
}