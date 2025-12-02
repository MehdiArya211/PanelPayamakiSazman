using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extentions
{
    public class NameGenerator
    {
        public static string GenerateUniqCode()
        {
            //==== GUId = Globaly Unique Identifire
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
