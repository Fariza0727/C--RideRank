namespace RR.Dto
{
    public class PayPalResponseDto
    {
        public bool IsSuccess { get; set; }

        public int Result { get; set; }

        public string PnRef { get; set; }

        public string RespMsg { get; set; }

        public string AuthCode { get; set; }

        public string AVSAddr { get; set; }

        public string AVSZip { get; set; }

        public string IAVS { get; set; }

        public string HostCode { get; set; }

        public string ProcAVS { get; set; }

        public string CVV2Match { get; set; }

        public string ProcCVV2 { get; set; }

        public string RespText { get; set; }

        public string AddlMsgs { get; set; }
    }
}