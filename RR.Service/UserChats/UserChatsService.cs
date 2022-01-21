using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;

namespace RR.Service
{
    public class UserChatsService : IUserChatsService
    {
        private readonly IRepository<UserChatMessages, RankRideContext> _repoUserChats;
        private readonly AppSettings _appSettings;
        private readonly IRepository<UserDetail, RankRideContext> _repoUsers;
        private readonly IRepository<Transaction, RankRideContext> _repoTransaction;
        private readonly IRepository<JoinedContest, RankRideContext> _repoJoinedContest;
        private readonly IHostingEnvironment _env;

        public UserChatsService(IRepository<UserChatMessages, RankRideContext> repoUserChats, 
            IRepository<UserDetail, RankRideContext> repoUsers, IOptions<AppSettings> appSettings,
            IRepository<JoinedContest, RankRideContext> repoJoinedContest,
            IHostingEnvironment env,
            IRepository<Transaction, RankRideContext> repoTransaction)
        {
            _repoUserChats = repoUserChats;
            _appSettings = appSettings.Value;
            _repoUsers = repoUsers;
            _repoTransaction = repoTransaction;
            _repoJoinedContest = repoJoinedContest;
            _env = env;
        }

        public async Task<bool> SendMessage(UserChatMessageDto userChat)
        {
            if(!string.IsNullOrEmpty(userChat.Message) && (!string.IsNullOrEmpty(userChat.ConnectedUserId) || userChat.ContestId >0))
            {
               await _repoUserChats.InsertAsync(new UserChatMessages
                {
                    Message = userChat.Message,
                    ConnectedUserId = userChat.ConnectedUserId,
                    ContestId = userChat.ContestId,
                    UserId = userChat.UserId,
                    CreatedDate = DateTime.Now,
                    IsReceived = userChat.IsReceived,
                });
               return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public Task<bool> SendGroupMessage(UserChatMessageDto userChat)
        {
            if (!string.IsNullOrEmpty(userChat.Message) && !string.IsNullOrEmpty(userChat.ConnectedUserId))
            {
                _repoUserChats.InsertAsync(new UserChatMessages
                {
                    Message = userChat.Message,
                    ContestId = userChat.ContestId,
                    UserId = userChat.UserId,
                    CreatedDate = DateTime.Now,
                });
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<UserConversationDto> GetMessage(string userId, string AspUserId_ConnectedUser, bool isSeened = false)
        {
            UserConversationDto chats = new UserConversationDto();
            if (!string.IsNullOrEmpty(AspUserId_ConnectedUser))
            {
                var Mainuser = _repoUsers.Query().Filter(r => r.UserId == AspUserId_ConnectedUser).Get().SingleOrDefault();

                chats.UserInfo = new ChatUsersLiteDto
                {
                    AspUserId = Mainuser.UserId,
                    Avatar = File.Exists(string.Concat(_env.ContentRootPath, "/", _appSettings.ProfilePicPath, "/", Mainuser.Avtar)) ?
                                string.Concat(_appSettings.ProfilePicPath.Replace("wwwroot", ""), "/", Mainuser.Avtar) : "/images/RR/user-n.png",
                    IsUserOnline = Mainuser.IsUserOnline,
                    Username = Mainuser.UserName
                };

                chats.Chats = _repoUserChats.Query().Filter(d =>
                d.UserId == userId && d.ConnectedUserId == AspUserId_ConnectedUser ||
                d.UserId == AspUserId_ConnectedUser && d.ConnectedUserId == userId).Get().Join(_repoUsers.Query().Get(),
                          rc => rc.UserId,
                          us => us.UserId, (rc, uc) => new { rc, uc }).Select(d => new ChatLiteDto
                          {
                              Id = d.rc.Id,
                              Avatar = File.Exists(string.Concat(_env.ContentRootPath, "/", _appSettings.ProfilePicPath, "/", d.uc.Avtar)) ?
                                string.Concat(_appSettings.ProfilePicPath.Replace("wwwroot", ""), "/", d.uc.Avtar) : "/images/RR/user-n.png",
                              AspUserId = d.uc.UserId,
                              CreatedDate = d.rc.CreatedDate,
                              isSeen = d.rc.IsSeen,
                              IsConnectedUser = (d.rc.UserId == AspUserId_ConnectedUser),
                              Message = d.rc.Message,
                              SeenDate = d.rc.CreatedDate,
                              Username = d.uc.UserName,
                              isReveived = d.rc.IsReceived,
                              ContestId = d.rc.ContestId,
                              ChatAgo = GetDateDeffrience(d.rc.CreatedDate)


                          }).ToList();

                chats.Chats.Where(d => d.isSeen!=true && d.IsConnectedUser).ToList().ForEach(r =>
                {
                    var message = _repoUserChats.Query().Filter(d => d.Id == r.Id).Get().SingleOrDefault();
                    if (message != null)
                    {
                        message.IsReceived = isSeened;
                        message.IsSeen = isSeened;
                        message.LastSeenDate = DateTime.Now;
                        _repoUserChats.Update(message);

                    }
                });
            }
            
            return Task.FromResult(chats);
        }

        public Task<UserConversationDto> GetContestMessage(ChatInvokeRequestDto request)
        {
            UserConversationDto chats = new UserConversationDto();

            if (request!=null && request.ContestId > 0)
            {
                chats.UserInfo = new ChatUsersLiteDto
                {
                    ContestId = request.ContestId,
                    Avatar = "/images/RR/New-logo.png",
                    EventId = request.EventId,
                };

               chats.Chats = _repoUserChats.Query().Filter(d =>
               d.ContestId == request.ContestId).Get().Join(_repoUsers.Query().Get(),
                         rc => rc.UserId,
                         us => us.UserId, (rc, uc) => new { rc, uc }).Select(d => new ChatLiteDto
                         {
                             Id = d.rc.Id,
                             Avatar = File.Exists(string.Concat(_env.ContentRootPath, "/", _appSettings.ProfilePicPath, "/", d.uc.Avtar)) ?
                                string.Concat(_appSettings.ProfilePicPath.Replace("wwwroot", ""), "/", d.uc.Avtar) : "/images/RR/user-n.png",
                             AspUserId = d.uc.UserId,
                             CreatedDate = d.rc.CreatedDate,
                             isSeen = d.rc.IsSeen,
                             Message = d.rc.Message,
                             IsConnectedUser = (d.rc.UserId != request.AspUserId),
                             SeenDate = d.rc.CreatedDate,
                             Username = d.uc.UserName,
                             isReveived = d.rc.IsReceived,
                             ContestId = d.rc.ContestId,
                             ChatAgo = GetDateDeffrience(d.rc.CreatedDate)

                         }).ToList();


              

            }

            return Task.FromResult(chats);
        }

        public Task<List<ChatUsersLiteDto>> GetChatUsers(string AspUserId, ChatUserInvokeRequestDto request = null)
        {
            var users = new List<ChatUsersLiteDto>();

            if (request != null && request.ContestId > 0)
            {

                try
                {

                    var isSelfJoinedcontest = _repoJoinedContest.Query().Filter(m => m.ContestId == request.ContestId && m.UserId == AspUserId).Get().SingleOrDefault();
                    if (isSelfJoinedcontest != null) {
                        users = _repoJoinedContest.Query().Get()
                            .Join(_repoUsers.Query().Get(),
                                    c => c.UserId,
                                    u => u.UserId, (contest, user) => new { contest, user }).Where(r => r.user.IsPaidMember == true)
                            .GroupJoin(_repoUserChats.Query().Filter(r => r.ContestId > 0).Get(),
                                    mb => mb.contest.ContestId,
                                    chat => chat.ContestId, (contest, chat) => new { contest, chat })
                            .Where(m => m.contest.contest.ContestId == request.ContestId && m.contest.user.IsPaidMember == true && m.contest.user.UserId != AspUserId)
                            .Select(u => new ChatUsersLiteDto
                            {
                                AspUserId = u.contest.user.UserId,
                                Avatar = File.Exists(string.Concat(_env.ContentRootPath, "/", _appSettings.ProfilePicPath,"/", u.contest.user.Avtar)) ?
                                string.Concat(_appSettings.ProfilePicPath.Replace("wwwroot",""),"/",u.contest.user.Avtar) : "/images/RR/user-n.png",
                                IsUserOnline = u.contest.user.IsUserOnline,
                                Username = u.contest.user.UserName,
                                LastSeenDate = u.chat.Where(d => d.IsSeen == true).LastOrDefault()?.LastSeenDate,
                            }).ToList();

                        var contestChats = _repoUserChats.Query().Filter(r => r.ContestId > 0).Get();

                        users.Add(new ChatUsersLiteDto
                        {
                            ContestId = request.ContestId,
                            EventId = request.EventId,
                            EventName = request.EventName,
                            ContestName = request.ContestName,
                            isChatRoom = true,
                            chatRoomUsers = users.Select(r => r.Username),
                            Avatar = "/images/RR/New-logo.png",
                            //LastSeenDate = contestChats.Where(d => d.IsSeen == true).LastOrDefault()?.LastSeenDate,
                            //Lastmessage = contestChats.LastOrDefault()?.Message,
                            //Unseend = contestChats.Where(d => d.IsSeen != true).Count()


                        });

                        } }
                catch (Exception ed)
                {


                }
            }
            else
            {

                users = _repoUsers.Query().Get().GroupJoin(_repoUserChats.Query().Filter(r => r.ConnectedUserId == AspUserId).Get(),
                user => user.UserId,
                chat => chat.UserId,
                (user, chat) =>
                  new { user, chat }).Where(m => m.user.IsPaidMember == true && m.user.UserId != AspUserId).Select(u =>
                                new ChatUsersLiteDto
                                {
                                    AspUserId = u.user.UserId,
                                    Avatar = File.Exists(string.Concat(_env.ContentRootPath, "/", _appSettings.ProfilePicPath, "/", u.user.Avtar)) ?
                                string.Concat(_appSettings.ProfilePicPath.Replace("wwwroot", ""), "/", u.user.Avtar) : "/images/RR/user-n.png",
                                    IsUserOnline = u.user.IsUserOnline,
                                    Username = u.user.UserName,
                                    LastSeenDate = u.chat.Where(d => d.IsSeen == true).LastOrDefault()?.LastSeenDate,
                                    Lastmessage = u.chat.LastOrDefault()?.Message,
                                    Unseend = u.chat.Where(d => d.IsSeen != true).Count()

                                }).ToList();

            }

            return Task.FromResult(users);
        }

        private async Task SeenedMessages(long[] messagesId )
        {
            for (int i = 0; i < messagesId.Length; i++)
            {
                var message = _repoUserChats.Query().Filter(d => d.Id == messagesId[i]).Get().SingleOrDefault();
                message.IsReceived = true;
                message.IsSeen = true;
                message.LastSeenDate = DateTime.Now;
                await _repoUserChats.UpdateAsync(message);
            }
            
        }

        public void Dispose()
        {
            if (_repoUserChats != null)
            {
                _repoUserChats.Dispose();
            }
            
        }

        private string GetDateDeffrience(DateTime? date)
        {
            if (date != null)
            {

                var date_ = DateTime.Now;
                TimeSpan diff = date_.Subtract(Convert.ToDateTime(date));

                if (diff.Days > 28)
                    return $"{Math.Round(diff.TotalDays / 28, 0)} month ago";

                if (diff.Days > 0)
                    return $"{diff.Days} days ago";

                if (diff.Hours > 0)
                    return $"{diff.Hours} hours ago";

                if (diff.Minutes > 0)
                    return $"{diff.Minutes} Minutes ago";

                if (diff.Seconds > 0)
                    return $"{diff.Seconds} seconds ago";


            }

            return "";
        }
    }
}
