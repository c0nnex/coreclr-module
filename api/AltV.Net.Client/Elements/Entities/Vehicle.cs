using System.Numerics;
using WebAssembly;
using WebAssembly.Core;

namespace AltV.Net.Client.Elements.Entities
{
    public class Vehicle: Entity, IVehicle
    {
        public int Gear => (int) jsObject.GetObjectProperty("gear");
        
        public int Rpm => (int) jsObject.GetObjectProperty("rpm");
        
        public int Speed => System.Convert.ToInt32( jsObject.GetObjectProperty("speed"));
        
        public Vector3 SpeedVector
        {
            get
            {
                var vector3Obj = (JSObject) jsObject.GetObjectProperty("speedVector");
                return new Vector3((float) vector3Obj.GetObjectProperty("x"), (float) vector3Obj.GetObjectProperty("y"),
                    (float) vector3Obj.GetObjectProperty("z"));
            }
        }

        public int WheelsCount => (int) jsObject.GetObjectProperty("wheelsCount");
        


        internal Vehicle(JSObject jsObject): base(jsObject)
        {
        }

    }
}