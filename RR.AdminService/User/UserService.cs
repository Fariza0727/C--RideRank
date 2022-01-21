using Microsoft.EntityFrameworkCore;
using RR.AdminData;
using RR.AdminMapper;
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
    public class UserService : IUserService
    {
        #region Contructor

        private readonly IRepository<UserDetail, RankRideContext> _repoUserDetail;
        private readonly IRepository<ForgetPasswordRequest, RankRideContext> _repoForgetPasswordRequest;
        private readonly IRepository<Data.AspNetUsers, RankRideContext> _repoUsers;
        private readonly IRepository<Data.Transaction, RankRideContext> _repoTransaction;
        private readonly IRepository<Data.JoinedContest, RankRideContext> _repoJoinedContest;
        private readonly IRepository<Data.Team, RankRideContext> _repoTeam;
        private readonly IRepository<Data.TeamBull, RankRideContext> _repoTeamBull;
        private readonly IRepository<Data.TeamRider, RankRideContext> _repoTeamRider;
        private readonly IRepository<Data.ContestUserWinner, RankRideContext> _repoContestWinner;




        public UserService(IRepository<UserDetail, RankRideContext> repoUserDetail,
             IRepository<ForgetPasswordRequest, RankRideContext> repoForgetPasswordRequest, IRepository<Data.AspNetUsers, RankRideContext> repoUsers,
             IRepository<Data.Transaction, RankRideContext> repoTransaction, IRepository<Data.JoinedContest, RankRideContext> repoJoinedContest,
             IRepository<Data.Team, RankRideContext> repoTeam,
             IRepository<Data.TeamBull, RankRideContext> repoTeamBull,
             IRepository<Data.TeamRider, RankRideContext> repoTeamRider,
             IRepository<Data.ContestUserWinner, RankRideContext> repoContestWinner)
        {
            _repoUserDetail = repoUserDetail;
            _repoForgetPasswordRequest = repoForgetPasswordRequest;
            _repoUsers = repoUsers;
            _repoTransaction = repoTransaction;
            _repoJoinedContest = repoJoinedContest;
            _repoTeam = repoTeam;
            _repoTeamBull = repoTeamBull;
            _repoTeamRider = repoTeamRider;
            _repoContestWinner = repoContestWinner;
        }

        #endregion

        /// <summary>
        /// Add Edit UserDetail
        /// </summary>
        /// <param name="detailDto">The UserDetailDto</param>
        /// <returns></returns>
        public async Task AddEditUserDetail(UserDetailDto userDetailDto)
        {
            var user = _repoUserDetail.Query()
                .Filter(o => o.Id == userDetailDto.Id)
                .Get()
                .SingleOrDefault();
            if (user != null)
            {
                user.Address1 = !string.IsNullOrEmpty(userDetailDto.Address1) ? userDetailDto.Address1 : "";
                user.Address2 = !string.IsNullOrEmpty(userDetailDto.Address2) ? userDetailDto.Address2 : "";
                user.Address3 = !string.IsNullOrEmpty(userDetailDto.Address3) ? userDetailDto.Address3 : "";
                user.Avtar = !string.IsNullOrEmpty(userDetailDto.Avtar) ? userDetailDto.Avtar : "";
                user.Banking = !string.IsNullOrEmpty(userDetailDto.Banking) ? userDetailDto.Banking : "";
                user.City = userDetailDto.City;
                user.Country = userDetailDto.Country;
                user.Email = !string.IsNullOrEmpty(userDetailDto.Email) ? userDetailDto.Email : "";
                user.FirstName = !string.IsNullOrEmpty(userDetailDto.FirstName) ? userDetailDto.FirstName : "";
                user.IsActive = userDetailDto.IsActive;
                user.IsBlock = userDetailDto.IsBlock;
                user.IsDelete = userDetailDto.IsDelete;
                user.LastName = !string.IsNullOrEmpty(userDetailDto.LastName) ? userDetailDto.LastName : "";
                user.LeagueNotification = userDetailDto.LeagueNotification;
                user.PhoneNumber = !string.IsNullOrEmpty(userDetailDto.PhoneNumber) ? userDetailDto.PhoneNumber : "";
                user.State = userDetailDto.State;
                user.UpdatedDate = DateTime.Now;
                user.UserName = userDetailDto.UserName;
                user.PlayerType = userDetailDto.PlayerType;
                user.WalletToken = userDetailDto.WalletToken;
                user.ZipCode = userDetailDto.ZipCode;
                await _repoUserDetail.UpdateAsync(user);
            }
        }

        /// <summary>
        /// Get User Detail
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User DetailDto</returns>
        public async Task<UserDetailDto> GetUserDetail(int id)
        {
            var user = _repoUserDetail.Query()
                .Filter(o => o.Id == id)
                .Get()
                .SingleOrDefault();
            return await Task.FromResult(UserDetailMapper.MapDto(user));
        }

        /// <summary>
        /// Get All Exist Users
        /// </summary>
        /// <param name="start">PageNumber</param>
        /// <param name="length">Number Of Record On Single Page</param>
        /// <param name="searchStr">Search string</param>
        /// <param name="column">Column Index</param>
        /// <param name="sort">Order</param>
        /// <returns>List Of UserDetailDto</returns>
        public async Task<Tuple<IEnumerable<UserLiteDetailDto>, int>> GetAllExistUsers(int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;
            var predicate = PredicateBuilder.True<UserDetail>()
           .And(x => (searchStr == "" || x.UserName.ToLower().Contains(searchStr.ToLower())
                    || x.Email.ToLower().Contains(searchStr.ToLower())
                    || x.ShopifyMembership.ToLower().Contains(searchStr.ToLower())
                    || x.PhoneNumber.ToLower().Contains(searchStr.ToLower())));

            var users = _repoUserDetail
                .Query()
                .Includes(ud => ud.Include(u => u.User).ThenInclude(r=>r.UserDetail))
                .Filter(predicate);

            if (FilterSortingVariable.USER_NAME == column)
            {
                users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.UserName)) : users.OrderBy(x => x.OrderBy(xx => xx.UserName)));
            }
            if (FilterSortingVariable.USER_EMAIL == column)
            {
                users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.Email)) : users.OrderBy(x => x.OrderBy(xx => xx.Email)));
            }
            if (FilterSortingVariable.USER_PHONENUMBER == column)
            {
                users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.PhoneNumber)) : users.OrderBy(x => x.OrderBy(xx => xx.PhoneNumber)));
            }
            if (FilterSortingVariable.USER_UPDATEDDATE == column)
            {
                users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.UpdatedDate)) : users.OrderBy(x => x.OrderBy(xx => xx.UpdatedDate)));
            }
            if (FilterSortingVariable.USER_MEMBERSHIP == column)
            {
                users = (sort == "desc" ? users.OrderBy(x => x.OrderByDescending(xx => xx.ShopifyMembership)) : users.OrderBy(x => x.OrderBy(xx => xx.ShopifyMembership)));
            }
            return await Task.FromResult(new Tuple<IEnumerable<UserLiteDetailDto>, int>(users
                    .GetPage(start, length, out count).Select(y => new UserLiteDetailDto
                    {
                        UserName = y.UserName,
                        Email = y.Email,
                        PhoneNumber = y.PhoneNumber,
                        IsActive = y.IsActive,
                        Id = y.Id,
                        LockoutEnd = y.User.LockoutEnd.ToString() /*y.User.AccessFailedCount.ToString()*/,
                        UpdatedDate = y.UpdatedDate.ToString("dd MMM yyyy"),
                        IsEmailConfirm = y.User.EmailConfirmed,
                        Membership = !string.IsNullOrEmpty(y.ShopifyMembership) ? y.ShopifyMembership : y.IsPaidMember == true ? "Premium" : "Free Member",
                        Address = y.User?.UserDetail.FirstOrDefault()?.Address1,
                        IsNotifyEmail = y.User?.UserDetail.FirstOrDefault()?.IsNotifyEmail ?? false,
                        IsNotifySMS = y.User?.UserDetail.FirstOrDefault()?.IsNotifySms ?? false
        }), count));

        }



        /// <summary>
        /// Update User Status
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public async Task UpdateStatus(int userId)
        {
            var user = _repoUserDetail.Query()
                 .Filter(x => x.Id == userId)
                .Get()
                .SingleOrDefault();
            user.IsActive = (user.IsActive == true) ? false : true;
            await _repoUserDetail.UpdateAsync(user);
        }

        public async Task AddForgetPasswordRequest(Guid code, string userId)
        {
            var forgetPasswordRequest = _repoForgetPasswordRequest.Query()
                .Filter(o => o.UserId == userId)
                .Get()
                .SingleOrDefault();
            if (forgetPasswordRequest == null)
            {
                forgetPasswordRequest = new ForgetPasswordRequest
                {
                    UrlId = code.ToString(),
                    UserId = userId
                };
                await _repoForgetPasswordRequest.InsertAsync(forgetPasswordRequest);
            }
            else
            {
                forgetPasswordRequest.UrlId = code.ToString();
                await _repoForgetPasswordRequest.UpdateAsync(forgetPasswordRequest);
            }
        }
        /// <summary>
        /// Unlock User
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<string> Updateuserlockstatus(string userName)
        {

            var user = _repoUsers.Query().Get().Where(x => x.UserName == userName).FirstOrDefault();

            if (user.LockoutEnd != null)
            {
                user.LockoutEnd = null;
                user.AccessFailedCount = 0;
                await _repoUsers.UpdateAsync(user);
                return await Task.FromResult("unlocked");
            }
            else
            {
                try
                {


                    DateTimeOffset targetTime;


                    // Convert to same time (return sourceTime unchanged)
                    targetTime = DateTimeOffset.Now.ToOffset(new TimeSpan(0, 0, 0));


                    user.AccessFailedCount = 5;
                    user.LockoutEnd = targetTime.AddMinutes(330);

                    await _repoUsers.UpdateAsync(user);
                }
                catch (Exception ex)
                {
                    string exe = ex.ToString();
                    throw;
                }
                return await Task.FromResult("locked");
            }


        }

        /// <summary>
        /// Get UserDetail By Email
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        public async Task<UserDetailDto> GetUserDetailbyEmail(string emailId)
        {
            var user = _repoUserDetail.Query()
                .Filter(o => o.Email == emailId)
                .Get()
                .SingleOrDefault();
            return await Task.FromResult(UserDetailMapper.MapDto(user));
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserByID(long id)
        {
            try
            {

                //find AspNet ID
                var userId = _repoUserDetail.Query().
                    Filter(x => x.Id == id).Get().SingleOrDefault().UserId;

                var transactions = _repoTransaction.Query().
                    Filter(x => x.UserId == userId).Get().ToList();

                var joinedcontests = _repoJoinedContest.Query()
                    .Filter(x => x.UserId == userId).Get().ToList();

                //Contest Winner
                var winnings = _repoContestWinner.Query().Filter(x => x.UserId == userId).Get().ToList();

                //Contest Winner
                var teams = _repoTeam.Query().Filter(x => x.UserId == userId).Get().ToList();

                //var teams = 
                //delete Joined Contest Details
                foreach (var item in joinedcontests)
                {
                    await _repoJoinedContest.DeleteAsync(item.PaymentTxnId);
                }
                //delete Transaction Details
                foreach (var item in transactions)
                {
                    await _repoTransaction.DeleteAsync(item.Id);
                }

                //delete winnings for user
                foreach (var item in winnings)
                {
                    await _repoContestWinner.DeleteAsync(item.Id);
                }

                //deleteteams for user
                foreach (var item in teams)
                {
                    //Get Team Bulls
                    var bulls = _repoTeamBull.Query().Filter(x => x.TeamId == item.Id).Get().ToList();
                    var riders = _repoTeamRider.Query().Filter(x => x.TeamId == item.Id).Get().ToList();
                    //delete team bulls
                    foreach (var bull in bulls)
                    {
                        await _repoTeamBull.DeleteAsync(bull.TeamBullId);
                    }
                    //delete team riders
                    foreach (var rider in riders)
                    {
                        await _repoTeamRider.DeleteAsync(rider.TeamRiderId);
                    }
                    await _repoTeam.DeleteAsync(item.Id);
                }


                //Delete User Details
                await _repoUserDetail.DeleteAsync(id);

                //Delete Aspnet Users
                await _repoUsers.DeleteAsync(userId);

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);

            }

        }

        /// <summary>
        /// update user role
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Role"></param>
        /// <returns></returns>
        public async Task<string> UpdateUserRole(string UserId, string Role)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@UserId", UserId);
            param.Add("@UserRole", Role);

            var obj = new KeyValuePair<string, Dictionary<string, object>>("proc_UpdateUserRole", param);
            var response = await _repoUserDetail.ExecuteSPAsync(obj);

            return await Task.FromResult(response.Data.ToString());
        }

        /// <summary>
        /// Dispose User Service
        /// </summary>
        public void Dispose()
        {
            if (_repoUserDetail != null)
            {
                _repoUserDetail.Dispose();
            }
            if (_repoForgetPasswordRequest != null)
            {
                _repoForgetPasswordRequest.Dispose();
            }
        }

        public async Task<Tuple<int, int>> GetUsersAsCard()
        {

            var dateArray = DateTime.Now.DayOfWeek;

            var total_ = _repoUserDetail.Query().Get().Count();
            var active_ = _repoUserDetail.Query().Filter(r=>r.IsActive == true).Get().Count();

            return await Task.FromResult(new Tuple<int, int>(total_, active_));
        }


        public async Task<UserChartDto> GetWeeklyUsersAsCard()
        {
            string[] labels = new string[6];
            int[] series = new int[6];
            int total = 0;

            for (int i = 0; i < 6; i++)
            {
                labels[i] = ((DayOfWeek)i).ToString()[0].ToString();
                series[i] = _repoUserDetail.Query().Filter(r => r.CreatedDate.Date == StartOfWeek(DateTime.Now.AddDays(-5), ((DayOfWeek)i)).Date).Get().Count();
                total = _repoUserDetail.Query().Get().Count();

            }

            return await Task.FromResult(new UserChartDto
            {
                labels = labels,
                series = series,
                parcentage = Math.Round((decimal)(series.Sum() * 100)/total,2).ToString(),
                title = "Players Graph",
                high = series.Max()
            });
        }

        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
