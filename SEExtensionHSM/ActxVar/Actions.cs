using System;
using Exiled.API.Features;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;

namespace API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Exiled.API.Features;
    using Exiled.Loader;

    /// <summary>
    /// The class for Scripted Events custom action integration.
    /// </summary>
    internal static class ScriptedEventsIntegration
    {
        /// <summary>
        /// Gets the Scripted Events API.
        /// </summary>
        internal static Type API => Loader.GetPlugin("ScriptedEvents")?.Assembly?.GetType("ScriptedEvents.API.Features.ApiHelper");

        /// <summary>
        /// Gets a value indicating whether the Scripted Evetns API is available to be used.
        /// </summary>
        internal static bool CanInvoke => API is not null && AddAction is not null && RemoveAction is not null && APIGetPlayersMethod is not null;

        /// <summary>
        /// Gets the MethodInfo for checking if the ScriptModule was loaded.
        /// </summary>
        internal static MethodInfo ModuleLoadedMethod => API?.GetMethod("IsModuleLoaded");

        /// <summary>
        /// Gets a value indicating whether the ScriptModule of Scripted Events is loaded.
        /// </summary>
        internal static bool IsModuleLoaded => (bool)ModuleLoadedMethod.Invoke(null, Array.Empty<object>());

        /// <summary>
        /// Gets the MethodInfo for adding a custom action.
        /// </summary>
        internal static MethodInfo AddAction => API?.GetMethod("RegisterCustomAction");

        /// <summary>
        /// Gets the MethodInfo for removing a custom action.
        /// </summary>
        internal static MethodInfo RemoveAction => API?.GetMethod("UnregisterCustomAction");

        /// <summary>
        /// Gets the MethodInfo for removing a custom action.
        /// </summary>
        internal static MethodInfo APIGetPlayersMethod => API?.GetMethod("GetPlayers");

        /// <summary>
        /// Gets a list of custom actions registered.
        /// </summary>
        internal static List<string> CustomActions { get; } = new();

        public static void RegisterCustomAction(string name, Func<Tuple<string[], object>, Tuple<bool, string, object[]>> action)
        {
            try
            {
                AddAction.Invoke(null, new object[] { name, action });
                CustomActions.Add(name);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Source} - {e.GetType().FullName} error: {e.Message}");
            }
        }

        /// <summary>
        /// Registers custom actions defined in the method.
        /// Used when plugin is enabled.
        /// </summary>
        public static async void RegisterCustomActions()
        {
            if (!CanInvoke)
            {
                Log.Warn("[Scripted Events integration] Scripted Events is either not present or outdated. Ignore this message if you're not using Scripted Events.");
                return;
            }

            int tries = 0;
            while (!IsModuleLoaded)
            {
                tries++;
                Log.Debug("[Scripted Events integration] Scripted Events is present, but ScriptModule is not yet loaded; retrying in 1s");
                await Task.Delay(1000);

                if (tries > 10)
                {
                    Log.Error("[Scripted Events integration] ScriptModule has not initialized in time; custom actions will not be added.");
                    return;
                }
            }
            
            RegisterCustomAction("ADVANCEDHINT", (Tuple<string[], object> input) =>
            {
                string[] args = input.Item1;
                object script = input.Item2;
                
                //Example usage:
                //ADVANCEDHINT SET {MyPlayers} 2 Left 100 100 20 5 Hello World Beutiful Day Isn't it?
                //MODE,PLAYERS,ID,Align-Left(1)/Center(2)/Right(3),XPos,YPos,FontSize,RemovalAfter,Text
                
                var toHint = GetPlayers(args[1], script, -1);

                var amt = 1;
                
                switch (args[0].ToUpper())
                {
                    case "SET":
                        
                        var alignText = HintAlignment.Center;
        
                        switch (args[4])
                        {
                            case "1":
                                alignText = HintAlignment.Left;
                                break;
            
                            case "2":
                                alignText = HintAlignment.Center;
                                break;
            
                            case "3":
                                alignText = HintAlignment.Right;
                                break;
                        }

                        //args[0] = Mode Set/Remove
                        
                        //args[1] = Players
                        
                        var id = args[2];

                        var align = alignText; //args[3]
                        
                        if (!float.TryParse(args[4], out float xPos))
                            return new(false, "3th array of all arguments could not be parsed. This is meant to be XCoordinate (XPos)", null);
                        
                        if (!float.TryParse(args[5], out float yPos))
                            return new(false, "4th array of all arguments could not be parsed. This is meant to be YCoordinate (YPos)", null);
                        
                        if(!float.TryParse(args[6], out float removeAfter))
                            return new(false, "5th arry of all arguments could not be parsed. This is meant to be removeAfter.", null);
                        
                        if (!int.TryParse(args[7], out int fontSize))
                            return new(false, "6th array of all arguments could not be parsed. This is meant to be Font size", null);

                        var text = args[8].Replace("_", " ");
                        
                        foreach (var player in toHint)
                        {
                            for (int i = 0; i < amt; i++)
                            {
                                var pd = player.GetPlayerDisplay();
                                var SeHint = new HintServiceMeow.Core.Models.Hints.Hint()
                                {
                                    Id = id,
                                    Alignment = align,
                                    XCoordinate = xPos,
                                    YCoordinate = yPos,
                                    FontSize = fontSize,
                                    Text = text,
                                };
                        
                                pd.AddHint(SeHint);
                                pd.RemoveAfter(SeHint, float.Parse(args[7]));
                            }
                        }

                        return new(true, string.Empty, null);
                        
                        break;
                    
                    case "REMOVE":
                        foreach (var player in toHint)
                        {
                            for (int i = 0; i < amt; i++)
                            {
                                var pd = player.GetPlayerDisplay();

                                if (pd != null)
                                    pd.RemoveHint(args[2]);
                            }
                        }
                        return new(true, string.Empty, null);
                        break;
                }
                return new(false, "Selected mode was not Remove or Set.", null);
            });
            
            /*
            // actions are defined here
            RegisterCustomAction("GET_ALL_PLAYERS", (Tuple<string[], object> input) =>
            {
                
                * true - action executed successfully
                * string.Empty - error response (no error so empty)
                * new[] { Player.List.ToArray() } - object[] containing Player[] containing all players
                
            });
            */
        }

        /// <summary>
        /// Registers custom actions defined previously.
        /// Used when plugin is disabled.
        /// </summary>
        public static void UnregisterCustomActions()
        {
            if (!CanInvoke) return;

            foreach (string name in CustomActions)
                RemoveAction.Invoke(null, new object[] { name });
        }

        /// <summary>
        /// Gets the MethodInfo for getting the players from a variable.
        /// </summary>
        /// <param name="input">The input to process.</param>
        /// <param name="script">The script as object.</param>
        /// <param name="max">The number of players to return (-1 for unlimited).</param>
        /// <returns>The list of players.</returns>
        internal static Player[] GetPlayers(string input, object script, int max = -1)
        {
            return (Player[])APIGetPlayersMethod.Invoke(null, new[] { input, script, max });
        }
    }
}