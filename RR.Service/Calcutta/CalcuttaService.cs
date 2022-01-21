using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Dto.Calcutta;
using RR.Dto.Contest;
using RR.Dto.Team;
using RR.Mapper;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Capture = PayPalCheckoutSdk.Orders.Capture;

namespace RR.Service
{
    public class CalcuttaService : ICalcuttaService
    {
        #region Constructor

        private readonly IRepository<UserDetail, RankRideContext> _repoUsers;
        private readonly IRepository<CalcuttaEvent, RankRideStaticContext> _repoCalcuttaEvent;
        private readonly IRepository<CalcuttaEventClass, RankRideStaticContext> _repoCalcuttaEventClass;
        private readonly IRepository<CalcuttaEventEntry, RankRideStaticContext> _repoCalcuttaEventEntry;
        private readonly IRepository<CalcuttaEventResult, RankRideStaticContext> _repoCalcuttaEventResult;
        private readonly IRepository<CalcuttaRC, RankRideStaticContext> _repoCalcuttaRC;
        private readonly IRepository<CalcuttaRCEntry, RankRideStaticContext> _repoCalcuttaRCEntry;
        private readonly IRepository<CalcuttaRCResult, RankRideStaticContext> _repoCalcuttaRCResult;
        private readonly IRepository<PayoutBasic, RankRideStaticContext> _repoPayoutBacic;
        private readonly IRepository<UserDetail, RankRideContext> _repoUserDetail;
        private readonly IRepository<ShoppingCart, RankRideContext> _repoShoppingCart;
        private readonly IRepository<TransactionHistory, RankRideContext> _repoTransactionHistory;
        private readonly IRepository<SimpleTeam, RankRideContext> _repoSimpleTeam;
        private readonly IRepository<SimpleTeamBull, RankRideContext> _repoSimpleTeamBull;
        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        public static AppSettings _appSettings;
        private readonly IRiderService _riderService;

        private readonly IHttpContextAccessor _httpContext;

        public CalcuttaService(
             IRepository<UserDetail, RankRideContext> repoUsers,
             IRepository<CalcuttaEvent, RankRideStaticContext> repoCalcuttaEvent,
             IRepository<CalcuttaEventClass, RankRideStaticContext> repoCalcuttaEventClass,
             IRepository<CalcuttaEventEntry, RankRideStaticContext> repoCalcuttaEventEntry,
             IRepository<CalcuttaEventResult, RankRideStaticContext> repoCalcuttaEventResult,
             IRepository<PayoutBasic, RankRideStaticContext> repoPayoutBacic,
             IRepository<UserDetail, RankRideContext> repoUserDetail,
             IRepository<ShoppingCart, RankRideContext> repoShoppingCart,
             IRepository<TransactionHistory, RankRideContext> repoTransactionHistory,
             IRepository<SimpleTeam, RankRideContext> repoSimpleTeam,
             IRepository<SimpleTeamBull, RankRideContext> repoSimpleTeamBull,
             IRepository<CalcuttaRC, RankRideStaticContext> repoCalcuttaRC,
             IRepository<CalcuttaRCEntry, RankRideStaticContext> repoCalcuttaRCEntry,
             IRepository<CalcuttaRCResult, RankRideStaticContext> repoCalcuttaRCResult,
             IRepository<Rider, RankRideStaticContext> repoRider,
             IOptions<AppSettings> appSettings,
             IRiderService riderService,
             IHttpContextAccessor httpContext
            )
        {
            _repoCalcuttaEvent = repoCalcuttaEvent;
            _repoCalcuttaEventClass = repoCalcuttaEventClass;
            _repoCalcuttaEventEntry = repoCalcuttaEventEntry;
            _repoCalcuttaEventResult = repoCalcuttaEventResult;
            _repoCalcuttaRC = repoCalcuttaRC;
            _repoCalcuttaRCEntry = repoCalcuttaRCEntry;
            _repoCalcuttaRCResult = repoCalcuttaRCResult;
            _repoPayoutBacic = repoPayoutBacic;
            _repoUsers = repoUsers;
            _repoUserDetail = repoUserDetail;
            _repoTransactionHistory = repoTransactionHistory;
            _repoSimpleTeam = repoSimpleTeam;
            _repoSimpleTeamBull = repoSimpleTeamBull;
            _repoShoppingCart = repoShoppingCart;
            _repoRider = repoRider;
            _appSettings = appSettings.Value;
            _riderService = riderService;
            _httpContext = httpContext;
        }
        #endregion

        /// <summary>
        /// Dispose All Services of contest
        /// </summary>
        public void Dispose()
        {
            if (_repoUserDetail != null)
            {
                _repoUserDetail.Dispose();
            }

            if (_repoUsers != null)
            {
                _repoUsers.Dispose();
            }
            if (_repoCalcuttaEvent != null)
            {
                _repoCalcuttaEvent.Dispose();
            }
            if (_repoCalcuttaEventClass != null)
            {
                _repoCalcuttaEventClass.Dispose();
            }
            if (_repoCalcuttaEventEntry != null)
            {
                _repoCalcuttaEventEntry.Dispose();
            }
            if (_repoCalcuttaEventResult != null)
            {
                _repoCalcuttaEventResult.Dispose();
            }
            if (_repoTransactionHistory != null)
            {
                _repoTransactionHistory.Dispose();
            }
            if (_repoShoppingCart != null)
            {
                _repoShoppingCart.Dispose();
            }
            if (_repoPayoutBacic != null)
            {
                _repoPayoutBacic.Dispose();
            }
            if (_repoCalcuttaRC != null)
            {
                _repoCalcuttaRC.Dispose();
            }
            if (_repoCalcuttaRCEntry != null)
            {
                _repoCalcuttaRCEntry.Dispose();
            }
            if (_repoCalcuttaRCResult != null)
            {
                _repoCalcuttaRCResult.Dispose();
            }
            if (_repoRider != null)
            {
                _repoRider.Dispose();
            }
            if (_repoSimpleTeam != null)
            {
                _repoSimpleTeam.Dispose();
            }
            if (_repoSimpleTeamBull != null)
            {
                _repoSimpleTeamBull.Dispose();
            }
        }

        public async Task<CalcuttaDetailDto> GetEventDetail(int eventId, string userId)
        {
            CalcuttaDetailDto formDto = new CalcuttaDetailDto();
            formDto.IsFinished = true;
            formDto.Id = eventId;
            var eventDetail = _repoCalcuttaEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();
            if (eventDetail != null)
            {
                formDto.ParentEventId = eventDetail.ParentEventId;
                formDto.StartDate = eventDetail.StartDate;
                formDto.ContestUTCLockTime = eventDetail.ContestUTCLockTime;
                formDto.Title = eventDetail.Title;
                formDto.City = eventDetail.City;
                formDto.State = eventDetail.State;
                formDto.ContestType = eventDetail.ContestType;
                formDto.ContestStatus = eventDetail.ContestStatus;

                /*if (eventDetail.ContestUTCLockTime > DateTime.UtcNow)
                    formDto.IsFinished = false;
                */
            }
            formDto.EntryList = new List<CalcuttaEventEntryLiteDto>();
            
            formDto.ResultList = new List<CalcuttaEventResultLiteDto>();

            #region EntryList
            var resultEntries = _repoCalcuttaEventResult.Query().Filter(x => x.ParentEventId == formDto.ParentEventId).Get().ToList();

            formDto.ClassList = _repoCalcuttaEventClass.Query()
                .Filter(x => x.ParentEventId == formDto.ParentEventId && x.EventType.ToLower() != "all in")
                .Get()
                .Select(x => new CalcuttaEventClassDto
                {
                    EventId = x.EventId,
                    EventClass = x.EventClass,
                    EventLabel = x.EventLabel,
                    EventType = x.EventType,
                    Sanction = x.Sanction,
                    StartDate = x.StartDate,
                    ClassUTCLockTime = x.ClassUTCLockTime,
                    Fees = x.Fees,
                    Id = x.Id,
                    ResultCount = resultEntries.Where(y => y.EventId == x.EventId).Count(),
                    IsCompleted = x.ClassUTCLockTime <= DateTime.UtcNow
                })
                .ToList();
            if (formDto.ClassList.Count > 0)
            {
                var lastClassUtclockTIme = formDto.ClassList.OrderByDescending(x => x.ClassUTCLockTime).ToList()[0].ClassUTCLockTime;
                if (lastClassUtclockTIme > DateTime.UtcNow)
                {
                    formDto.IsFinished = false;
                }
            }
            
            var passedClassList = formDto.ClassList.Where(x => x.IsCompleted).Select(x => x.EventId).ToList();

            var checkedEntries = _repoShoppingCart.Query()
                .Filter(x => x.ParentEventId == formDto.ParentEventId && !passedClassList.Contains(x.EventId))
                .Get().ToList();

            var entries = _repoCalcuttaEventEntry.Query()
                .Filter(x => x.ParentEventId == formDto.ParentEventId && !passedClassList.Contains(x.EventId))
                .Get()
                .Select(ent => new CalcuttaEventEntryLiteDto
                {
                    Id = ent.Id,
                    ParentEventId = ent.ParentEventId,
                    EventId = ent.EventId,
                    CalcuttaPrice = ent.CalcuttaPrice,
                    RegNo = ent.RegNo,
                    CompetitorId = ent.CompetitorId,
                    CompetitorName = ent.CompetitorName,
                    Owner = ent.Owner,
                    Handler = ent.Handler,
                    Del = ent.Del,
                    Draw = ent.Draw,
                    EntryId = ent.EntryId,
                    IsSold = checkedEntries.Where(x => x.EntryId == ent.EntryId && x.EventId == ent.EventId && x.IsSold == true && x.UserId == userId).Count() > 0 ? true : false, // owned buy
                    IsSolded = checkedEntries.Where(x => x.EntryId == ent.EntryId && x.EventId == ent.EventId && x.IsSold == true).Count() > 0 ? true : false, // for all buy
                    IsCheckOuted = userId == null ? false : checkedEntries.Where(x => x.EntryId == ent.EntryId && x.EventId == ent.EventId && x.UserId == userId).Count() > 0 ? true : false,
                    CheckOutUsers = checkedEntries.Where(x => x.EntryId == ent.EntryId && x.EventId == ent.EventId && x.UserId != userId).Count(),
                    TotalWon = JsonConvert.DeserializeObject<List<CalcuttaEventEntryStandingDto>>(ent.Standings).Select(x => x.Money).Sum()

                }).ToList();

            formDto.EntryList = entries;
            formDto.CheckOutedCount = userId == null ? 0 : checkedEntries.Where(x => x.UserId == userId && x.IsSold == false).Count();

            #endregion

            #region ResultList
            var groupedClassList = _repoShoppingCart.Query().Filter(x => x.ParentEventId == formDto.ParentEventId && x.IsSold == true).Get()
                    .GroupBy(p => p.EventId).Select(g => new
                    {
                        ClasssId = g.Key,
                        TotalMoney = g.Sum(y => y.CalcuttaPrice) * (decimal)(1 - _appSettings.CalcuttaDrag),
                        TotalSoldAnimals = g.Count()
                    }).ToList();
            var payoutBasicList = _repoPayoutBacic.Query().Get().ToList();

            var calculatedPayoutArray = new Dictionary<string, List<PayoutBasicCalDto>>();

            foreach (var clsTemp in groupedClassList)
            {
                int placeCount = clsTemp.TotalSoldAnimals / _appSettings.CalcuttaPlaceBreak;
                var placePayoutsBasic = payoutBasicList.Where(x => x.PlaceTTL == placeCount)
                    .Select(x => new PayoutBasicCalDto
                    {
                        Position = x.Position,
                        PayoutPrice = (int)Math.Round(x.PayPerc * clsTemp.TotalMoney, 0)
                    })
                    .ToList();
                for (var i = placeCount + 1; i <= clsTemp.TotalSoldAnimals; i++)
                {
                    placePayoutsBasic.Add(new PayoutBasicCalDto
                    {
                        Position = i,
                        PayoutPrice = 0
                    });
                }
                calculatedPayoutArray.Add(clsTemp.ClasssId, placePayoutsBasic);
            }

            var soldedEntries = (from res in _repoCalcuttaEventResult.Query().Filter(x => x.ParentEventId == formDto.ParentEventId).Get()
                                 join shopcart in _repoShoppingCart.Query().Filter(x => x.ParentEventId == formDto.ParentEventId && x.IsSold == true).Get() on new { res.EntryId, res.EventId } equals new { shopcart.EntryId, shopcart.EventId }
                                 join u in _repoUserDetail.Query().Get() on shopcart.UserId equals u.UserId
                                 select new
                                 {
                                     UserName = u.UserName,
                                     EntryId = shopcart.EntryId,
                                     EventId = shopcart.EventId,
                                     Place = res.Place,
                                     RealPlace = Int32.Parse(res.Place.Split("/")[0].Split("-")[0])

                                 }).ToList();
            List<EarngingDto> earnedEntries = new List<EarngingDto>();
            foreach (var x in soldedEntries)
            {
                var eqaulPlaceCount = soldedEntries.Where(y => y.Place == x.Place && y.EventId == x.EventId).Count();
                var prevPlaceCount = soldedEntries.Where(z => z.RealPlace < x.RealPlace && z.EventId == x.EventId).Count();
                var earnPlace = eqaulPlaceCount > 1 ? (prevPlaceCount + 1).ToString() + "/" + (eqaulPlaceCount + prevPlaceCount).ToString() : (prevPlaceCount + 1).ToString();

                earnedEntries.Add(new EarngingDto
                {
                    EntryId = x.EntryId,
                    EventId = x.EventId,
                    Place = x.Place,
                    RealPlace = x.RealPlace,
                    EarnPlace = earnPlace,
                    EarnRealPlace = Int32.Parse(earnPlace.Split("/")[0]),
                    EarningPrice = GetEarningPrice(calculatedPayoutArray[x.EventId], earnPlace)
                });
            }

            var entriesResult = (from res in _repoCalcuttaEventResult.Query().Filter(x => x.ParentEventId == formDto.ParentEventId).Get()
                           join ent in _repoCalcuttaEventEntry.Query().Filter(x => x.ParentEventId == formDto.ParentEventId).Get() on new { res.EntryId, res.EventId } equals new { ent.EntryId, ent.EventId }
                           select new CalcuttaEventResultLiteDto
                           {
                               Id = res.Id,
                               ParentEventId = res.ParentEventId,
                               EventId = res.EventId,
                               RegNo = res.RegNo,
                               CompetitorId = res.CompetitorId,
                               CompetitorName = res.CompetitorName,
                               Owner = res.Owner,
                               Del = res.Del,
                               EntryId = res.EntryId,
                               Score = res.Score,
                               Money = res.Money,
                               EventLinkId = res.EventLinkId,
                               Place = res.Place,
                               OutId = res.OutId,

                               RealPlace = Int32.Parse(res.Place.Split("/")[0].Split("-")[0]),
                               CalcuttaPrice = ent.CalcuttaPrice,
                               IsSolded = soldedEntries.Where(x => x.EntryId == res.EntryId && x.EventId == res.EventId).FirstOrDefault() == null ? 0 : 1,
                               UserName = soldedEntries.Where(x => x.EntryId == res.EntryId && x.EventId == res.EventId).FirstOrDefault() == null ? "" : soldedEntries.Where(x => x.EntryId == res.EntryId && x.EventId == res.EventId).FirstOrDefault().UserName,
                               EarnMoney = earnedEntries.Where(x => x.EntryId == res.EntryId && x.EventId == res.EventId).FirstOrDefault() == null ? 0 : earnedEntries.Where(x => x.EntryId == res.EntryId && x.EventId == res.EventId).FirstOrDefault().EarningPrice,
                               EarnPlace = earnedEntries.Where(x => x.EntryId == res.EntryId && x.EventId == res.EventId).FirstOrDefault() == null ? "" : earnedEntries.Where(x => x.EntryId == res.EntryId && x.EventId == res.EventId).FirstOrDefault().EarnPlace,
                               EarnRealPlace = earnedEntries.Where(x => x.EntryId == res.EntryId && x.EventId == res.EventId).FirstOrDefault() == null ? 0 : earnedEntries.Where(x => x.EntryId == res.EntryId && x.EventId == res.EventId).FirstOrDefault().EarnRealPlace,
                           }).ToList();
            formDto.ResultList = entriesResult;
            #endregion
            
            return await Task.FromResult(formDto);
        }
        public decimal GetEarningPrice(List<PayoutBasicCalDto> payouts, string place)
        {
            decimal earningPrice = 0;
            var placeArr = place.Split("/");
            var hiVal = Int32.Parse(placeArr[0]);
            if(hiVal > payouts.Count())
            {
                return earningPrice;
            }
            if (placeArr.Length == 2)
            {
                var loVal = Int32.Parse(placeArr[1]);
                var spiltCount = loVal - hiVal + 1;
                var splitSum = payouts.Where(x => x.Position >= hiVal && x.Position <= loVal).Select(y => y.PayoutPrice).Sum();
                earningPrice = Math.Round(splitSum / spiltCount, 2);
            }
            else
            {
                earningPrice = payouts.Where(x => x.Position == hiVal).Select(y => y.PayoutPrice).FirstOrDefault();
            }
            return earningPrice;
        }
        public async Task<bool> AddCart(AddCartDto cartDto, string userId)
        {
            var cartExist = _repoShoppingCart.Query().Filter(x => x.EntryId == cartDto.EntryId && x.EventId == cartDto.EventId && x.UserId == userId).Get().FirstOrDefault();
            if (cartExist == null)
            {
                var cart = new ShoppingCart
                {
                    EntId = cartDto.EntId,
                    ParentEventId = cartDto.ParentEventId,
                    EventId = cartDto.EventId,
                    EntryId = cartDto.EntryId,
                    UserId = userId,
                    IsSold = false,
                    OrderId = "",
                    PayerId = "",
                    CalcuttaPrice = cartDto.CalcuttaPrice
                };
                await _repoShoppingCart.InsertAsync(cart);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<CalcuttaCheckoutDetailDto> GetCheckoutDetail(int eventId, string userId)
        {
            var evDetail = _repoCalcuttaEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();
            var formDto = new CalcuttaCheckoutDetailDto();
            formDto.Id = eventId;
            if (evDetail != null)
            {
                var availableClassList = _repoCalcuttaEventClass.Query()
                .Filter(x => x.ParentEventId == evDetail.ParentEventId && x.ClassUTCLockTime > DateTime.UtcNow)
                .Get()
                .Select(x => x.EventId)
                .ToList();

                var checkedAllEntries = _repoShoppingCart.Query()
                    .Filter(x => x.ParentEventId == evDetail.ParentEventId && availableClassList.Contains(x.EventId)) 
                    .Get().ToList();
                var checkedUserEntries = (from shopcart in _repoShoppingCart.Query().Filter(x => x.ParentEventId == evDetail.ParentEventId && x.UserId == userId && availableClassList.Contains(x.EventId)).Get()
                                          join ent in _repoCalcuttaEventEntry.Query().Filter(x => x.ParentEventId == evDetail.ParentEventId).Get() on shopcart.EntId equals ent.Id
                                          join cls in _repoCalcuttaEventClass.Query().Filter(x => x.ParentEventId == evDetail.ParentEventId).Get() on ent.EventId equals cls.EventId
                                          select new CalcuttaEventEntryLiteDto
                                          {
                                              Id = ent.Id,
                                              ShopCartId = shopcart.Id,
                                              ParentEventId = shopcart.ParentEventId,
                                              EventId = shopcart.EventId,
                                              EntryId = shopcart.EntryId,
                                              RegNo = ent.RegNo,
                                              CompetitorId = ent.CompetitorId,
                                              CompetitorName = ent.CompetitorName,
                                              Owner = ent.Owner,
                                              Handler = ent.Handler,
                                              Del = ent.Del,
                                              Draw = ent.Draw,
                                              CalcuttaPrice = ent.CalcuttaPrice,
                                              IsSold = shopcart.IsSold, //owned buy
                                              IsSolded = checkedAllEntries.Where(x => x.EntId == ent.Id && x.IsSold == true ).Count() > 0 ? true : false,// for all buy
                                              CheckOutUsers = checkedAllEntries.Where(x => x.EntId == ent.Id && x.UserId != userId && x.IsSold == false).Count(),
                                              IsCheckOuted = true,
                                              EventClassName = cls.EventClass + " " + cls.EventType + " " + cls.EventLabel,
                                              TotalWon = JsonConvert.DeserializeObject<List<CalcuttaEventEntryStandingDto>>(ent.Standings).Select(x => x.Money).Sum()
                                          }).ToList();
                formDto.TotalPrice = checkedUserEntries.Where(x => x.IsSolded == false && x.IsSold == false).Select(x => x.CalcuttaPrice).Sum();
                formDto.TotalItems = checkedUserEntries.Where(x => x.IsSolded == false && x.IsSold == false).Count();
                formDto.EntryList = checkedUserEntries;
                formDto.ContestType = evDetail.ContestType;
                return await Task.FromResult(formDto);
            }
            else
            {
                formDto.EntryList = new List<CalcuttaEventEntryLiteDto>();
                formDto.TotalPrice = 0;
                formDto.TotalItems = 0;
                return await Task.FromResult(formDto);
            }
        }

        public async Task<bool> RemoveCart(int cartId, string userId)
        {
            await _repoShoppingCart.DeleteAsync(cartId);
            return true;
        }

        public async Task<string> CreatePayment(int eventId, string userId, bool isRiderComp)
        {
            if (isRiderComp)
            {
                var checkoutDetail = await GetRCCheckoutDetail(eventId, userId);

                if (checkoutDetail.TotalPrice > 0)
                {
                    var approvedUrl = "";
                    #region Paypal Checkout
                    var paypalClient = GetPayPalHttpClient();
                    OrderRequest orderRequest = new OrderRequest()
                    {
                        CheckoutPaymentIntent = "CAPTURE",

                        ApplicationContext = new ApplicationContext
                        {
                            BrandName = "Rank Ride",
                            LandingPage = "BILLING",
                            ReturnUrl = isRiderComp ? _appSettings.MainSiteURL + "event/ridercomp/success/" + eventId.ToString() : _appSettings.MainSiteURL + "event/bullcomp/success/" + eventId.ToString(),
                            CancelUrl = isRiderComp ? _appSettings.MainSiteURL + "event/ridercomp/cancel/" + eventId.ToString() : _appSettings.MainSiteURL + "event/bullcomp/cancel/" + eventId.ToString(),
                            //UserAction = "CONTINUE",
                            //ShippingPreference = "SET_PROVIDED_ADDRESS"
                        },
                        PurchaseUnits = new List<PurchaseUnitRequest>
                    {
                        new PurchaseUnitRequest
                        {
                            AmountWithBreakdown = new AmountWithBreakdown
                            {
                                CurrencyCode = "USD",
                                Value = checkoutDetail.TotalPrice.ToString("0.00")
                            }
                        }
                    }
                    };
                    var request = new OrdersCreateRequest();
                    request.Prefer("return=representation");
                    request.RequestBody(orderRequest);
                    var response = await paypalClient.Execute(request);
                    var result = response.Result<Order>();
                    if (result.Status == "CREATED")
                    {
                        foreach (var link in result.Links)
                        {
                            if (link.Rel == "approve")
                            {
                                approvedUrl = link.Href;
                            }
                        }
                        var orderId = result.Id;
                        //save order_id to databse
                        foreach (var cart in checkoutDetail.EntryList.Where(x => x.IsSold == false))
                        {
                            if (cart.IsSolded)
                            {
                                await _repoShoppingCart.DeleteAsync(cart.ShopCartId);
                            }
                            else
                            {
                                var cartExist = _repoShoppingCart.Query().Filter(x => x.Id == cart.ShopCartId).Get().FirstOrDefault();
                                if (cartExist != null)
                                {
                                    cartExist.OrderId = orderId;
                                    await _repoShoppingCart.UpdateAsync(cartExist);
                                }
                            }

                        }
                    }
                    #endregion
                    return approvedUrl;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                var checkoutDetail = await GetCheckoutDetail(eventId, userId);

                if (checkoutDetail.TotalPrice > 0)
                {
                    var approvedUrl = "";
                    #region Paypal Checkout
                    var paypalClient = GetPayPalHttpClient();
                    OrderRequest orderRequest = new OrderRequest()
                    {
                        CheckoutPaymentIntent = "CAPTURE",

                        ApplicationContext = new ApplicationContext
                        {
                            BrandName = "Rank Ride",
                            LandingPage = "BILLING",
                            ReturnUrl = isRiderComp ? _appSettings.MainSiteURL + "event/ridercomp/success/" + eventId.ToString() : _appSettings.MainSiteURL + "event/bullcomp/success/" + eventId.ToString(),
                            CancelUrl = isRiderComp ? _appSettings.MainSiteURL + "event/ridercomp/cancel/" + eventId.ToString() : _appSettings.MainSiteURL + "event/bullcomp/cancel/" + eventId.ToString(),
                            //UserAction = "CONTINUE",
                            //ShippingPreference = "SET_PROVIDED_ADDRESS"
                        },
                        PurchaseUnits = new List<PurchaseUnitRequest>
                    {
                        new PurchaseUnitRequest
                        {
                            AmountWithBreakdown = new AmountWithBreakdown
                            {
                                CurrencyCode = "USD",
                                Value = checkoutDetail.TotalPrice.ToString("0.00")
                            }
                        }
                    }
                    };
                    var request = new OrdersCreateRequest();
                    request.Prefer("return=representation");
                    request.RequestBody(orderRequest);
                    var response = await paypalClient.Execute(request);
                    var result = response.Result<Order>();
                    if (result.Status == "CREATED")
                    {
                        foreach (var link in result.Links)
                        {
                            if (link.Rel == "approve")
                            {
                                approvedUrl = link.Href;
                            }
                        }
                        var orderId = result.Id;
                        //save order_id to databse
                        foreach (var cart in checkoutDetail.EntryList.Where(x => x.IsSold == false))
                        {
                            if (cart.IsSolded)
                            {
                                await _repoShoppingCart.DeleteAsync(cart.ShopCartId);
                            }
                            else
                            {
                                var cartExist = _repoShoppingCart.Query().Filter(x => x.Id == cart.ShopCartId).Get().FirstOrDefault();
                                if (cartExist != null)
                                {
                                    cartExist.OrderId = orderId;
                                    await _repoShoppingCart.UpdateAsync(cartExist);
                                }
                            }

                        }
                    }
                    #endregion
                    return approvedUrl;
                }
                else
                {
                    return null;
                }

            }


        }

        public async Task<decimal> CapturePayment(int eventId, string token, string payerId, bool isRiderComp)
        {
            var paypalClient = GetPayPalHttpClient();
            var request = new OrdersCaptureRequest(token);
            request.Prefer("return=representation");
            request.RequestBody(new OrderActionRequest());
            var response = await paypalClient.Execute(request);
            var result = response.Result<Order>();
            if (result.Status == "COMPLETED")
            {
                var completedEntries = _repoShoppingCart.Query().Filter(x => x.OrderId == token).Get().ToList();
                string userId = "";
                
                foreach (var cart in completedEntries)
                {
                    userId = cart.UserId;
                    cart.IsSold = true;
                    cart.PayerId = result.Payer.PayerId;
                    await _repoShoppingCart.UpdateAsync(cart);
                }
                var amount = Convert.ToDecimal(result.PurchaseUnits[0].AmountWithBreakdown.Value);
                var transHistory = new TransactionHistory
                {
                    EventId = eventId,
                    EventMode = isRiderComp ? 1: 0,
                    UserId = userId,
                    PayerId = payerId,
                    OrderId = token,
                    PayerEmail = result.Payer.Email,
                    PayerGivenName = result.Payer.Name.GivenName == null ? "" : result.Payer.Name.GivenName,
                    PayerSureName = result.Payer.Name.Surname == null ? "" : result.Payer.Name.Surname,
                    Amount = amount,
                };
                await _repoTransactionHistory.InsertAsync(transHistory);
                return amount;
            }
            return 0;
        }

        public PayPalHttpClient GetPayPalHttpClient()
        {
            return new PayPalHttpClient(_appSettings.PayPalSandbox ? new SandboxEnvironment(_appSettings.PayPalClientID, _appSettings.PayPalClientSecret) : new LiveEnvironment(_appSettings.PayPalClientID, _appSettings.PayPalClientSecret));
        }

        public async Task<Tuple<List<CalcuttaHistoryLiteDto>, int>> GetHistoryOfCurrentUser(string userId, int start = 0, int length = 10)
        {
            var groupedClassList = _repoShoppingCart.Query().Filter(x => x.IsSold == true).Get()
                    .GroupBy(p => p.EventId).Select(g => new
                    {
                        ClasssId = g.Key,
                        TotalMoney = g.Sum(y => y.CalcuttaPrice) * (decimal)(1 - _appSettings.CalcuttaDrag),
                        TotalSoldAnimals = g.Count()
                    }).ToList();
            var payoutBasicList = _repoPayoutBacic.Query().Get().ToList();

            var calculatedPayoutArray = new Dictionary<string, List<PayoutBasicCalDto>>();

            foreach (var clsTemp in groupedClassList)
            {
                int placeCount = clsTemp.TotalSoldAnimals / _appSettings.CalcuttaPlaceBreak;
                var placePayoutsBasic = payoutBasicList.Where(x => x.PlaceTTL == placeCount)
                    .Select(x => new PayoutBasicCalDto
                    {
                        Position = x.Position,
                        PayoutPrice = (int)Math.Round(x.PayPerc * clsTemp.TotalMoney, 0)
                    })
                    .ToList();
                for (var i = placeCount + 1; i <= clsTemp.TotalSoldAnimals; i++)
                {
                    placePayoutsBasic.Add(new PayoutBasicCalDto
                    {
                        Position = i,
                        PayoutPrice = 0
                    });
                }
                calculatedPayoutArray.Add(clsTemp.ClasssId, placePayoutsBasic);
            }
            #region BullComp
            var soldedEntries = (from res in _repoCalcuttaEventResult.Query().Get()
                                 join shopcart in _repoShoppingCart.Query().Filter(x => x.IsSold == true).Get() on new { res.EntryId, res.EventId } equals new { shopcart.EntryId, shopcart.EventId }
                                 select new
                                 {
                                     EntryId = shopcart.EntryId,
                                     EventId = shopcart.EventId,
                                     Place = res.Place,
                                     RealPlace = Int32.Parse(res.Place.Split("/")[0])

                                 }).ToList();
            List<EarngingDto> earnedEntries = new List<EarngingDto>();
            foreach (var x in soldedEntries)
            {
                var eqaulPlaceCount = soldedEntries.Where(y => y.Place == x.Place && y.EventId == x.EventId).Count();
                var prevPlaceCount = soldedEntries.Where(z => z.RealPlace < x.RealPlace && z.EventId == x.EventId).Count();
                var earnPlace = eqaulPlaceCount > 1 ? (prevPlaceCount + 1).ToString() + "/" + (eqaulPlaceCount + prevPlaceCount).ToString() : (prevPlaceCount + 1).ToString();

                earnedEntries.Add(new EarngingDto
                {
                    EntryId = x.EntryId,
                    EventId = x.EventId,
                    Place = x.Place,
                    RealPlace = x.RealPlace,
                    EarnPlace = earnPlace,
                    EarnRealPlace = Int32.Parse(earnPlace.Split("/")[0]),
                    EarningPrice = GetEarningPrice(calculatedPayoutArray[x.EventId], earnPlace)
                });
            }

            var calHistory = (from shopcart in _repoShoppingCart.Query().Filter(x => x.UserId == userId && x.IsSold == true).Get()
                                 join evt in _repoCalcuttaEvent.Query().Get() on shopcart.ParentEventId equals evt.ParentEventId
                                 join cls in _repoCalcuttaEventClass.Query().Get() on new { shopcart.ParentEventId, shopcart.EventId } equals new { cls.ParentEventId, cls.EventId }
                                 join res in earnedEntries on new { shopcart.EntryId, shopcart.EventId } equals new { res.EntryId, res.EventId } into r
                                 from x in r.DefaultIfEmpty()
                                 select new CalcuttaHistoryLiteDto
                                 {
                                     Money = (x == null ? -20 : x.EarningPrice),
                                     Place = (x == null ?  "" : x.EarnPlace),
                                     RealPlace = (x == null ?  -20 : x.EarnRealPlace),
                                     EventClassName = cls.EventClass + " " + cls.EventType + " " + cls.EventLabel,
                                     EventName = evt.Title,
                                     ContestUTCLockTime = evt.ContestUTCLockTime,
                                     IsFinished = evt.ContestUTCLockTime < DateTime.UtcNow ? true : false,
                                     JoiningDateUTCString = evt.ContestUTCLockTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                                 }).ToList();
            #endregion

            #region RiderComp
            var soldedRCEntries = (from res in _repoCalcuttaRCResult.Query().Get()
                                 join shopcart in _repoShoppingCart.Query().Filter(x => x.IsSold == true).Get() on res.RowId equals shopcart.EntId
                                 join u in _repoUserDetail.Query().Get() on shopcart.UserId equals u.UserId
                                 select new
                                 {
                                     UserName = u.UserName,
                                     RowId = res.RowId,
                                     EventId = shopcart.EventId,
                                     Score = res.Score
                                 }).ToList();
            List<EarngingDto> earnedRCEntries = new List<EarngingDto>();
            foreach (var x in soldedRCEntries)
            {
                var eqaulPlaceCount = soldedRCEntries.Where(y => y.Score == x.Score && y.EventId == x.EventId).Count();
                var prevPlaceCount = soldedRCEntries.Where(z => z.Score > x.Score && z.EventId == x.EventId).Count();
                var earnPlace = eqaulPlaceCount > 1 ? (prevPlaceCount + 1).ToString() + "/" + (eqaulPlaceCount + prevPlaceCount).ToString() : (prevPlaceCount + 1).ToString();

                earnedRCEntries.Add(new EarngingDto
                {
                    RowId = x.RowId,
                    EventId = x.EventId,
                    EarnPlace = earnPlace,
                    EarnRealPlace = Int32.Parse(earnPlace.Split("/")[0]),
                    EarningPrice = GetEarningPrice(calculatedPayoutArray[x.EventId], earnPlace)
                });
            }

            var calRCHistory = (from shopcart in _repoShoppingCart.Query().Filter(x => x.UserId == userId && x.IsSold == true).Get()
                                 join evt in _repoCalcuttaRC.Query().Get() on shopcart.ParentEventId equals evt.ParentEventId
                                 join res in earnedRCEntries on shopcart.EntId equals res.RowId into r
                                 from x in r.DefaultIfEmpty()
                                 select new CalcuttaHistoryLiteDto
                                 {
                                     Money = (x == null ? -20 : x.EarningPrice),
                                     Place = (x == null ? "" : x.EarnPlace),
                                     RealPlace = (x == null ? -20 : x.EarnRealPlace),
                                     EventClassName = "",
                                     EventName = evt.Title,
                                     ContestUTCLockTime = evt.ContestUTCLockTime,
                                     IsFinished = evt.ContestUTCLockTime < DateTime.UtcNow ? true : false,
                                     JoiningDateUTCString = evt.ContestUTCLockTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                                 }).ToList();
            calHistory.AddRange(calRCHistory);
            #endregion

            var data = calHistory.OrderByDescending(x => x.ContestUTCLockTime).ThenBy(x => x.RealPlace).Skip(start).Take(length).ToList();
            int recordsTotal = calHistory.Count();

            return await Task.FromResult(new Tuple<List<CalcuttaHistoryLiteDto>, int>(data, recordsTotal));
        }

        public async Task<Tuple<List<CalcuttaHistoryLiteDto>, int>> GetAwardHistoryOfCurrentUser(string userId, int start = 0, int length = 10)
        {
            // get total collected money and solded animal count
            var groupedClassList = _repoShoppingCart.Query().Filter(x => x.IsSold == true).Get()
                    .GroupBy(p => p.EventId).Select(g => new
                    {
                        ClasssId = g.Key,
                        TotalMoney = g.Sum(y => y.CalcuttaPrice) * (decimal)(1 - _appSettings.CalcuttaDrag),
                        TotalSoldAnimals = g.Count()
                    }).ToList();
            var payoutBasicList = _repoPayoutBacic.Query().Get().ToList();

            var calculatedPayoutArray = new Dictionary<string, List<PayoutBasicCalDto>>();

            foreach (var clsTemp in groupedClassList)
            {
                int placeCount = clsTemp.TotalSoldAnimals / _appSettings.CalcuttaPlaceBreak;
                var placePayoutsBasic = payoutBasicList.Where(x => x.PlaceTTL == placeCount)
                    .Select(x => new PayoutBasicCalDto
                    {
                        Position = x.Position,
                        PayoutPrice = (int)Math.Round(x.PayPerc * clsTemp.TotalMoney, 0)
                    })
                    .ToList();
                for (var i = placeCount + 1; i <= clsTemp.TotalSoldAnimals; i++)
                {
                    placePayoutsBasic.Add(new PayoutBasicCalDto
                    {
                        Position = i,
                        PayoutPrice = 0
                    });
                }
                calculatedPayoutArray.Add(clsTemp.ClasssId, placePayoutsBasic);
            }
            var soldedEntries = (from res in _repoCalcuttaEventResult.Query().Get()
                                 join shopcart in _repoShoppingCart.Query().Filter(x => x.IsSold == true).Get() on new { res.EntryId, res.EventId } equals new { shopcart.EntryId, shopcart.EventId }
                                 select new
                                 {
                                     EntryId = shopcart.EntryId,
                                     EventId = shopcart.EventId,
                                     Place = res.Place,
                                     RealPlace = Int32.Parse(res.Place.Split("/")[0])

                                 }).ToList();
            List<EarngingDto> earnedEntries = new List<EarngingDto>();
            foreach (var x in soldedEntries)
            {
                var eqaulPlaceCount = soldedEntries.Where(y => y.Place == x.Place && y.EventId == x.EventId).Count();
                var prevPlaceCount = soldedEntries.Where(z => z.RealPlace < x.RealPlace && z.EventId == x.EventId).Count();
                var earnPlace = eqaulPlaceCount > 1 ? (prevPlaceCount + 1).ToString() + "/" + (eqaulPlaceCount + prevPlaceCount).ToString() : (prevPlaceCount + 1).ToString();

                earnedEntries.Add(new EarngingDto
                {
                    EntryId = x.EntryId,
                    EventId = x.EventId,
                    Place = x.Place,
                    RealPlace = x.RealPlace,
                    EarnPlace = earnPlace,
                    EarnRealPlace = Int32.Parse(earnPlace.Split("/")[0]),
                    EarningPrice = GetEarningPrice(calculatedPayoutArray[x.EventId], earnPlace)
                });
            }
            var calHistory = (from shopcart in _repoShoppingCart.Query().Filter(x => x.UserId == userId && x.IsSold == true).Get()
                              join evt in _repoCalcuttaEvent.Query().Get() on shopcart.ParentEventId equals evt.ParentEventId
                              join cls in _repoCalcuttaEventClass.Query().Get() on new { shopcart.ParentEventId, shopcart.EventId } equals new { cls.ParentEventId, cls.EventId }
                              join res in earnedEntries on new { shopcart.EntryId, shopcart.EventId } equals new { res.EntryId, res.EventId } into r
                              from x in r.DefaultIfEmpty()
                              select new CalcuttaHistoryLiteDto
                              {
                                  Money = (x == null ? -20 : x.EarningPrice),
                                  Place = (x == null ? "" : x.EarnPlace),
                                  RealPlace = (x == null ? -20 : x.EarnRealPlace),
                                  EventClassName = cls.EventClass + " " + cls.EventType + " " + cls.EventLabel,
                                  EventName = evt.Title,
                                  ContestUTCLockTime = evt.ContestUTCLockTime,
                                  IsFinished = evt.ContestUTCLockTime < DateTime.UtcNow ? true : false,
                                  JoiningDateUTCString = evt.ContestUTCLockTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                              }).ToList();
            var temp = calHistory.Where(x => x.Money > 0);
            var data = temp.OrderByDescending(x => x.ContestUTCLockTime).ThenBy(x => x.RealPlace).Skip(start).Take(length).ToList();
            int recordsTotal = temp.ToList().Count();

            return await Task.FromResult(new Tuple<List<CalcuttaHistoryLiteDto>, int>(data, recordsTotal));
        }

        public async Task<CalcuttRCDetailDto> GetRCEventDetail(int eventId, string userId)
        {
            CalcuttRCDetailDto formDto = new CalcuttRCDetailDto();
            formDto.IsFinished = false;
            formDto.Id = eventId;
            var eventDetail = _repoCalcuttaRC.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();
            if (eventDetail != null)
            {
                formDto.ParentEventId = eventDetail.ParentEventId;
                formDto.StartDate = eventDetail.StartDate;
                formDto.ContestUTCLockTime = eventDetail.ContestUTCLockTime;
                formDto.Title = eventDetail.Title;
                formDto.City = eventDetail.City;
                formDto.State = eventDetail.State;
                formDto.ContestType = eventDetail.ContestType;
                formDto.ContestStatus = eventDetail.ContestStatus;

                if (eventDetail.ContestUTCLockTime > DateTime.UtcNow)
                    formDto.IsFinished = false;

            }
            formDto.EntryList = new List<CalcuttaRCEntryLiteDto>();

            formDto.ResultList = new List<CalcuttaRCResultLiteDto>();
            if (formDto.IsFinished)
            {
                #region ResultList
                var groupedClassList = _repoShoppingCart.Query().Filter(x => x.ParentEventId == formDto.ParentEventId && x.EventId == formDto.ParentEventId && x.IsSold == true).Get()
                        .GroupBy(p => p.EventId).Select(g => new
                        {
                            ClasssId = g.Key,
                            TotalMoney = g.Sum(y => y.CalcuttaPrice) * (decimal)(1 - _appSettings.CalcuttaDrag),
                            TotalSoldAnimals = g.Count()
                        }).ToList();
                var payoutBasicList = _repoPayoutBacic.Query().Get().ToList();

                var calculatedPayoutArray = new Dictionary<string, List<PayoutBasicCalDto>>();

                foreach (var clsTemp in groupedClassList)
                {
                    int placeCount = clsTemp.TotalSoldAnimals / _appSettings.CalcuttaPlaceBreak;
                    var placePayoutsBasic = payoutBasicList.Where(x => x.PlaceTTL == placeCount)
                        .Select(x => new PayoutBasicCalDto
                        {
                            Position = x.Position,
                            PayoutPrice = (int)Math.Round(x.PayPerc * clsTemp.TotalMoney, 0)
                        })
                        .ToList();
                    for (var i = placeCount + 1; i <= clsTemp.TotalSoldAnimals; i++)
                    {
                        placePayoutsBasic.Add(new PayoutBasicCalDto
                        {
                            Position = i,
                            PayoutPrice = 0
                        });
                    }
                    calculatedPayoutArray.Add(clsTemp.ClasssId, placePayoutsBasic);
                }

                var soldedEntries = (from res in _repoCalcuttaRCResult.Query().Filter(x => x.ParentEventId == formDto.ParentEventId).Get()
                                     join shopcart in _repoShoppingCart.Query().Filter(x => x.ParentEventId == formDto.ParentEventId && x.EventId == formDto.ParentEventId && x.IsSold == true).Get() on res.RowId equals shopcart.EntId
                                     join u in _repoUserDetail.Query().Get() on shopcart.UserId equals u.UserId
                                     select new
                                     {
                                         UserName = u.UserName,
                                         RowId = res.RowId,
                                         EventId = shopcart.EventId,
                                         Score = res.Score
                                     }).ToList();
                List<EarngingDto> earnedEntries = new List<EarngingDto>();
                foreach (var x in soldedEntries)
                {
                    var eqaulPlaceCount = soldedEntries.Where(y => y.Score == x.Score && y.EventId == x.EventId).Count();
                    var prevPlaceCount = soldedEntries.Where(z => z.Score > x.Score && z.EventId == x.EventId).Count();
                    var earnPlace = eqaulPlaceCount > 1 ? (prevPlaceCount + 1).ToString() + "/" + (eqaulPlaceCount + prevPlaceCount).ToString() : (prevPlaceCount + 1).ToString();

                    earnedEntries.Add(new EarngingDto
                    {
                        RowId = x.RowId,
                        EventId = x.EventId,
                        EarnPlace = earnPlace,
                        EarnRealPlace = Int32.Parse(earnPlace.Split("/")[0]),
                        EarningPrice = GetEarningPrice(calculatedPayoutArray[x.EventId], earnPlace)
                    });
                }

                var entriesResult = (from res in _repoCalcuttaRCResult.Query().Filter(x => x.ParentEventId == formDto.ParentEventId).Get()
                                     join r in _repoRider.Query().Get() on Int32.Parse(res.CompetitorId) equals r.RiderId
                                     join ent in _repoCalcuttaRCEntry.Query().Filter(x => x.ParentEventId == formDto.ParentEventId).Get() on res.RowId equals ent.RowId
                                     select new CalcuttaRCResultLiteDto
                                     {
                                         Id = res.Id,
                                         ParentEventId = res.ParentEventId,
                                         CompetitorId = res.CompetitorId,
                                         CompetitorName = res.CompetitorName,
                                         Score = res.Score,

                                         RiderId = r.Id,
                                         RiderAvatar = _riderService.GetRiderPic(r.RiderId, _appSettings.MainSiteURL).Result,

                                         CalcuttaPrice = ent.CalcuttaPrice,
                                         IsSolded = soldedEntries.Where(x => x.RowId == res.RowId).FirstOrDefault() == null ? 0 : 1,
                                         UserName = soldedEntries.Where(x => x.RowId == res.RowId).FirstOrDefault() == null ? "" : soldedEntries.Where(x => x.RowId == res.RowId).FirstOrDefault().UserName,
                                         EarnMoney = earnedEntries.Where(x => x.RowId == res.RowId).FirstOrDefault() == null ? 0 : earnedEntries.Where(x => x.RowId == res.RowId).FirstOrDefault().EarningPrice,
                                         EarnPlace = earnedEntries.Where(x => x.RowId == res.RowId).FirstOrDefault() == null ? "" : earnedEntries.Where(x => x.RowId == res.RowId).FirstOrDefault().EarnPlace,
                                         EarnRealPlace = earnedEntries.Where(x => x.RowId == res.RowId).FirstOrDefault() == null ? 0 : earnedEntries.Where(x => x.RowId == res.RowId).FirstOrDefault().EarnRealPlace,
                                     }).ToList();
                formDto.ResultList = entriesResult;
                #endregion
            }
            else
            {
                #region EntryList
                
                var checkedEntries = _repoShoppingCart.Query()
                    .Filter(x => x.ParentEventId == formDto.ParentEventId && x.EventId == formDto.ParentEventId)
                    .Get().ToList();

                var entries = (from ent in _repoCalcuttaRCEntry.Query().Filter(x => x.ParentEventId == formDto.ParentEventId).Get()
                               join r in _repoRider.Query().Get() on Int32.Parse(ent.CompetitorId) equals r.RiderId
                               select new CalcuttaRCEntryLiteDto
                               {
                                   Id = ent.Id,
                                   ParentEventId = ent.ParentEventId,
                                   CalcuttaPrice = ent.CalcuttaPrice,
                                   CompetitorId = ent.CompetitorId,
                                   CompetitorName = ent.CompetitorName,
                                   RowId = ent.RowId,
                                   RiderPower = ent.RiderPower,
                                   RiderPowerAvg = ent.RiderPowerAvg,
                                   RiderPowerCurrent = ent.RiderPowerCurrent,
                                   IsSold = checkedEntries.Where(x => x.EntId == ent.RowId && x.EventId == formDto.ParentEventId && x.IsSold == true && x.UserId == userId).Count() > 0 ? true : false, // owned buy
                                   IsSolded = checkedEntries.Where(x => x.EntId == ent.RowId && x.EventId == formDto.ParentEventId && x.IsSold == true).Count() > 0 ? true : false, // for all buy
                                   IsCheckOuted = userId == null ? false : checkedEntries.Where(x => x.EntId == ent.RowId && x.EventId == formDto.ParentEventId && x.UserId == userId).Count() > 0 ? true : false,
                                   CheckOutUsers = checkedEntries.Where(x => x.EntId == ent.RowId && x.EventId == formDto.ParentEventId && x.UserId != userId).Count(),

                                   RiderId = r.Id,
                                   RiderAvatar = _riderService.GetRiderPic(r.RiderId, _appSettings.MainSiteURL).Result,
                               }).ToList();

                formDto.EntryList = entries;
                formDto.CheckOutedCount = userId == null ? 0 : checkedEntries.Where(x => x.UserId == userId && x.IsSold == false).Count();

                #endregion
            }




            return await Task.FromResult(formDto);
        }

        public async Task<CalcuttaCheckoutRCDetailDto> GetRCCheckoutDetail(int eventId, string userId)
        {
            var evDetail = _repoCalcuttaRC.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();
            var formDto = new CalcuttaCheckoutRCDetailDto();
            formDto.Id = eventId;
            if (evDetail != null)
            {
               
                var checkedAllEntries = _repoShoppingCart.Query()
                    .Filter(x => x.ParentEventId == evDetail.ParentEventId && x.EventId == evDetail.ParentEventId)
                    .Get().ToList();
                var checkedUserEntries = (from shopcart in _repoShoppingCart.Query().Filter(x => x.ParentEventId == evDetail.ParentEventId && x.UserId == userId && x.EventId == evDetail.ParentEventId).Get()
                                          join ent in _repoCalcuttaRCEntry.Query().Filter(x => x.ParentEventId == evDetail.ParentEventId).Get() on shopcart.EntId equals ent.Id
                                          join r in _repoRider.Query().Get() on Int32.Parse(ent.CompetitorId) equals r.RiderId
                                          select new CalcuttaRCEntryLiteDto
                                          {
                                              Id = ent.Id,
                                              ShopCartId = shopcart.Id,
                                              ParentEventId = shopcart.ParentEventId,
                                              RowId = ent.RowId,
                                              RiderPower = ent.RiderPower,
                                              RiderPowerAvg = ent.RiderPowerAvg,
                                              RiderPowerCurrent = ent.RiderPowerCurrent,
                                              CompetitorId = ent.CompetitorId,
                                              CompetitorName = ent.CompetitorName,
                                              CalcuttaPrice = ent.CalcuttaPrice,
                                              IsSold = shopcart.IsSold, //owned buy
                                              IsSolded = checkedAllEntries.Where(x => x.EntId == ent.RowId && x.IsSold == true).Count() > 0 ? true : false,// for all buy
                                              CheckOutUsers = checkedAllEntries.Where(x => x.EntId == ent.RowId && x.UserId != userId && x.IsSold == false).Count(),
                                              IsCheckOuted = true,

                                              RiderId = r.Id,
                                              RiderAvatar = _riderService.GetRiderPic(r.RiderId, _appSettings.MainSiteURL).Result,
                                          }).ToList();
                formDto.TotalPrice = checkedUserEntries.Where(x => x.IsSolded == false && x.IsSold == false).Select(x => x.CalcuttaPrice).Sum();
                formDto.TotalItems = checkedUserEntries.Where(x => x.IsSolded == false && x.IsSold == false).Count();
                formDto.EntryList = checkedUserEntries;
                formDto.ContestType = evDetail.ContestType;
                return await Task.FromResult(formDto);
            }
            else
            {
                formDto.EntryList = new List<CalcuttaRCEntryLiteDto>();
                formDto.TotalPrice = 0;
                formDto.TotalItems = 0;
                return await Task.FromResult(formDto);
            }
        }

        public async Task<PickTeamDetailDto> GetSimplePickTeamDetail(int eventId, string userId)
        {
            PickTeamDetailDto formDto = new PickTeamDetailDto();
            formDto.IsFinished = false; //for presendation
            formDto.Id = eventId;
            var eventDetail = _repoCalcuttaEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();
            int teamId = 0;
            if (eventDetail != null)
            {
                formDto.ParentEventId = eventDetail.ParentEventId;
                formDto.StartDate = eventDetail.StartDate;
                formDto.ContestUTCLockTime = eventDetail.ContestUTCLockTime;
                formDto.Title = eventDetail.Title;
                formDto.City = eventDetail.City;
                formDto.State = eventDetail.State;
                formDto.ContestType = eventDetail.ContestType;
                formDto.ContestStatus = eventDetail.ContestStatus;

                /*if (eventDetail.ContestUTCLockTime > DateTime.UtcNow)
                    formDto.IsFinished = false;
                */
                if (userId != null)
                {
                    var teamDetail = _repoSimpleTeam.Query().Filter(x => x.EventId == eventId && x.UserId == userId).Get().FirstOrDefault();
                    if(teamDetail != null)
                    {
                        teamId = teamDetail.Id;
                    }
                }
            }
            formDto.TeamId = teamId;
            formDto.EntryList = new List<PickEntryLiteDto>();

            #region EntryList
            
            var lastClassUtclockTIme = _repoCalcuttaEventClass.Query()
                .Filter(x => x.ParentEventId == formDto.ParentEventId && x.EventType.ToLower() != "all in")
                .OrderBy(x => x.OrderByDescending(y => y.ClassUTCLockTime))
                .Get()
                .Select(x => x.ClassUTCLockTime).FirstOrDefault();
            if (lastClassUtclockTIme > DateTime.UtcNow)
            {
                formDto.IsFinished = false;
            }
            var teamBullList = _repoSimpleTeamBull.Query().Filter(x => x.SimpleTeamId == teamId).Get().ToList();
            var entries = _repoCalcuttaEventEntry.Query()
                .Filter(x => x.ParentEventId == formDto.ParentEventId)
                .Get()
                .GroupBy(m => new { m.CompetitorId, m.CompetitorName })
                .Select(ent => new PickEntryLiteDto
                {
                    CompetitorId = ent.First().CompetitorId,
                    CompetitorName = ent.First().CompetitorName,
                    Owner = ent.First().Owner,
                    IsSelected = teamBullList.Where(x => x.CompetitorId == ent.First().CompetitorId).Count() > 0 ? true : false,
                    TotalWon = JsonConvert.DeserializeObject<List<CalcuttaEventEntryStandingDto>>(ent.First().Standings).Select(x => x.Money).Sum()

                }).ToList();

            formDto.EntryList = entries;
            #endregion
            
            return await Task.FromResult(formDto);
        }

        public async Task<CalcuttaEvent> GetEventById(int eventId)
        {
            var eventDetail = _repoCalcuttaEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();

            return await Task.FromResult(eventDetail);
        }

        public async Task<int> CreateTeam(IEnumerable<SimpleTeamDto> teamDto, int eventId, string userId)
        {
                var teamModel = new SimpleTeam();
                teamModel = new SimpleTeam
                {
                    UserId = userId,
                    CreatedDate = DateTime.Now,
                    EventId = eventId,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.Now
                };
                foreach (var team in teamDto)
                {
                    teamModel.SimpleTeamBull.Add(new SimpleTeamBull
                    {
                        CompetitorId = team.CompetitorId
                    });
                }
                await _repoSimpleTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            
        }
        public async Task DeleteTeam(int TeamId)
        {
            var getTeamBull = _repoSimpleTeamBull.Query()
                 .Filter(x => x.SimpleTeamId == TeamId).Get().ToList();
            
            
            await _repoSimpleTeamBull.DeleteCollection(getTeamBull);
            await _repoSimpleTeam.DeleteAsync(TeamId);
        }

        public async Task<Tuple<List<JoinUserContestLiteDto>, int>> JoinedUserContestAjax(int eventId, int start = 0, int length = 10)
        {
            List<JoinUserContestLiteDto> joinUserContests = new List<JoinUserContestLiteDto>();
            int totalRecords = 0;

            try
            {
                
                var result = (from jc in _repoSimpleTeam.Query().Filter(x => x.EventId == eventId).Get()
                              join u in _repoUsers.Query().Get() on jc.UserId equals u.UserId
                              select new
                              {
                                  jc.SimpleTeamPoint,
                                  jc.UserId,
                                  jc.Id,
                                  u.UserName,
                                  u.TeamName,
                                  u.Email,
                                  u.Avtar
                              }).Distinct().ToList();

                //Calculate rank for joined team for contest.
                var rankResult = (from a in result.OrderByDescending(x => x.SimpleTeamPoint)
                                  select new
                                  {
                                      Rank = result.Count(x => x.SimpleTeamPoint > a.SimpleTeamPoint) + 1,
                                      a.UserId,
                                      a.Id,
                                      a.SimpleTeamPoint,
                                      a.UserName,
                                      a.TeamName,
                                      a.Email,
                                      a.Avtar
                                  }).ToList();

                totalRecords = rankResult.Count;
                var tmpResult = rankResult.Skip(start).Take(length);
                //get Number of contest 
                var allJonedContests = _repoSimpleTeam.Query().Get().ToList();

                foreach (var item in tmpResult)
                {
                    JoinUserContestLiteDto dto = new JoinUserContestLiteDto();
                    dto.Email = item.Email;
                    dto.UserName = item.UserName;
                    dto.TeamName = item.TeamName;
                    dto.TeamPoint = item.SimpleTeamPoint;
                    dto.TeamRank = item.Rank;
                    dto.UserId = item.UserId;
                    dto.TeamId = item.Id;
                    dto.Avatar = !string.IsNullOrEmpty(item.Avtar) ? (item.Avtar.Contains("https://") ? item.Avtar : (item.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + item.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png";
                    dto.CanUpdateTeam = false; //CanEditTeam;
                    dto.NumberOfContest = allJonedContests.Where(x => x.UserId == item.UserId).Count();
                    joinUserContests.Add(dto);
                }


            }
            catch (Exception ex)
            {
            }
            return await Task.FromResult(new Tuple<List<JoinUserContestLiteDto>, int>(joinUserContests.ToList(), totalRecords));
        }

        public async Task<Tuple<IEnumerable<PickEntryLiteDto>, int, int, decimal, DateTime>> GetTeamDetailOfCurrentUser(int eventId, int teamId)
        {
            var joinedTeam = _repoSimpleTeam.Query().Filter(x => x.EventId == eventId).Get().ToList();
            var numberOfEntries = joinedTeam.Count();
            var teamPoint = joinedTeam.Where(x => x.Id == teamId).Select(x => x.SimpleTeamPoint).SingleOrDefault();
            var position = joinedTeam.Count(x => x.SimpleTeamPoint > teamPoint) + 1;
            var joinedDate = joinedTeam.Where(x => x.Id == teamId).Select(x => x.CreatedDate).SingleOrDefault();

            var eventDetail = _repoCalcuttaEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();
            var tmpTeamBull = (from a in _repoSimpleTeamBull.Query().Filter(x => x.SimpleTeamId == teamId).Get()
                                 join b in _repoCalcuttaEventEntry.Query().Filter(x => x.ParentEventId == eventDetail.ParentEventId).Get()
                                 on a.CompetitorId equals b.CompetitorId
                                  select new PickEntryLiteDto {
                                      CompetitorId = a.CompetitorId,
                                      CompetitorName = b.CompetitorName,
                                      CompetitorPoint = a.CompetitorPoint
                                  }).ToList();
            var joinedTeamBull = tmpTeamBull.GroupBy(x => x.CompetitorId).Select(y => y.First()).ToList();
            return await Task.FromResult(new Tuple<IEnumerable<PickEntryLiteDto>, int, int, decimal, DateTime>(joinedTeamBull, numberOfEntries, position, teamPoint, joinedDate));
        }

        public async Task<Tuple<List<JoinUserContestLiteDto>, int>> JoinedCurrentUserContestAjax(string userId, int start = 0, int length = 10)
        {
            List<JoinUserContestLiteDto> joinUserContests = new List<JoinUserContestLiteDto>();
            int totalRecords = 0;

            try
            {

                var result = (from jc in _repoSimpleTeam.Query().Get()
                              join u in _repoUsers.Query().Get() on jc.UserId equals u.UserId
                              where u.UserId == userId
                              select new
                              {
                                  jc.SimpleTeamPoint,
                                  jc.UserId,
                                  jc.Id,
                                  jc.EventId,
                                  u.UserName,
                                  u.TeamName,
                                  u.Email,
                                  u.Avtar
                              }).Distinct().ToList();
                var tmpResult = result.Skip(start).Take(length);
                totalRecords = result.Count;


                //get Number of contest 
                var allJonedContests = _repoSimpleTeam.Query().Get().ToList();

                foreach (var item in tmpResult)
                {
                    JoinUserContestLiteDto dto = new JoinUserContestLiteDto();
                    dto.Email = item.Email;
                    dto.UserName = item.UserName;
                    dto.TeamName = item.TeamName;
                    dto.TeamPoint = item.SimpleTeamPoint;
                    dto.TeamRank = GetTeamRankByUserId(item.EventId, item.SimpleTeamPoint).Result;
                    dto.UserId = item.UserId;
                    dto.TeamId = item.Id;
                    dto.ContestId = item.EventId;

                    dto.Avatar = !string.IsNullOrEmpty(item.Avtar) ? (item.Avtar.Contains("https://") ? item.Avtar : (item.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + item.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png";
                    dto.CanUpdateTeam = false; //CanEditTeam;
                    dto.NumberOfContest = allJonedContests.Where(x => x.UserId == item.UserId).Count();
                    joinUserContests.Add(dto);
                }


            }
            catch (Exception ex)
            {
            }
            return await Task.FromResult(new Tuple<List<JoinUserContestLiteDto>, int>(joinUserContests.ToList(), totalRecords));
        }

        public async Task<int> GetTeamRankByUserId(int eventId, decimal teamPoint)
        {

            var entries = _repoSimpleTeam.Query().Filter(x => x.EventId == eventId).Get().ToList();

            var position = entries.Count(x => x.SimpleTeamPoint > teamPoint) + 1;
            return await Task.FromResult(position);
        }
    }
}
