using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Contact.Data.Model
{
    public static class DataMapper
    {
        public static Dictionary<string, object> ToBulkDictionary<TDto>(this TDto dto)
        {
            return typeof(TDto).GetProperties()
                .ToDictionary(x => x.Name, x => x.GetValue(dto));
        }
    }
}