namespace FileClasses
{
    public class Message
    {
        private readonly string message;
        private readonly DateOnly date;
        private readonly TimeOnly time;
        private readonly string emitter;

        public Message(string message, DateOnly date, TimeOnly time, string emitter)
        {
            this.message = message;
            this.date = date;
            this.time = time;
            this.emitter = emitter;
        }

        public override string ToString()
        {
            return $"[{this.date}]:[{this.time}] {this.emitter}: {this.message}";
        }

        public string GetMessage() => this.message;
        public DateOnly Date => this.date;
        public TimeOnly Time => this.time;
        public string GetEmitter() => this.emitter;
    }
}