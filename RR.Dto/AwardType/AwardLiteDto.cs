namespace RR.Dto
{
     public class AwardLiteDto
     {
          /// <summary>
          /// ContestId
          /// </summary>
          public long ContestId { get; set; }

          /// <summary>
          /// AwardName
          /// </summary>
          public string AwardName { get; set; }

          /// <summary>
          /// RankTo
          /// </summary>
          public int RankTo { get; set; }

          /// <summary>
          /// RankFrom
          /// </summary>
          public int RankFrom { get; set; }

          /// <summary>
          /// Price
          /// </summary>
          public string Price { get; set; }

          /// <summary>
          /// Token
          /// </summary>
          public string Token { get; set; }

          /// <summary>
          /// Merchendise
          /// </summary>
          public string Merchendise { get; set; }
     }

    public class AwardLiteDescriptionDto
    {
        public int PlayerOfContest { get; set; }
        public string Title { get; set; }
        public long Token { get; set; }
        public decimal Price { get; set; }
    }
}
