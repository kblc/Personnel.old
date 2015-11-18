using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Personnel.Application.ViewModels.ServiceWorkers
{
    public class HistoryWorker : AbstractBaseWorker
    {
        private Thread historyThread = null;

        protected override bool DoStart()
        {
            try
            { 
                historyThread = new Thread(new ParameterizedThreadStart(InfinityHistoryThread));
                historyThread.IsBackground = true;
                historyThread.Start(Context);
                return true;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
        }
        protected override bool DoStop()
        {
            try
            { 
                if (historyThread != null)
                    historyThread.Abort();
                return true;
            }
            catch(Exception ex)
            {
                SetError(ex);
                return false;
            }
            finally
            {
                historyThread = null;
            }
        }

        private bool isWaiting = false;
        public bool IsWaiting
        {
            get { return isWaiting; }
            private set
            {
                if (isWaiting == value)
                    return;
                isWaiting = value;
                RaisePropertyChanged();
                RaiseOnWaitingChanged();
            }
        }

        private bool isConnecting = false;
        public bool IsConnecting
        {
            get { return isConnecting; }
            private set
            {
                if (isConnecting == value)
                    return;
                isConnecting = value;
                RaisePropertyChanged();
                RaiseOnConnectingChanged();
            }
        }

        private void RaiseHistoryChanged(HistoryService.History history) => Context.DoCallBack(() => Changed?.Invoke(this, history));
        public event EventHandler<HistoryService.History> Changed;

        private void InfinityHistoryThread(object obj)
        {
            var modelLevelThContext = (System.Runtime.Remoting.Contexts.Context)obj;
            long eventId = 0;
            bool inited = false;
            do
            {
                IsWaiting = false;

                try
                {
                    using (var hClient = new HistoryService.HistoryServiceClient())
                    {
                        hClient.ChangeLanguage(ServiceCultureInfo.Name);

                        #region While not inited

                        while(!inited)
                        {
                            IsConnecting = true;
                            var initRes = hClient.Get();
                            IsConnecting = false;

                            if (string.IsNullOrEmpty(initRes.Error))
                            {
                                eventId = initRes.Value.EventId;
                                inited = true;
                                IsLoaded = true;
                                SetError(string.Empty);
                            }
                            else
                            {
                                SetError(initRes.Error);
                                Thread.Sleep(ConnectionTimeInterval);
                            }
                        }

                        #endregion

                        IsWaiting = true;
                        IsLoaded = true;

                        while (true)
                            try
                            {
                                IsConnecting = true;
                                var initRes = hClient.GetFrom(eventId);
                                IsConnecting = false;

                                if (string.IsNullOrEmpty(initRes.Error))
                                {
                                    eventId = initRes.Value.EventId;
                                    SetError(string.Empty);
                                    RaiseHistoryChanged(initRes.Value);
                                }
                                else
                                {
                                    SetError(initRes.Error);
                                    Thread.Sleep(ConnectionTimeInterval);
                                }
                            }
                            catch (TimeoutException)
                            {
                                IsConnecting = false;
                            }
                    }
                }
                catch(Exception ex)
                {
                    IsConnecting = false;
                    SetError(ex);
                }

                Thread.Sleep(ConnectionTimeInterval);

            } while (true);
        }

        private void RaiseOnWaitingChanged() => Context.DoCallBack(() => OnWaitingChanged?.Invoke(this, IsWaiting));
        private void RaiseOnConnectingChanged() => Context.DoCallBack(() => OnConnectingChanged?.Invoke(this, IsWaiting));

        public event EventHandler<bool> OnWaitingChanged;
        public event EventHandler<bool> OnConnectingChanged;
    }
}
