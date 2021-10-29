namespace EInvoice.Model
{
    public enum ReceiverType
    {
        P,
        B,
        F
    }
    public static class ReceiverTypeExtensions
    {
        public static ReceiverType ToReceiverType(this string enumName)
        {
            switch (enumName)
            {
                case "P":
                    return ReceiverType.P;
                case "B":
                    return ReceiverType.B;
                case "F":
                    return ReceiverType.F;
                default:
                    throw new InvalidIssuerReceiverTypeException($"Invalid Receiver Type {enumName}");
            }
        }
        public static string ToString(this ReceiverType receiverType)
        {
            switch (receiverType)
            {
                case ReceiverType.B:
                    return "B";
                case ReceiverType.P:
                    return "P";
                case ReceiverType.F:
                    return "F";
                default:
                    return "";
            }
        }
    }
}
