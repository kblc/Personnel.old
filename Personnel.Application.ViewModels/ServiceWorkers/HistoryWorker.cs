using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.ServiceWorkers
{
    public static partial class Workers
    {
        private static HistoryWorker historyWorker = null;
        public static HistoryWorker HistoryWorker { get { return historyWorker ?? (historyWorker = new HistoryWorker()); } }
    }

    public class HistoryWorker : AbstractBaseWorker
    {
        protected override void Init()
        {
            throw new NotImplementedException();
        }
    }
}
