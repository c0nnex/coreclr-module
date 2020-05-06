using System;
using AltV.Net.Client.Events;
using Array = WebAssembly.Core.Array;

namespace AltV.Net.Client.EventHandlers
{
    internal class NativeServerEventHandler : NativeEventHandler<NativeEventDelegate, ServerEventDelegate>
    {
        private readonly NativeEventDelegate nativeEventDelegate;
        private string eventName;

        public NativeServerEventHandler(string name)
        {
            eventName = name;
            nativeEventDelegate = new NativeEventDelegate(OnNativeEvent);
        }

        public void OnNativeEvent(Array nativeArgs)
        {
            try
            {
                var scriptEventHandler = EventHandlers.First;
                object[] args;
                if (nativeArgs != null)
                {
                    var length = nativeArgs.Length;
                    args = new object[length];
                    for (var i = 0; i < length; i++)
                    {
                        args[i] = nativeArgs[i];
                    }
                }
                else
                {
                    args = new object[0];
                }
                Alt.Log("OnServer " + eventName, args);
                while (scriptEventHandler != null)
                {
                    scriptEventHandler.Value(args);
                    scriptEventHandler = scriptEventHandler.Next;
                }
            }
            catch (Exception exception)
            {
                Alt.LogError("Exception in event handler:" + exception);
            }
        }

        public override NativeEventDelegate GetNativeEventHandler()
        {
            return nativeEventDelegate;
        }
    }
}