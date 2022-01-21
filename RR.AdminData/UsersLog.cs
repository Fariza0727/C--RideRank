using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class UsersLog
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string Message { get; set; }
        public DateTime LogDate { get; set; }
    }
}
