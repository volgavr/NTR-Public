using System;
namespace ClearentScheduler.Exceptions
{
    [Serializable()]
    public class InvoiceNotFoundException : System.Exception
    {
        public InvoiceNotFoundException() : base() { }
        public InvoiceNotFoundException(string message) : base(message) { }
        public InvoiceNotFoundException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected InvoiceNotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
