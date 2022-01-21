namespace RR.Dto
{
    public class StateDto
    {
        public string Name { get; set; }
        public bool IsNovice { get; set; }
        public bool IsIntermediate { get; set; }
        public bool IsPro { get; set; }
        public string ErrorMessage { get; set; }
        public string StateCode { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int StateId { get; set; }
    }
}
