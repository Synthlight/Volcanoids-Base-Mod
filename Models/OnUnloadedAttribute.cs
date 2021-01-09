using System;

namespace Base_Mod.Models {
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class OnUnloadedAttribute : Attribute {
    }
}