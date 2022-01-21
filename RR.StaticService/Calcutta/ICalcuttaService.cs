using RR.Dto.Calcutta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.StaticService.Calcutta
{
    public interface ICalcuttaService : IDisposable
    {

        Task ManageEvents(List<CalcuttaEventDto> eventDto);
        Task ManageEventClasses(CalcuttaEventClassesDto eventClassesDto);
        Task ManageEventEntries(CalcuttaEventEntriesDto eventEntriesDto);
        Task ManageEventResults(CalcuttaEventResultsDto eventResultsDto);
        Task ManagePayoutBasic(CalcuttaPayoutsDto paysDto);
    }
}
