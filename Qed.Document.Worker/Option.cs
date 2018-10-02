using CommandLine;

namespace Qed.Document.Worker
{
    public class Option
    {
        [Option('l', "LoanGuid", Required = false, HelpText = "LoanGuid to download documents for")]
        public string LoanGuid { get; set; }
    }
}
