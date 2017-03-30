using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DP.LSP.Tools.DiskMon.Core
{
    interface IServiceLocator
    {
        T GetService<T>();
        void Register<T>(object service);
    }

    internal class ServiceLocator : IServiceLocator
    {
        private IDictionary<object, object> _services;
        private static IServiceLocator _instance;

        public static IServiceLocator Instance
        {
            get { return _instance = _instance ?? new ServiceLocator(); }
        }

        private ServiceLocator()
        {
            _services = new Dictionary<object, object>();
        }

        public T GetService<T>()
        {
            try
            {
                return (T)_services[typeof(T)];
            }
            catch
            {
                var error = string.Format("Service of type {0} is not registered.", typeof(T));
                LogHelper.Instance.Error(error);
                throw new Exception(error);
            }
        }

        public void Register<T>(object service)
        {
            if (!_services.ContainsKey(typeof(T)))
                _services.Add(typeof(T), service);
            else
            {
                var error = string.Format("Service of type {0} is already registered.", typeof(T));
                LogHelper.Instance.Error(error);
                throw new Exception(error);
            }
        }
    }
}
