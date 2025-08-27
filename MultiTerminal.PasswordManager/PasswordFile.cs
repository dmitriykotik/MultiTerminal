namespace MultiTerminal.PasswordManager
{
    internal class PasswordFile
    {
        public required byte[] PasswordHash { get; set; }
        public required List<PasswordEntry> Entries { get; set; }
    }
}
