
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RR.Core;
using RR.Dto;
using RR.Dto.Event;
using RR.Dto.Team;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
    public class MyRedisConnectorHelper
    {
        private Lazy<StackExchange.Redis.ConnectionMultiplexer> _redisConnection;
        private IDatabase _cache;
        private string _cacheEnvKey;
        private HttpContext _currentContext;
        private bool _redisConnected = false;
        public MyRedisConnectorHelper(AppSettings appSettings)
        {
            _cacheEnvKey = appSettings.CacheEnvKey;
            try
            {
                _redisConnection = new Lazy
                   <StackExchange.Redis.ConnectionMultiplexer>(() =>
                   {
                       return StackExchange.Redis.ConnectionMultiplexer.Connect(appSettings.CacheConnection);
                   }
                );
                _cache = _redisConnection.Value.GetDatabase();
                _redisConnected = true;
            }catch(Exception e)
            {
                _redisConnected = false;

            }
            
        }
        public bool RedisConnected()
        {
            return _redisConnected;
        }
        public bool EqualRedisJoinedContestCount(int count)
        {
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-JoinedContestCount");
            if (value.HasValue)
            {
                return int.Parse(value.ToString()) == count;
            }
            else
            {
                return false;
            }
        }
        public void SaveRedisJoinedContestCount(int count)
        {
            _cache.StringSet($"{_cacheEnvKey}-JoinedContestCount", count.ToString());
        }

        public bool EqualRedisEventId(int eventID)
        {
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-eventID");
            if (value.HasValue)
            {
                return int.Parse(value.ToString()) == eventID;
            }
            else
            {
                return false;
            }
        }
        public void SaveRedisEventID(int eventID)
        {
            _cache.StringSet($"{_cacheEnvKey}-eventID", eventID.ToString());
        }

        public bool EqualRedisContestListCount(int count)
        {
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-ContestListCount");
            if (value.HasValue)
            {
                return int.Parse(value.ToString()) == count;
            }
            else
            {
                return false;
            }
        }
        public void SaveRedisContestListCount(int count)
        {
            _cache.StringSet($"{_cacheEnvKey}-ContestListCount", count.ToString());
        }
        public bool ExistRedisPlayerPoints()
        {
            if (!_redisConnected)
            {
                return false;
            }
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-playerPoints");
            return value.HasValue;
        }
        public void SaveRedisPlayerPoints(IEnumerable<PlayStandingLiteDto> playerPoints)
        {

            _cache.StringSet($"{_cacheEnvKey}-playerPoints", JsonConvert.SerializeObject(playerPoints));
        }
        public IEnumerable<PlayStandingLiteDto> ReadRedisPlayerPoints()
        {
            
            IEnumerable<PlayStandingLiteDto> playerPoints = JsonConvert.DeserializeObject<IEnumerable<PlayStandingLiteDto>>(_cache.StringGet($"{_cacheEnvKey}-playerPoints"));
            return playerPoints;
        }
        public bool ExistRedisPlayContestStandingList(int count)
        {
            if (!_redisConnected)
            {
                return false;
            }
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-{count.ToString()}-playContestStandingList");
            return value.HasValue;
        }
        public void SaveRedisPlayContestStandingList(List<PlayStandingLiteDto> playContestStandingList, int count)
        {

            _cache.StringSet($"{_cacheEnvKey}-{count.ToString()}-playContestStandingList", JsonConvert.SerializeObject(playContestStandingList));
        }
        public List<PlayStandingLiteDto> ReadRedisPlayContestStandingList(int count)
        {

            List<PlayStandingLiteDto> playContestStandingList = JsonConvert.DeserializeObject<List<PlayStandingLiteDto>> (_cache.StringGet($"{_cacheEnvKey}-{count.ToString()}-playContestStandingList"));
            return playContestStandingList;
        }
        public bool ExistRedisBullPageList(int start, int column, string sort)
        {
            if (!_redisConnected)
            {
                return false;
            }
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-bull-page-{start.ToString()}-{column.ToString()}-{sort}");
            return value.HasValue;
        }
        public void SaveRedisBullPageList(Tuple<IEnumerable<BullDto>, int> bullPageList, int start, int column, string sort)
        {

            _cache.StringSet($"{_cacheEnvKey}-bull-page-{start.ToString()}-{column.ToString()}-{sort}", JsonConvert.SerializeObject(bullPageList));
        }
        public void SaveRedisBullAll(List<Tuple<string, string>> resultBulls)
        {
            KeyValuePair<RedisKey, RedisValue>[] allBull = new KeyValuePair<RedisKey, RedisValue>[resultBulls.Count];
            int i = 0;
            foreach (var temp in resultBulls)
            {
                allBull[i] = new KeyValuePair<RedisKey, RedisValue>($"{_cacheEnvKey}{temp.Item1}", temp.Item2);
                i++;
            }
            _cache.StringSet(allBull);
        }
        public Tuple<IEnumerable<BullDto>, int> ReadRedisBullPageList(int start, int column, string sort)
        {

            Tuple<IEnumerable<BullDto>, int> bullPageList = JsonConvert.DeserializeObject<Tuple<IEnumerable<BullDto>, int>>(_cache.StringGet($"{_cacheEnvKey}-bull-page-{start.ToString()}-{column.ToString()}-{sort}"));
            return bullPageList;
        }
        
        
        public bool ExistRedisRidersPageList(int start, int column, string sort)
        {
            if (!_redisConnected)
            {
                return false;
            }
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-riders-page-{start.ToString()}-{column.ToString()}-{sort}");
            return value.HasValue;
        }
        public void SaveRedisRidersAll(List<Tuple<string, string>> resultRiders)
        {
            KeyValuePair<RedisKey, RedisValue>[] allRiders = new KeyValuePair<RedisKey, RedisValue>[resultRiders.Count];
            int i = 0;
            foreach (var temp in resultRiders)
            {
                allRiders[i] = new KeyValuePair<RedisKey, RedisValue>($"{_cacheEnvKey}{temp.Item1}", temp.Item2);
                i++;
            }
            _cache.StringSet(allRiders);
        }
        public void SaveRedisRidersPageList(Tuple<IEnumerable<RiderDto>, int> bullPageList, int start, int column, string sort)
        {

            _cache.StringSet($"{_cacheEnvKey}-riders-page-{start.ToString()}-{column.ToString()}-{sort}", JsonConvert.SerializeObject(bullPageList));
        }
        public Tuple<IEnumerable<RiderDto>, int> ReadRedisRidersPageList(int start, int column, string sort)
        {

            Tuple<IEnumerable<RiderDto>, int> bullPageList = JsonConvert.DeserializeObject<Tuple<IEnumerable<RiderDto>, int>>(_cache.StringGet($"{_cacheEnvKey}-riders-page-{start.ToString()}-{column.ToString()}-{sort}"));
            return bullPageList;
        }

        // upcoming events for eventdraw
        public bool ExistRedisEventDrawList(int eventId)
        {
            if (!_redisConnected)
            {
                return false;
            }
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-{eventId.ToString()}-EventDrawList");
            return value.HasValue;
        }
        public void SaveRedisEventDrawList(List<TeamDrawDto> eventDrawList, int eventId)
        {

            _cache.StringSet($"{_cacheEnvKey}-{eventId.ToString()}-EventDrawList", JsonConvert.SerializeObject(eventDrawList));
        }
        public List<TeamDrawDto> ReadRedisEventDrawList(int eventId)
        {

            List<TeamDrawDto> playContestStandingList = JsonConvert.DeserializeObject<List<TeamDrawDto>>(_cache.StringGet($"{_cacheEnvKey}-{eventId.ToString()}-EventDrawList"));
            return playContestStandingList;
        }
        // upcoming events for extra bulls
        public bool ExistRedisExtraBullsList(int eventId)
        {
            if (!_redisConnected)
            {
                return false;
            }
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-{eventId.ToString()}-ExtraBullsList");
            return value.HasValue;
        }
        public void SaveRedisExtraBullsList(List<TeamBullDrawDto> extraBulls, int eventId)
        {

            _cache.StringSet($"{_cacheEnvKey}-{eventId.ToString()}-ExtraBullsList", JsonConvert.SerializeObject(extraBulls));
        }
        public List<TeamBullDrawDto> ReadRedisExtraBullsList(int eventId)
        {

            List<TeamBullDrawDto> extraBulls = JsonConvert.DeserializeObject<List<TeamBullDrawDto>>(_cache.StringGet($"{_cacheEnvKey}-{eventId.ToString()}-ExtraBullsList"));
            return extraBulls;
        }
        // Event result
        public bool ExistRedisEventResultList(int eventId)
        {
            if (!_redisConnected)
            {
                return false;
            }
            RedisValue value = _cache.StringGet($"{_cacheEnvKey}-{eventId.ToString()}-EventResult");
            return value.HasValue;
        }
        public void SaveRedisEventResultList(List<EventResultDto> eventResults, int eventId)
        {

            _cache.StringSet($"{_cacheEnvKey}-{eventId.ToString()}-EventResult", JsonConvert.SerializeObject(eventResults));
        }
        public List<EventResultDto> ReadRedisEventResultList(int eventId)
        {

            List<EventResultDto> eventResults = JsonConvert.DeserializeObject<List<EventResultDto>>(_cache.StringGet($"{_cacheEnvKey}-{eventId.ToString()}-EventResult"));
            return eventResults;
        }
    }
}
