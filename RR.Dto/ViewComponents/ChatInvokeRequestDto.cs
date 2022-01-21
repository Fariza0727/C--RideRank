namespace RR.Dto
{
    public class ChatInvokeRequestDto
    {
       
        public ChatInvokeRequestDto(string userId = "", bool isSeen = false)
        {
            connectedUserId = userId;
            IsSeen = isSeen;
        }

        public ChatInvokeRequestDto(string aspuserId, long contestId)
        {
            ContestId = contestId;
            AspUserId = aspuserId;
        }

        public string connectedUserId { get; set; }
        public bool IsSeen { get; set; }

        public long ContestId { get; set; }
        public long EventId { get; set; }
        public string AspUserId { get; set; }
    }

    public class ChatUserInvokeRequestDto
    {
        public string AspUserId { get; set; }
        public long ContestId { get; set; }
        public long EventId { get; set; }
        public string ContestName { get; set; }
        public string EventName { get; set; }
    }
}
