
namespace CongEspVilaGuilhermeApi.Domain.Entities;

public interface IDomainValidations
{
    public record DomainError(string field, string error)
    {
        static string NullErrorMessage = "valor não pode ser nulo";
        public static void Null(string fieldName, string value, List<DomainError> errors)
        {
            if(string.IsNullOrEmpty(value))
            {
                errors.Add(new DomainError(fieldName, NullErrorMessage));
            }
        }
    }

    DomainError[] CheckErrors();
}