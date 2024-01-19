namespace CAP_DataAccess.Components.ComponentDraftMapper.DTOs
{
    public class Connection
    {
        public int fromPinNr { get; set; }
        public int toPinNr { get; set; }
        public string realOrFormula { get; set; }
        public double imaginary { get; set; }
    }
}
