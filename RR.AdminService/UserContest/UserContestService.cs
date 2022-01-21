using Microsoft.EntityFrameworkCore;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public class UserContestService : IUserContestService
     {
          #region Constructor
          private readonly IRepository<Contest, RankRideAdminContext> _repoContest;
          private readonly IRepository<UserDetail, RankRideContext> _repoUsers;
          private readonly IRepository<JoinedContest, RankRideContext> _repoJoinContest;
          private readonly IRepository<ContestUserWinner, RankRideContext> _repoUsersWinners;
          private readonly IRepository<ContestWinner, RankRideAdminContext> _repoContestWinners;
          private readonly IRepository<Team, RankRideContext> _repoTeam;
          private readonly IRepository<Award, RankRideAdminContext> _repoAward;

          public UserContestService(IRepository<Contest, RankRideAdminContext> repoContest, IRepository<UserDetail, RankRideContext> repoUsers, IRepository<JoinedContest, RankRideContext> repoJoinContest, IRepository<ContestUserWinner, RankRideContext> repoUsersWinners, IRepository<Team, RankRideContext> repoTeam, IRepository<ContestWinner, RankRideAdminContext> repoContestWinners, IRepository<Award, RankRideAdminContext> repoAward)
          {
               _repoContest = repoContest;
               _repoUsers = repoUsers;
               _repoJoinContest = repoJoinContest;
               _repoUsersWinners = repoUsersWinners;
               _repoTeam = repoTeam;
               _repoContestWinners = repoContestWinners;
               _repoAward = repoAward;

          }
          #endregion

          /// <summary>
          /// Get All User of A given Contest
          /// </summary>
          /// <param name="start"></param>
          /// <param name="length"></param>
          /// <param name="column"></param>
          /// <param name="searchStr"></param>
          /// <param name="sort"></param>
          /// <param name="contestId"></param>
          /// <returns></returns>   
          public async Task<Tuple<List<UserContestDto>, int>> GetAlluserbyContest(int start, int length, int column, string searchStr = "", string sort = "", long contestId = 0)
          {
               int count = 0;

               //repo and contests
               var users = _repoJoinContest.Query()
                    .Includes(t => t.Include(tt => tt.Team)
                    .ThenInclude(tcc => tcc.User)
                    .ThenInclude(ud => ud.UserDetail));

               users = users.Filter(x => x.ContestId == contestId && (x.User.UserDetail.FirstOrDefault().FirstName.Contains(searchStr.ToLower()) ||
              x.User.UserDetail.FirstOrDefault().Email.Contains(searchStr.ToLower()) ||
              x.User.UserDetail.FirstOrDefault().Address1.Contains(searchStr.ToLower()) ||
              x.User.UserDetail.FirstOrDefault().PhoneNumber.Contains(searchStr.ToLower())));

               switch (column)
               {
                    case 0:
                         users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.User.UserDetail.FirstOrDefault().FirstName)) : users.OrderBy(x => x.OrderBy(xx => xx.User.UserDetail.FirstOrDefault().FirstName)));
                         break;
                    case 1:
                         users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.User.UserDetail.FirstOrDefault().Email)) : users.OrderBy(x => x.OrderBy(xx => xx.User.UserDetail.FirstOrDefault().Email)));
                         break;
                    case 2:
                         users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.User.UserDetail.FirstOrDefault().Address1)) : users.OrderBy(x => x.OrderBy(xx => xx.User.UserDetail.FirstOrDefault().Address1)));
                         break;
                    case 3:
                         users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.User.UserDetail.FirstOrDefault().PhoneNumber)) : users.OrderBy(x => x.OrderBy(xx => xx.User.UserDetail.FirstOrDefault().PhoneNumber)));
                         break;
                    default:
                         users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.User.UserDetail.FirstOrDefault().FirstName)) : users.OrderBy(x => x.OrderBy(xx => xx.User.UserDetail.FirstOrDefault().FirstName)));
                         break;
               }

               var usercontest = new Tuple<List<UserContestDto>, int>(users.Get().GroupBy(x => x.UserId).Select(x => x.FirstOrDefault()).Select(item => new UserContestDto
               {
                    TeamPoint = item.Team?.TeamPoint,
                    UserId = item.User.UserDetail.FirstOrDefault()?.Id,
                    ContestId = item.ContestId,
                    Name = item.User.UserDetail.FirstOrDefault()?.FirstName + item.User.UserDetail.FirstOrDefault().LastName,
                    Email = item.User.UserDetail.FirstOrDefault()?.Email,
                    PhoneNumber = item.User.UserDetail.FirstOrDefault()?.PhoneNumber,
                    Address = item.User.UserDetail.FirstOrDefault()?.Address1,
                    IsActive = item.User.UserDetail.FirstOrDefault()?.IsActive
               }).ToList(), users.Get().GroupBy(x => x.UserId).Select(x => x.FirstOrDefault()).Count());

               //Calculate rank for joined team for contest.
               //var userContestDto = new List<UserContestDto>();
               var userContestDto = (from a in usercontest.Item1.OrderByDescending(x => x.TeamPoint)
                                     select new
                                     {
                                          Rank = usercontest.Item1.Count(x => x.TeamPoint > a.TeamPoint) + 1,
                                          a.ContestId,
                                          a.UserId,
                                          a.Name,
                                          a.TeamPoint,
                                          a.PhoneNumber,
                                          a.Address,
                                          a.IsActive,
                                          a.Email
                                     });

               return await Task.FromResult(new Tuple<List<UserContestDto>, int>(userContestDto.Select(item => new UserContestDto
               {
                    Address = item.Address,
                    ContestId = item.ContestId,
                    Email = item.Email,
                    IsActive = item.IsActive,
                    Name = item.Name,
                    PhoneNumber = item.PhoneNumber,
                    Rank = item.Rank,
                    TeamPoint = item.TeamPoint,
                    UserId = item.UserId
               }).ToList(), usercontest.Item2));

          }

          /// <summary>
          /// Get Contest Winners
          /// </summary>
          /// <param name="contestId"></param>
          /// <returns></returns>
          public async Task<IEnumerable<UserContestWinnerDto>> GetContestWinnerUsers(long contestId = 0)
          {
               List<ContestUserWinner> winnerList = _repoUsersWinners.Query().Get().ToList();
               List<Team> teamList = _repoTeam.Query().Get().ToList();
               List<UserDetail> userList = _repoUsers.Query().Get().ToList();
               List<ContestWinner> prizeList = _repoContestWinners.Query().Get().ToList();
               List<Contest> contestList = _repoContest.Query().Get().ToList();

               var usersWinnersData = (from winner in winnerList
                                       join team in teamList
                                       on winner.TeamId equals team.Id
                                       join user in userList
                                       on winner.UserId equals user.UserId
                                       join price in prizeList
                                       on winner.ContestWinnerId equals price.Id
                                       join contest in contestList
                                       on winner.ContestId equals contest.Id

                                       orderby winner.Id ascending
                                       select new UserContestWinnerDto
                                       {
                                            ContestId = contest.Id,
                                            PlayerName = user.FirstName + user.LastName,
                                            Email = user.Email,
                                            TeamPoint = team.TeamPoint,
                                            TeamRank = winner.TeamRank,
                                            WinningToken = GetWinningToken(price.TokenPercentage, contest.WinningToken),
                                            WinningPrice = GetWinningPrice(price.PricePercentage, contest.WinningPrice),
                                            Marchendise = GetMerchendiseAward(price.Marchendise),
                                            Others = GetMerchendiseAward(price.OtherReward),
                                       }
                                 ).Where(x => x.ContestId == contestId);

               return await Task.FromResult(usersWinnersData);
          }
          /// <summary>
          /// Get Winning Price
          /// </summary>
          /// <param name="percentage"></param>
          /// <param name="TotalPrice"></param>
          /// <returns></returns>
          public string GetWinningPrice(int percentage, decimal TotalPrice)
          {
               string price = "";
               try
               {
                    if (percentage > 0)
                    {
                         var Amount = (TotalPrice * percentage) / 100;
                         price = "$" + Amount;
                    }
               }
               catch (Exception ex)
               {
               }
               return price;
          }
          /// <summary>
          /// Get Winning Token
          /// </summary>
          /// <param name="percentage"></param>
          /// <param name="TotalToken"></param>
          /// <returns></returns>
          public string GetWinningToken(int percentage, decimal TotalToken)
          {
               string price = "";
               try
               {
                    if (percentage > 0)
                    {
                         var Token = (TotalToken * percentage) / 100;
                         price = "T" + Token;
                    }
               }
               catch (Exception ex)
               {
               }
               return price;
          }
          /// <summary>
          /// Get MerchendiseAwardname
          /// </summary>
          /// <param name="Id"></param>
          /// <returns></returns>
          public string GetMerchendiseAward(long? Id)
          {
               string award = "";

               try
               {
                    if (Id.HasValue)
                    {
                         var awardData = _repoAward.Query().Filter(x => x.Id == Id.Value && x.IsDelete == false).Get().FirstOrDefault();

                         if (awardData != null)
                         {

                              award = awardData.Message;
                         }
                    }
               }
               catch (Exception ex)
               {
                    return ex.ToString();
               }

               return award;
          }
          /// <summary>
          /// Dispose User Service
          /// </summary>
          public void Dispose()
          {
               if (_repoContest != null)
               {
                    _repoContest.Dispose();
               }
               if (_repoUsers != null)
               {
                    _repoUsers.Dispose();
               }
               if (_repoJoinContest != null)
               {
                    _repoJoinContest.Dispose();
               }
               if (_repoUsersWinners != null)
               {
                    _repoUsersWinners.Dispose();
               }
               if (_repoTeam != null)
               {
                    _repoTeam.Dispose();
               }
               if (_repoContestWinners != null)
               {
                    _repoContestWinners.Dispose();
               }
               if (_repoAward != null)
               {
                    _repoAward.Dispose();
               }
          }



     }
}
