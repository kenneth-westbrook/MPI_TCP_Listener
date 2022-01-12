namespace VBS.MPI_TCP_Listener
{
    using System.Text;

    public static class Hl7Utils
    {
        private static string WrapOutgoingMessage(this string message)
        {
            return $"{(char)0x0b}{message}{(char)0x1C}{(char)0x0d}";
        }

        public static byte[] GetEncodedOutput(this string message)
        {
            return Encoding.ASCII.GetBytes(message.WrapOutgoingMessage());
        }
    }
}