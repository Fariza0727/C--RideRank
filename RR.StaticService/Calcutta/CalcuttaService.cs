using Newtonsoft.Json;
using RR.Dto.Calcutta;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.StaticService.Calcutta
{
    public class CalcuttaService : ICalcuttaService
    {
        #region Constructor

        private readonly IRepository<CalcuttaEvent, RankRideStaticContext> _repoCalcuttaEvent;
        private readonly IRepository<CalcuttaEventClass, RankRideStaticContext> _repoCalcuttaEventClass;
        private readonly IRepository<CalcuttaEventEntry, RankRideStaticContext> _repoCalcuttaEventEntry;
        private readonly IRepository<CalcuttaEventResult, RankRideStaticContext> _repoCalcuttaEventResult;
        private readonly IRepository<PayoutBasic, RankRideStaticContext> _repoPayoutBasic;

        public CalcuttaService(
            IRepository<CalcuttaEvent, RankRideStaticContext> repoCalcuttaEvent,
            IRepository<CalcuttaEventClass, RankRideStaticContext> repoCalcuttaEventClass,
            IRepository<CalcuttaEventEntry, RankRideStaticContext> repoCalcuttaEventEntry,
            IRepository<CalcuttaEventResult, RankRideStaticContext> repoCalcuttaEventResult,
            IRepository<PayoutBasic, RankRideStaticContext> repoPayoutBasic
            )
        {
            _repoCalcuttaEvent = repoCalcuttaEvent;
            _repoCalcuttaEventClass = repoCalcuttaEventClass;
            _repoCalcuttaEventEntry = repoCalcuttaEventEntry;
            _repoCalcuttaEventResult = repoCalcuttaEventResult;
            _repoPayoutBasic = repoPayoutBasic;
        }


        #endregion

        public void Dispose()
        {
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
            if (_repoPayoutBasic != null)
            {
                _repoPayoutBasic.Dispose();
            }
        }

        public async Task ManageEventClasses(CalcuttaEventClassesDto eventClassesDto)
        {
            foreach (var item in eventClassesDto.CalcuttaEventClassList)
            {
                try
                {
                    var classExists = _repoCalcuttaEventClass.Query()
                    .Filter(x => x.ParentEventId == eventClassesDto.ParentEventId && x.EventId == item.EventId)
                    .Get().FirstOrDefault();
                    if (classExists != null)
                    {
                        //update
                        classExists.ParentEventId = eventClassesDto.ParentEventId;
                        classExists.EventId = item.EventId;
                        classExists.StartDate = item.StartDate;
                        classExists.ClassUTCLockTime = item.ClassUTCLockTime;
                        classExists.EventClass = item.EventClass;
                        classExists.EventType = item.EventType;
                        classExists.EventLabel = item.EventLabel;
                        classExists.Sanction = item.Sanction;
                        classExists.Fees = item.Fees;
                        classExists.UpdatedDate = DateTime.Now;
                        await _repoCalcuttaEventClass.UpdateAsync(classExists);
                    }
                    else
                    {
                        //insert
                        classExists = new CalcuttaEventClass();
                        classExists.ParentEventId = eventClassesDto.ParentEventId;
                        classExists.EventId = item.EventId;
                        classExists.StartDate = item.StartDate;
                        classExists.ClassUTCLockTime = item.ClassUTCLockTime;
                        classExists.EventClass = item.EventClass;
                        classExists.EventType = item.EventType;
                        classExists.EventLabel = item.EventLabel;
                        classExists.Sanction = item.Sanction;
                        classExists.Fees = item.Fees;
                        classExists.CreatedDate = DateTime.Now;
                        classExists.UpdatedDate = DateTime.Now;
                        await _repoCalcuttaEventClass.InsertAsync(classExists);
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task ManageEventEntries(CalcuttaEventEntriesDto eventEntriesDto)
        {
            foreach (var item in eventEntriesDto.CalcuttaEventEntryList)
            {
                try
                {
                    var entryExists = _repoCalcuttaEventEntry.Query()
                    .Filter(x => x.ParentEventId == eventEntriesDto.ParentEventId && x.EventId == item.EventId && x.EntryId == item.EntryId)
                    .Get().FirstOrDefault();
                    if (entryExists != null)
                    {
                        //update
                        entryExists.RegNo = item.RegNo;
                        entryExists.CompetitorId = item.CompetitorId;
                        entryExists.CompetitorName = item.CompetitorName;
                        entryExists.Owner = item.Owner;
                        entryExists.Handler = item.Handler;
                        entryExists.Del = item.Del;
                        entryExists.Draw = item.Draw;
                        entryExists.CalcuttaPrice = item.CalcuttaPrice;
                        entryExists.Standings = JsonConvert.SerializeObject(item.Standings);
                        await _repoCalcuttaEventEntry.UpdateAsync(entryExists);
                    }
                    else
                    {
                        //insert
                        entryExists = new CalcuttaEventEntry();
                        entryExists.ParentEventId = eventEntriesDto.ParentEventId;
                        entryExists.EventId = item.EventId;
                        entryExists.EntryId = item.EntryId;
                        entryExists.RegNo = item.RegNo;
                        entryExists.CompetitorId = item.CompetitorId;
                        entryExists.CompetitorName = item.CompetitorName;
                        entryExists.Owner = item.Owner;
                        entryExists.Handler = item.Handler;
                        entryExists.Del = item.Del;
                        entryExists.Draw = item.Draw;
                        entryExists.CalcuttaPrice = item.CalcuttaPrice;
                        entryExists.Standings = JsonConvert.SerializeObject(item.Standings);
                        await _repoCalcuttaEventEntry.InsertAsync(entryExists);
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task ManageEventResults(CalcuttaEventResultsDto eventResultsDto)
        {
            foreach (var item in eventResultsDto.CalcuttaEventResultList)
            {
                try
                {
                    var resultExists = _repoCalcuttaEventResult.Query()
                    .Filter(x => x.ParentEventId == eventResultsDto.ParentEventId && x.EventId == item.EventId && x.EntryId == item.EntryId && x.OutId == item.OutId)
                    .Get().FirstOrDefault();
                    if (resultExists != null)
                    {
                        //update
                        resultExists.ParentEventId = eventResultsDto.ParentEventId;
                        resultExists.EventId = item.EventId;
                        resultExists.OutId = item.OutId;
                        resultExists.EntryId = item.EntryId;
                        resultExists.RegNo = item.RegNo;
                        resultExists.CompetitorId = item.CompetitorId;
                        resultExists.CompetitorName = item.CompetitorName;
                        resultExists.Owner = item.Owner;
                        resultExists.Score = item.Score;
                        resultExists.Del = item.Del;
                        resultExists.Place = item.Place;
                        resultExists.Money = item.Money;
                        resultExists.EventLinkId = item.EventLinkId;
                        await _repoCalcuttaEventResult.UpdateAsync(resultExists);
                    }
                    else
                    {
                        //insert
                        resultExists = new CalcuttaEventResult();
                        resultExists.ParentEventId = eventResultsDto.ParentEventId;
                        resultExists.EventId = item.EventId;
                        resultExists.OutId = item.OutId;
                        resultExists.EntryId = item.EntryId;
                        resultExists.RegNo = item.RegNo;
                        resultExists.CompetitorId = item.CompetitorId;
                        resultExists.CompetitorName = item.CompetitorName;
                        resultExists.Owner = item.Owner;
                        resultExists.Score = item.Score;
                        resultExists.Del = item.Del;
                        resultExists.Place = item.Place;
                        resultExists.Money = item.Money;
                        resultExists.EventLinkId = item.EventLinkId;
                        await _repoCalcuttaEventResult.InsertAsync(resultExists);
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task ManageEvents(List<CalcuttaEventDto> eventDto)
        {
            int count = 0;
            foreach (var item in eventDto.Where(x => x.StartDate.Date > DateTime.Now.Date))
            {
                try
                {
                    var eventExists = _repoCalcuttaEvent.Query()
                    .Filter(x => x.ParentEventId == item.ParentEventId)
                    .Get().FirstOrDefault();

                    if (eventExists != null)
                    {
                        //update
                        eventExists.ParentEventId = item.ParentEventId;
                        eventExists.Title = !string.IsNullOrEmpty(item.Title) ? item.Title : "";
                        eventExists.City = item.City;
                        eventExists.State = item.State;
                        eventExists.ContestType = !string.IsNullOrEmpty(item.ContestType) ? item.ContestType : "";
                        eventExists.StartDate = item.StartDate;
                        eventExists.ContestUTCLockTime = item.ContestUTCLockTime;
                        eventExists.ContestStatus = !string.IsNullOrEmpty(item.ContestStatus) ? item.ContestStatus : "";
                        eventExists.UpdatedDate = DateTime.Now;
                        await _repoCalcuttaEvent.UpdateAsync(eventExists);
                    }
                    else
                    {
                        //insert
                        eventExists = new CalcuttaEvent();
                        eventExists.ParentEventId = item.ParentEventId;
                        eventExists.Title = !string.IsNullOrEmpty(item.Title) ? item.Title : "";
                        eventExists.City = item.City;
                        eventExists.State = item.State;
                        eventExists.ContestType = !string.IsNullOrEmpty(item.ContestType) ? item.ContestType : "";
                        eventExists.StartDate = item.StartDate;
                        eventExists.ContestUTCLockTime = item.ContestUTCLockTime;
                        eventExists.ContestStatus = !string.IsNullOrEmpty(item.ContestStatus) ? item.ContestStatus : "";
                        eventExists.UpdatedDate = DateTime.Now;
                        eventExists.CreatedDate = DateTime.Now;
                        await _repoCalcuttaEvent.InsertAsync(eventExists);
                        count = count + 1;
                    }
                }
                catch (Exception ex)
                {
                }
            }

        }


        public async Task ManagePayoutBasic(CalcuttaPayoutsDto paysDto)
        {
            int count = 0;
            foreach (var item in paysDto.CalcuttaPayoutList.Where(x => x.Sanction == "ABBI"))
            {
                try
                {
                    var itemExist = _repoPayoutBasic.Query()
                    .Filter(x => x.RowId == item.RowId)
                    .Get().FirstOrDefault();

                    if (itemExist != null)
                    {
                        //update
                        itemExist.Sanction = item.Sanction;
                        itemExist.Position = item.Position;
                        itemExist.PlaceTTL = item.PlaceTTL;
                        itemExist.PayPerc = item.PayPerc;
                        itemExist.UpdatedDate = DateTime.Now;
                        await _repoPayoutBasic.UpdateAsync(itemExist);
                    }
                    else
                    {
                        //insert
                        itemExist = new PayoutBasic();
                        itemExist.RowId = item.RowId;
                        itemExist.Sanction = item.Sanction;
                        itemExist.Position = item.Position;
                        itemExist.PlaceTTL = item.PlaceTTL;
                        itemExist.PayPerc = item.PayPerc;
                        itemExist.UpdatedDate = DateTime.Now;
                        itemExist.CreatedDate = DateTime.Now;
                        await _repoPayoutBasic.InsertAsync(itemExist);
                        count = count + 1;
                    }
                }
                catch (Exception ex)
                {
                }
            }

        }

    }
}
