using System;

namespace dialogflow.dotnet.DialogFlow
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IntentAttribute : Attribute
    {
        public string Name { get; private set; }

        public IntentAttribute(string name)
        {
            Name = name;
        }
    } 
}