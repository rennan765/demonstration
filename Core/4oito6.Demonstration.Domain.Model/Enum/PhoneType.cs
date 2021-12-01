using System.ComponentModel;

namespace _4oito6.Demonstration.Domain.Model.Enum
{
    public enum PhoneType
    {
        [Description("Resiedncial")]
        Home = 1,

        [Description("Profissional")]
        Business = 2,

        [Description("Celular")]
        Cel = 3
    }
}