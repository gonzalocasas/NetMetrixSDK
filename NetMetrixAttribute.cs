using System;

namespace NetMetrixSdk
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NetMetrixAttribute : Attribute
    {
        public NetMetrixAttribute(string section)
        {
            Section = section;
        }

        public string Section { get; private set; }
    }
}
