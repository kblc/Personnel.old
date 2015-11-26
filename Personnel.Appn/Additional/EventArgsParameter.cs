using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Appn.Additional
{
    public class EventArgsParameter
    {
        public object CommandParameter { get; private set; }
        public object EventArgs { get; private set; }
        public EventArgsParameter(object commandParameter, object eventArgs)
        {
            CommandParameter = commandParameter;
            EventArgs = eventArgs;
        }
    }
}
