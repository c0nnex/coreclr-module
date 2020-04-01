using WebAssembly.Core;

namespace AltV.Net.Client.Events
{
    public delegate void NativeEventDelegate(Array args);
    public delegate void NativeArgEventDelegate<T>(T arg0);
    public delegate void NativeArgEventDelegate<T,T1>(T arg0,T1 arg1);
    public delegate void NativeArgEventDelegate<T,T1,T2>(T arg0, T1 arg1, T2 arg2);
}