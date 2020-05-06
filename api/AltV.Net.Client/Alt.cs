using System.Collections.Generic;
using System.Numerics;
using AltV.Net.Client.Elements.Entities;
using AltV.Net.Client.Elements.Factories;
using AltV.Net.Client.Elements.Pools;
using AltV.Net.Client.Elements;
using AltV.Net.Client.EventHandlers;
using AltV.Net.Client.Events;
using WebAssembly;
using WebAssembly.Core;
using System.Text;

namespace AltV.Net.Client
{
    public static partial class Alt
    {
        private static NativeAlt _alt;

        public static JSObject NativeObject => _alt.alt;
        public static NativeNatives Natives;

        internal static NativeLocalStorage LocalStorage;

        internal static NativePlayer Player;

        internal static NativeHandlingData HandlingData;

        private static NativeAreaBlip AreaBlip;

        private static NativeRadiusBlip RadiusBlip;

        private static NativePointBlip PointBlip;

        private static IBaseObjectPool<IPlayer> PlayerPool;

        private static NativeWebView WebView;

        private static readonly IDictionary<string, NativeEventHandler<NativeEventDelegate, ServerEventDelegate>>
            NativeServerEventHandlers =
                new Dictionary<string, NativeEventHandler<NativeEventDelegate, ServerEventDelegate>>();

        private static readonly IDictionary<string, NativeEventHandler<NativeEventDelegate, ServerEventDelegate>>
            NativeEventHandlers =
                new Dictionary<string, NativeEventHandler<NativeEventDelegate, ServerEventDelegate>>();

        private static readonly IDictionary<string, NativeEventHandler<NativeEventDelegate, ServerEventDelegate>>
            NativeAsyncEventHandlers =
                new Dictionary<string, NativeEventHandler<NativeEventDelegate, ServerEventDelegate>>();


        private static NativeEventHandler<ConnectionCompleteEventDelegate, ConnectionCompleteEventDelegate>
            _nativeConnectionCompleteHandler;


        private static NativeEventHandler<DisconnectEventDelegate, DisconnectEventDelegate> _nativeDisconnectHandler;

        private static NativeEventHandler<EveryTickEventDelegate, EveryTickEventDelegate> _nativeEveryTickHandler;

        private static NativeEventHandler<NativeGameEntityCreateEventDelegate, GameEntityCreateEventDelegate> _nativeGameEntityCreateHandler;

        private static NativeEventHandler<NativeGameEntityDestroyEventDelegate, GameEntityDestroyEventDelegate> _nativeGameEntityDestroyHandler;

        private static NativeEventHandler<KeyDownEventDelegate, KeyDownEventDelegate> _nativeKeyDownHandler;
        private static NativeEventHandler<KeyUpEventDelegate, KeyUpEventDelegate> _nativeKeyUpHandler;


        public static event ConnectionCompleteEventDelegate OnConnectionComplete
        {
            add
            {
                if (_nativeConnectionCompleteHandler == null)
                {
                    _nativeConnectionCompleteHandler = new NativeConnectionCompleteEventHandler();
                    _alt.On("connectionComplete", _nativeConnectionCompleteHandler.GetNativeEventHandler());
                }

                _nativeConnectionCompleteHandler.Add(value);
            }
            remove => _nativeConnectionCompleteHandler?.Remove(value);
        }

        public static event DisconnectEventDelegate OnDisconnect
        {
            add
            {
                if (_nativeDisconnectHandler == null)
                {
                    _nativeDisconnectHandler = new NativeDisconnectEventHandler();
                    _alt.On("disconnect", _nativeDisconnectHandler.GetNativeEventHandler());
                }

                _nativeDisconnectHandler.Add(value);
            }
            remove => _nativeDisconnectHandler?.Remove(value);
        }

        public static event EveryTickEventDelegate OnEveryTick
        {
            add
            {
                if (_nativeEveryTickHandler == null)
                {
                    _nativeEveryTickHandler = new NativeEveryTickEventHandler();
                    _alt.EveryTick(_nativeEveryTickHandler.GetNativeEventHandler());
                }

                _nativeEveryTickHandler.Add(value);
            }
            remove => _nativeEveryTickHandler?.Remove(value);
        }

        public static event GameEntityCreateEventDelegate OnGameEntityCreate
        {
            add
            {
                if (_nativeGameEntityCreateHandler == null)
                {
                    _nativeGameEntityCreateHandler = new GameEntityCreateEventHandler();
                    _alt.On("gameEntityCreate", _nativeGameEntityCreateHandler.GetNativeEventHandler());
                }

                _nativeGameEntityCreateHandler.Add(value);
            }
            remove => _nativeGameEntityCreateHandler?.Remove(value);
        }

        public static event GameEntityDestroyEventDelegate OnGameEntityDestroy
        {
            add
            {
                if (_nativeGameEntityDestroyHandler == null)
                {
                    _nativeGameEntityDestroyHandler = new GameEntityDestroyEventHandler();
                    _alt.On("gameEntityDestroy", _nativeGameEntityDestroyHandler.GetNativeEventHandler());
                }

                _nativeGameEntityDestroyHandler.Add(value);
            }
            remove => _nativeGameEntityDestroyHandler?.Remove(value);
        }

        public static event KeyDownEventDelegate OnKeyDown
        {
            add
            {
                if (_nativeKeyDownHandler == null)
                {
                    _nativeKeyDownHandler = new KeyDownEventHandler();
                    _alt.OnSimple("keydown", _nativeKeyDownHandler.GetNativeEventHandler());
                }

                _nativeKeyDownHandler.Add(value);
            }
            remove
            {
                _nativeKeyDownHandler?.Remove(value);
            }
        }

        public static event KeyUpEventDelegate OnKeyUp
        {
            add
            {
                if (_nativeKeyUpHandler == null)
                {
                    _nativeKeyUpHandler = new KeyUpEventHandler();
                    _alt.OnSimple("keyup", _nativeKeyUpHandler.GetNativeEventHandler());
                }

                _nativeKeyUpHandler.Add(value);
            }
            remove
            {
                _nativeKeyUpHandler?.Remove(value);
            }
        }

        public static void Init(object wrapper)
        {
            Init(wrapper, new PlayerFactory());
        }

        public static void Init(object wrapper, IBaseObjectFactory<IPlayer> playerFactory)
        {
            PlayerPool = new BaseObjectPool<IPlayer>(playerFactory);
            var jsWrapper = (JSObject)wrapper;
            _alt = new NativeAlt((JSObject)jsWrapper.GetObjectProperty("alt"));
            Natives = new NativeNatives((JSObject)jsWrapper.GetObjectProperty("natives"));
            LocalStorage = new NativeLocalStorage((JSObject)jsWrapper.GetObjectProperty("LocalStorage"));
            Player = new NativePlayer((JSObject)jsWrapper.GetObjectProperty("Player"), PlayerPool);
            HandlingData = new NativeHandlingData((JSObject)jsWrapper.GetObjectProperty("HandlingData"));
            AreaBlip = new NativeAreaBlip((JSObject)jsWrapper.GetObjectProperty("AreaBlip"));
            RadiusBlip = new NativeRadiusBlip((JSObject)jsWrapper.GetObjectProperty("RadiusBlip"));
            PointBlip = new NativePointBlip((JSObject)jsWrapper.GetObjectProperty("PointBlip"));
            WebView = new NativeWebView((JSObject)jsWrapper.GetObjectProperty("WebView"));
        }

        public static void Log(string message,params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                _alt.Log(message + " ["+ string.Join(" , ", args)+" ]");
            }
            else
                _alt.Log(message);
        }

        public static void LogError(string message)
        {
            _alt.LogError(message);
        }

        public static void LogWarning(string message)
        {
            _alt.LogWarning(message);
        }

        public static void Emit(string eventName, params object[] args)
        {
            _alt.Emit(eventName, args);
        }

        public static void EmitServer(string eventName, params object[] args)
        {
            _alt.EmitServer(eventName, args);
        }

        public static void OnServer(string eventName, ServerEventDelegate serverEventDelegate)
        {
            if (!NativeServerEventHandlers.TryGetValue(eventName, out var nativeEventHandler))
            {
                nativeEventHandler = new NativeServerEventHandler(eventName);
                _alt.OnServer(eventName, nativeEventHandler.GetNativeEventHandler());
                NativeServerEventHandlers[eventName] = nativeEventHandler;
            }

            nativeEventHandler.Add(serverEventDelegate);
        }

        public static void OffServer(string eventName, ServerEventDelegate serverEventDelegate)
        {
            if (!NativeServerEventHandlers.TryGetValue(eventName, out var nativeEventHandler))
            {
                return;
            }

            nativeEventHandler.Remove(serverEventDelegate);
        }

        public static void On(string eventName, ServerEventDelegate serverEventDelegate)
        {
            if (!NativeEventHandlers.TryGetValue(eventName, out var nativeEventHandler))
            {
                nativeEventHandler = new NativeServerEventHandler(eventName);
                _alt.On(eventName, nativeEventHandler.GetNativeEventHandler());
                NativeEventHandlers[eventName] = nativeEventHandler;
            }

            nativeEventHandler.Add(serverEventDelegate);
        }

        public static void Off(string eventName, ServerEventDelegate serverEventDelegate)
        {
            if (!NativeEventHandlers.TryGetValue(eventName, out var nativeEventHandler))
            {
                return;
            }

            nativeEventHandler.Remove(serverEventDelegate);
        }

        public static void AddGxtText(string key, string value)
        {
            _alt.AddGxtText(key, value);
        }

        public static void BeginScaleformMovieMethodMinimap(string methodName)
        {
            _alt.BeginScaleformMovieMethodMinimap(methodName);
        }

        public static bool GameControlsEnabled()
        {
            return _alt.GameControlsEnabled();
        }

        public static Vector2 GetCursorPos()
        {
            return _alt.GetCursorPos();
        }

        public static string GetGxtText(string key)
        {
            return _alt.GetGxtText(key);
        }

        public static string GetLicenseHash()
        {
            return _alt.GetLicenseHash();
        }

        public static string GetLocale()
        {
            return _alt.GetLocale();
        }

        public static int GetMsPerGameMinute()
        {
            return _alt.GetMsPerGameMinute();
        }

        public static int GetStat(string statName)
        {
            return _alt.GetStat(statName);
        }

        public static int Hash(string hashString)
        {
            if (string.IsNullOrEmpty(hashString)) return 0;

            var characters = Encoding.UTF8.GetBytes(hashString.ToLower());

            int hash = 0;

            foreach (var c in characters)
            {
                hash += c;
                hash += hash << 10;
                hash ^= hash >> 6;
            }

            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;

            return hash;
        }

        public static bool IsConsoleOpen()
        {
            return _alt.IsConsoleOpen();
        }

        public static bool IsInSandbox()
        {
            return _alt.IsInSandbox();
        }

        public static bool IsMenuOpen()
        {
            return _alt.IsMenuOpen();
        }

        public static bool IsTextureExistInArchetype(int modelHash, string modelName)
        {
            return _alt.IsTextureExistInArchetype(modelHash, modelName);
        }

        public static void LoadModel(int modelHash)
        {
            _alt.LoadModel(modelHash);
        }

        public static void LoadModelAsync(int modelHash)
        {
            _alt.LoadModelAsync(modelHash);
        }

        public static void RemoveGxtText(string key)
        {
            _alt.RemoveGxtText(key);
        }

        public static void RemoveIpl(string iplName)
        {
            _alt.RemoveIpl(iplName);
        }

        public static void RequestIpl(string iplName)
        {
            _alt.RequestIpl(iplName);
        }

        public static void ResetStat(string statName)
        {
            _alt.ResetStat(statName);
        }

        /**
         * Remarks: Only available in sandbox mode
         */
        public static bool SaveScreenshot(string stem)
        {
            return _alt.SaveScreenshot(stem);
        }

        public static void SetCamFrozen(bool state)
        {
            _alt.SetCamFrozen(state);
        }

        public static void SetCursorPos(Vector2 pos)
        {
            _alt.SetCursorPos(pos);
        }

        public static void SetModel(string modelName)
        {
            _alt.SetModel(modelName);
        }

        public static void SetMsPerGameMinute(int ms)
        {
            _alt.SetMsPerGameMinute(ms);
        }

        public static void SetStat(string statName, int value)
        {
            _alt.SetStat(statName, value);
        }

        public static void SetWeatherCycle(int[] weathers, int[] multipliers)
        {
            _alt.SetWeatherCycle(weathers, multipliers);
        }

        public static void SetWeatherSyncActive(bool isActive)
        {
            _alt.SetWeatherSyncActive(isActive);
        }

        /**
         * Remarks:
         * This is handled by resource scoped internal integer, which gets increased/decreased by every function call. 
         * When you show your cursor 5 times, to hide it you have to do that 5 times accordingly.
         */
        public static void ShowCursor(bool state)
        {
            _alt.ShowCursor(state);
        }

        public static void ToggleGameControls(bool state)
        {
            _alt.ToggleGameControls(state);
        }

        public static object CreateVector3(float x, float y, float z)
        {
            return _alt.CreateVector3(x, y, z);
        }

        public static object CreateVector3(Vector3 v)
        {
            return _alt.CreateVector3(v.X, v.Y, v.Z);
        }

        public static IVehicle GetVehicleByScriptID(int id)
        {
            JSObject rVal = ((Function)_alt.alt.GetObjectProperty("getVehicleByScriptID")).Call(null, id) as JSObject;
            if (rVal != null)
                return new Vehicle(rVal);
            return null;
        }
        public static IVehicle GetVehicleByID(int id)
        {
            JSObject rVal = ((Function)_alt.alt.GetObjectProperty("getVehicleByID")).Call(null, id) as JSObject;
            if (rVal != null)
                return new Vehicle(rVal);
            return null;
        }


        public static IPlayer GetPlayerByID(int id)
        {
            JSObject rVal = ((Function)_alt.alt.GetObjectProperty("getPlayerByID")).Call(null, id) as JSObject;
            if (rVal != null)
                return new Player(rVal);
            return null;
        }

        public static IPlayer GetPlayerByScriptID(int id)
        {
            JSObject rVal = ((Function)_alt.alt.GetObjectProperty("getPlayerByScriptID")).Call(null, id) as JSObject;
            if (rVal != null)
                return new Player(rVal);
            return null;
        }

        public static void RegisterAsyncCallBack(string eventName, NativeArgEventDelegate<int> serverEventDelegate)
        {
            ((Function)_alt.alt.GetObjectProperty("onAsync")).Call(null, eventName, serverEventDelegate);
        }

        public static void LoadAnimDictAsync(string dict, int asyncId)
        {
            ((Function)_alt.alt.GetObjectProperty("loadAnimDictAsync")).Call(null, dict, asyncId);
        }

        public static void LoadModelAsync(int model, int asyncId)
        {
            ((Function)_alt.alt.GetObjectProperty("loadModelAsync")).Call(null, model, asyncId);
        }


        public static int SetTimeout(System.Action action, int timeout) => _alt.SetTimeout(action, timeout);
        public static void ClearTimeout(int handle) => _alt.ClearTimeout(handle);
        public static int SetInterval(System.Action action, int timeout) => _alt.SetInterval(action, timeout);
        public static void ClearInterval(int handle) => _alt.ClearInterval(handle);
    }
}