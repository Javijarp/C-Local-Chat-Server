using FileClasses;

namespace ServerClasses
{
    public class ClientConnectionMessage : Message
    {
        public ClientConnectionMessage(DateOnly date, TimeOnly time, string emitter) : base(date, time, emitter)
        {
            this.date = date;
            this.time = time;
            this.emitter = emitter;
            this.message = "Connected to the server.";
        
        }

        public override string ToString()
        {
            return $"[{this.date}]:[{this.time}] {this.emitter}: {this.message}";
        }
    }
}