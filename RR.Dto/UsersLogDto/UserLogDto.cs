using System;

namespace RR.Dto.UsersLogDto
{
     public class UserLogDto
     {
          public string TableName { get; set; }
          public string Message { get; set; }
          public DateTime LogDate { get; set; }
     }
}
