namespace DP.LSP.Tools.DiskMon.Core
{
    internal struct Recipient
    {
        public string Email { get; private set; }

        public Recipient(string email)
        {
            Email = email;
        }
    }
}
