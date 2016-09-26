using System;
using System.ComponentModel;

namespace ModelLauncher.Class
{
    public enum ConfigurationType
    {
        [Description("Viewer")]
        Viewer = 0,
        [Description("Drafter")]
        Drafter = 1,   
        [Description("Steel Detailing")]
        Steel_Detailing = 2,
        [Description("Developer")]
        Developer = 3        
    };    

    public static class EnumExtension
    {        
        public static string[] GetDescription(Type enumType)
        {
            string[] ret = new string[Enum.GetNames(enumType).Length];
            int counter = 0;
            foreach(ConfigurationType type in Enum.GetValues(enumType))
            {
               
                var field = enumType.GetField(type.ToString());
                var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute),
                                                           false);

                ret[counter] = attributes.Length == 0
                    ? type.ToString()
                    : ((DescriptionAttribute)attributes[0]).Description;
                counter++;
            }
            return ret;
        }
    }
}
