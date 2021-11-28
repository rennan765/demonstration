using System;

namespace _4oito6.Demonstration.Application.Model
{
    public class Notification : ICloneable
    {
        public Notification(string name, string message)
        {
            Name = name;
            Message = message;
        }

        public string Name { get; private set; }
        public string Message { get; private set; }

        public object Clone()
        {
            return new Notification(Name, Message);
        }
    }
}