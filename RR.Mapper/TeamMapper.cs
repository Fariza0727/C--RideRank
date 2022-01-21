using RR.Dto;
using RR.Dto.Team;
using RR.StaticData;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
     public static class TeamMapper
     {
          public static IEnumerable<EventDrawLiteDto> Map(IEnumerable<EventDraw> eventPlayers)
          {
               return eventPlayers.Select(p => Map(p));
          }
          public static EventDrawLiteDto Map(EventDraw eventPlayer)
          {
               return new EventDrawLiteDto
               {
                    BullId = eventPlayer.BullId,
                    BullName = eventPlayer.BullName,
                    RiderId = eventPlayer.RiderId,
                    RiderName = eventPlayer.RiderName,
                    Round = eventPlayer.Round,
                    EventId = eventPlayer.EventId,
                    BullTier = eventPlayer.Event.EventBull
                              .Where(x => x.Bull.BullId == eventPlayer.BullId)
                              .Select(t => t.EventTier)
                              .FirstOrDefault(),
                    RiderTier = eventPlayer.Event.EventRider
                              .Where(x => x.Rider.RiderId == eventPlayer.RiderId)
                              .Select(t => t.EventTier)
                              .FirstOrDefault(),
               };
          }

          public static IEnumerable<TeamBullFormationDto> Map(IEnumerable<EventBull> eventBulls)
          {
               return eventBulls.Select(p => Map(p));
          }
          public static TeamBullFormationDto Map(EventBull eventBull)
          {
               return new TeamBullFormationDto
               {
                    BullId = eventBull.BullId,
                    BullName = eventBull.Bull.Name,
                    BullTier = eventBull.EventTier
               };
          }

          public static IEnumerable<TeamRiderFormationDto> Map(IEnumerable<EventRider> eventRiders)
          {
               return eventRiders.Select(p => Map(p));
          }
          public static TeamRiderFormationDto Map(EventRider eventRider)
          {
               return new TeamRiderFormationDto
               {
                    RiderId = eventRider.RiderId,
                    RiderName = eventRider.Rider.Name,
                    RiderTier = eventRider.EventTier
               };
          }
     }
}
