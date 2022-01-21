using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
    public class RiderRankDto
    {
        public int Id { get; set; }
        public int RiderId { get; set; }
        public string Name { get; set; }
        public int Mounted { get; set; }
        public int WorldRank { get; set; }
    }
}
