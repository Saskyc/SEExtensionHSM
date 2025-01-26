using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;

namespace SEExtensionHSM
{
    public class SeExtensionHsm : Plugin<Config>
    {
        public override string Author => "saskyc";
        public override string Name => "SEExtensionHSM";
        
        public static Assembly Exists => Loader.Plugins.FirstOrDefault(p => p.Name is "UncomplicatedCustomRoles")?.Assembly;
        
        public override void OnEnabled()
        {
            if(Exists == null)
                Log.Info("Hello you installed extension of Scripted Events. We unfortunately cannot work with HSM due to not having installed the API. Visit github for more Info.");
            API.ScriptedEventsIntegration.RegisterCustomActions();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            API.ScriptedEventsIntegration.UnregisterCustomActions();
            base.OnDisabled();
        }
        
        
    }
}