using RR.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface INewsLetterService :IDisposable
    {
        /// <summary>
        /// Add news letter.
        /// </summary>
        /// <returns></returns>
        Task AddNewsLetter(SubscribeDto subscribe);
    }
}
