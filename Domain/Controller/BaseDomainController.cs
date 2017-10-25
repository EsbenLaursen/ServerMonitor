using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public abstract class BaseDomainController
    {

        protected bool UpdateAllowed(PropertyInfo prop)
        {
            bool primitive = new[] {typeof(string),
                                    typeof(char),
                                    typeof(byte),
                                    typeof(sbyte),
                                    typeof(ushort),
                                    typeof(short),
                                    typeof(uint),
                                    typeof(int),
                                    typeof(int?),
                                    typeof(ulong),
                                    typeof(long),
                                    typeof(float),
                                    typeof(double),
                                    typeof(decimal),
                                    typeof(bool),
                                    typeof(DateTime),
                                    typeof(DateTime?),
                                    typeof(TimeSpan),
                                    typeof(TimeSpan?),
                                    typeof(Guid),
                                    typeof(Guid?),
                                    typeof(decimal?)}.Contains(prop.PropertyType);

            return prop.CanWrite
                && primitive
                && !prop.Name.Equals("Id")
                && !prop.Name.Equals("DateTimeCreated");
        }
    }
}
