using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Exceptions;
public class DomainEntityException : Exception
{
    private IDomainValidations.DomainError[] errors;

    public DomainEntityException(IDomainValidations.DomainError[] errors)
    {
        this.errors = errors;
    }
}