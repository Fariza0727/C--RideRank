using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
     public class ContestUserWinnerService : IContestUserWinnerService
     {
          #region ctor

          private readonly IRepository<ContestUserWinner, RankRideContext> _contestUserWinner;
          private readonly IRepository<ContestWinner, RankRideAdminContext> _contestWinner;
          private readonly IRepository<Award, RankRideAdminContext> _repoAward;
          private readonly IRepository<UserDetail, RankRideContext> _repoUserDetail;
        private readonly AppSettings _appsetting;

        public ContestUserWinnerService(IRepository<ContestUserWinner, RankRideContext> contestUserWinner,
              IRepository<ContestWinner, RankRideAdminContext> contestWinner,
              IRepository<Award, RankRideAdminContext> repoAward,
              IRepository<UserDetail, RankRideContext> repoUserDetail,
              IOptions<AppSettings> appsetting)
          {
               _contestUserWinner = contestUserWinner;
               _contestWinner = contestWinner;
               _repoAward = repoAward;
               _repoUserDetail = repoUserDetail;
            _appsetting = appsetting.Value;
        }

          #endregion

          /// <summary>
          /// save contest winner based on team rank
          /// </summary>
          /// <param name="userWinnerDto"></param>
          /// <returns></returns>
          public async Task SaveContestWinner(List<ContestUserWinnerDto> userWinnerDto)
          {
               #region delete previous entry

               long contestId = userWinnerDto.Count > 0 ? userWinnerDto[0].ContestId : 0;
               var previousEntry = _contestUserWinner.Query().Filter(x => x.ContestId == contestId).Get().ToList();
               if (previousEntry.Count > 0)
               {
                    foreach (var item in previousEntry)
                    {
                         try
                         {
                              await AddEditTokenInUserWallet(item.UserId, item.ContestId, item.ContestWinnerId, "delete");
                              await _contestUserWinner.DeleteAsync(item.Id);
                         }
                         catch (Exception ex)
                         {

                              throw;
                         }
                    }
               }

               #endregion

               #region Make new Entry

               var userWinners = userWinnerDto.Select(x => new ContestUserWinner()
               {
                    ContestId = x.ContestId,
                    ContestWinnerId = x.ContestWinnerId,
                    CreatedDate = x.CreatedDate,
                    EventId = x.EventId,
                    TeamId = x.TeamId,
                    TeamRank = x.TeamRank,
                    UserId = x.UserId
               }).ToList();

               _contestUserWinner.InsertCollection(userWinners);

               foreach (var item in userWinners)
               {
                    await AddEditTokenInUserWallet(item.UserId, item.ContestId, item.ContestWinnerId, "add");
               }

               #endregion
          }

          /// <summary>
          /// Get users winnings
          /// </summary>
          /// <param name="userId"></param>
          /// <returns></returns>
          public async Task<List<ContestUserWinnerDto>> GetUserWinnings(string userId)
          {
               List<ContestUserWinnerDto> rewards = new List<ContestUserWinnerDto>();

               try
               {
                    var winnings = _contestUserWinner
                        .Query()
                        .Filter(x => x.UserId == userId)
                        .OrderBy(x =>
                            x.OrderByDescending(y => y.CreatedDate))
                        .Get()
                        .ToList();

                    foreach (var item in winnings)
                    {
                         var contestWinner = _contestWinner
                             .Query()
                             .Filter(x => x.Id == item.ContestWinnerId)
                             .Includes(xx => xx.Include(c => c.Contest))
                             //.Include(c => c.Award)
                             //.ThenInclude(cc => cc.AwardType)
                             .Get()
                             .FirstOrDefault();
                         if (contestWinner != null)
                         {
                              ContestUserWinnerDto reward = new ContestUserWinnerDto();
                              reward.Price = GetWinningPrice(contestWinner.PricePercentage, contestWinner.Contest.WinningPrice);
                              reward.Token = GetWinningToken(contestWinner.TokenPercentage, contestWinner.Contest.WinningToken);
                              reward.Merchendise = GetMercendiseAward(contestWinner.Marchendise);
                              reward.AwardMessage = GetMercendiseAward(contestWinner.OtherReward);
                              reward.ContestId = contestWinner.ContestId;
                              reward.UserId = item.UserId;
                              reward.TeamId = item.TeamId;

                              reward.CreatedDate = item.CreatedDate;
                              reward.ContestTitle = contestWinner.Contest.Title;
                              reward.ContestWinningTitle = contestWinner.Contest.WinningTitle;
                              reward.TeamRank = item.TeamRank;
                              rewards.Add(reward);
                         }
                    }
               }
               catch (Exception ex)
               {
               }

               return await Task.FromResult(rewards);
          }


          /// <summary>
          /// Dispose Team And Event Draw Service 
          /// </summary>
          public void Dispose()
          {
               if (_contestUserWinner != null)
               {
                    _contestUserWinner.Dispose();
               }
          }

          /// <summary>
          /// Set user wallet token on winning.
          /// </summary>
          /// <param name="UserID"></param>
          /// <param name="Token"></param>
          /// <param name="operation"></param>
          /// <param name="contestId"></param>
          /// <param name="contestWinnerId"></param>
          /// <returns></returns>
          public async Task AddEditTokenInUserWallet(string UserID, long contestId, long contestWinnerId, string operation)
          {
               if (string.IsNullOrEmpty(operation) || operation == "delete")
               {
                    var contestWinner = _contestWinner.Query().Filter(x => x.Id == contestWinnerId).Includes(x => x.Include(y => y.Contest)).Get().FirstOrDefault();
                    if (contestWinner != null)
                    {
                         if (contestWinner.Contest != null && contestWinner.Contest.WinningToken > 0)
                         {
                              if (contestWinner.TokenPercentage > 0)
                              {
                                   var tokens = (contestWinner.Contest.WinningToken * contestWinner.TokenPercentage) / 100;
                                   if (tokens > 0)
                                   {
                                        await UpdateUserWallet(UserID, Convert.ToInt32(tokens), "");
                                   }
                              }
                         }
                    }
               }
               else //add in wallet
               {
                    var contestWinner = _contestWinner.Query().Filter(x => x.Id == contestWinnerId).Includes(x => x.Include(y => y.Contest)).Get().FirstOrDefault();
                    if (contestWinner != null)
                    {
                         if (contestWinner.Contest != null && contestWinner.Contest.WinningToken > 0)
                         {
                              if (contestWinner.TokenPercentage > 0)
                              {
                                   var tokens = (contestWinner.Contest.WinningToken * contestWinner.TokenPercentage) / 100;
                                   if (tokens > 0)
                                   {
                                        await UpdateUserWallet(UserID, Convert.ToInt32(tokens), "add");
                                   }
                              }
                         }
                    }
               }
          }



          /// <summary>
          /// Add/deduct token from wallet.
          /// </summary>
          /// <param name="UserID"></param>
          /// <param name="Token"></param>
          /// <param name="type"></param>
          /// <returns></returns>
          public async Task UpdateUserWallet(string userID, int token, string type)
          {
               var User = _repoUserDetail.Query().Filter(x => x.UserId == userID).Get().FirstOrDefault();
               if (User != null)
               {
                    if (type == "add")
                    {
                         var totalToken = (User.WalletToken.HasValue ? User.WalletToken.Value : 0) + token;
                         User.WalletToken = totalToken;
                    }
                    else
                    {
                         if (User.WalletToken.HasValue)
                              if (User.WalletToken.Value > 0 && User.WalletToken.Value >= token)
                              {
                                   var totalToken = User.WalletToken.Value - token;
                                   User.WalletToken = totalToken;
                              }
                    }
                    await _repoUserDetail.UpdateAsync(User);
               }
          }

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
          public string GetMercendiseAward(long? Id)
          {
               string award = "";

               try
               {
                    if (Id.HasValue)
                    {
                         var awardData = _repoAward.Query().Filter(x => x.Id == Id.Value && x.IsDelete == false).Get().FirstOrDefault();
                         if (awardData != null)
                         {
                              award = awardData.Message + (!string.IsNullOrEmpty(awardData.Token) ? (". Use " + awardData.Token + " to avail this reward.") : "");
                            //award = string.Concat($"You have 14 day's to clame your prize. please contact us: {_appsetting.CustomerServiceEmail}");
                         }
                    }
               }
               catch (Exception ex)
               {
               }

               return award;
          }

     }
}
