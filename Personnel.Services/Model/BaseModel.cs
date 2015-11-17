using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Helpers.Linq;

namespace Personnel.Services.Model
{
    [DataContract]
    [Serializable]
    public abstract class BaseModel
    {
        public override string ToString()
        {
            return this.GetType().Name + ":[" +
                
                this.GetType()
                .GetProperties(System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy)
                .Select(p =>
                {
                    object value = null;
                    try
                    {
                        value = p.GetValue(this);
                    }
                    catch { value = "ERR"; }
                    return new
                    {
                        Value = value,
                        Property = p.Name
                    };
                })
                .Concat(i => $"{i.Property}='{i.Value?.ToString() ?? "NULL"}'", ",")
                
                + "]";
        }
    }
}
