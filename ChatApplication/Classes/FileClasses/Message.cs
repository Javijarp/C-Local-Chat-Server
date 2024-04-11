using System.Runtime.InteropServices.Marshalling;

namespace FileClasses
{
    public class Message
    {
        protected string message;
        protected DateOnly date;
        protected TimeOnly time;
        protected string emitter;

        public Message(string message, DateOnly date, TimeOnly time, string emitter)
        {
            this.message = message;
            this.date = date;
            this.time = time;
            this.emitter = emitter;
        }

        public Message(DateOnly date, TimeOnly time, string emitter)
        {
            this.date = date;
            this.time = time;
            this.emitter = emitter;
        }

        public override string ToString()
        {
            return $"[{this.date}]:[{this.time}] {this.emitter}: {this.message}";
        }

        public string GetMessage() => this.message;
        public DateOnly GetDate() => this.date;
        public TimeOnly GetTime() => this.time;
        public string GetEmitter() => this.emitter;
    }
}