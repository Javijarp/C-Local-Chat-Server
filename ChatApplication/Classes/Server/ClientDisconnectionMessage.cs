using FileClasses;

namespace ServerClasses
{
    public class ClientDisconnectionMessage : Message
    {
        public ClientDisconnectionMessage(DateOnly date, TimeOnly time, string emitter) : base(date, time, emitter)
        {
            this.date = date;
            this.time = time;
            this.emitter = emitter;
            this.message = "disconnected from the server.";
        
        }

        public override string ToString()
        {
            return $"[{this.date}]:[{this.time}] {this.emitter}: {this.message}";
        }
    }
}