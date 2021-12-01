using System.ComponentModel;

namespace _4oito6.Demonstration.Domain.Model.Enum
{
    public enum Gender
    {
        [Description("Masculino")]
        Male = 1,

        [Description("Feminino")]
        Female = 2,

        [Description("Não informado")]
        NotInformed = 0
    }
}