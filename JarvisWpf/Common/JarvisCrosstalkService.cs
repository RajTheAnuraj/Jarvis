using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWpf.Common
{
    public class JarvisCrosstalkService:ICrosstalkService
    {
        Dictionary<string, ICrosstalk> _CallbackObjectIndex;

        static ICrosstalkService staticInstance;
        
        public Dictionary<string, ICrosstalk> CallbackObjectIndex
        {
            get
            {
                return _CallbackObjectIndex;
            }
            set
            {
                _CallbackObjectIndex = value;
            }
        }

        private JarvisCrosstalkService()
        {
            CallbackObjectIndex = new Dictionary<string, ICrosstalk>();
        }

        public static ICrosstalkService CreateInstance()
        {
            if (staticInstance == null)
                staticInstance = new JarvisCrosstalkService();

            return staticInstance;
        }

        public void RegisterCallback(string CallbackObjectIdentifier, ICrosstalk CallbackObject)
        {
            if (CallbackObjectIndex.Keys.Contains(CallbackObjectIdentifier))
            {
                CallbackObjectIndex[CallbackObjectIdentifier] = CallbackObject;
            }
            else
            {
                CallbackObjectIndex.Add(CallbackObjectIdentifier, CallbackObject);
            }
        }

        public object Crosstalk(string CallbackObjectIdentifier, string ActionName, object[] Parameters)
        {
            if (!CallbackObjectIndex.Keys.Contains(CallbackObjectIdentifier))
                return null;
            ICrosstalk obj = CallbackObjectIndex[CallbackObjectIdentifier];
            if (obj == null) return null;
            return obj.IncomingCrosstalk(ActionName, Parameters);
        }
    }
}
