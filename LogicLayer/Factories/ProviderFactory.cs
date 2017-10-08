using LogicLayer.Implementations;
using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer.Factories
{
    public class ProviderFactory
    {
        static object CurrentProviderLocker = new object();

        private static IProjectPayloadProvider _CurrentProvider;

        private static IProjectPayloadProvider CurrentProvider
        {
            get
            {
                lock (CurrentProviderLocker)
                {
                    if (_CurrentProvider == null)
                        _CurrentProvider = new ProjectPayloadProvider();
                }
                return _CurrentProvider;
            }

        }

        public static IProjectPayloadProvider GetCurrentProvider()
        {
            return CurrentProvider;
        }
    }
}
