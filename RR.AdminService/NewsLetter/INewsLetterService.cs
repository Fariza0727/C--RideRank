using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface INewsLetterService : IDisposable
     {  
          /// <summary>
          /// Get All News
          /// </summary>
          /// <param name="start">Page Number</param>
          /// <param name="length">Number Of Record </param>
          /// <param name="searchStr"></param>
          /// <param name="sort"></param>
          /// <returns></returns>
          Task<Tuple<IEnumerable<SubscribeDto>, int>> GetAllNewsLetterSubscribers(int start, int length, int column, string searchStr = "", string sort = "");
          
          /// <summary>
          /// Delete News By Id
          /// </summary>
          /// <param name="Id">News Id</param>
          /// <returns></returns>
          void DeleteNewsLetter(int newsletterId);
     }
}
