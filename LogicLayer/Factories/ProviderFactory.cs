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

        private static IResourceProvider _CurrentProvider;

        private static IResourceProvider CurrentProvider
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

        public static IResourceProvider GetCurrentProvider()
        {
            return CurrentProvider;
        }
    }
}
