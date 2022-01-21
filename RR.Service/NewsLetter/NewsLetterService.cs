using RR.Data;
using RR.Dto;
using RR.Repo;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
    public class NewsLetterService :INewsLetterService
    {
        #region Constructor 

        private readonly IRepository<NewsLetterSubscribe, RankRideContext> _repoNewsLetter;

        public NewsLetterService(IRepository<NewsLetterSubscribe, RankRideContext> repoNewsLetter)
        {
            _repoNewsLetter = repoNewsLetter;
        }

        #endregion
        /// <summary>
        /// Add news letter.
        /// </summary>
        /// <returns></returns>
        public async Task AddNewsLetter(SubscribeDto subscribe)
        {
            NewsLetterSubscribe newsLetterEntity = _repoNewsLetter.Query()
                    .Filter(x => x.Email.ToLower() == subscribe.Email.ToLower())
                    .Get()
                    .FirstOrDefault();

            if (newsLetterEntity == null)
            {
                newsLetterEntity = new NewsLetterSubscribe();
                newsLetterEntity.Email = subscribe.Email;
                newsLetterEntity.CreatedOn = DateTime.Now;
                await _repoNewsLetter.InsertAsync(newsLetterEntity);
            }
        }

        /// <summary>
        /// Dispose NewsLetter Service
        /// </summary>
        public void Dispose()
        {
            if (_repoNewsLetter != null)
            {
                _repoNewsLetter.Dispose();
            }
        }
    }
}
