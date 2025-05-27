using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CongEspVilaGuilhermeApi.Domain.Exceptions
{
    public class EntityNotFoundException<T>: Exception
    {
        public EntityNotFoundException() : base($"Entity {nameof(T)} not found")
        {
        }
    }
}