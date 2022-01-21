using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class CommonService : ICommonService
    {
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly IStateService _stateService;

        public CommonService(ICountryService countryService,
             ICityService cityService,
             IStateService stateService)
        {
            _countryService = countryService;
            _cityService = cityService;
            _stateService = stateService;
        }

        public async Task<List<SelectListItem>> GetCountries()
        {
            try
            {
                var countries = await _countryService.GetCountries();
                return countries.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name, Selected = Convert.ToInt16(c.Id) == 99 }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GetCountryById(int? id)
        {
            try
            {
                if (id == null)
                    return string.Empty;
                var country = await _countryService.GetCountryById(id);
                return country.Name;
            }
            catch (Exception ex)
            {
                return "";
                //return ex.Message;
            }
        }

        public async Task<string> GetStateById(int? id)
        {
            try
            {
                if (id == null)
                    return string.Empty;
                var state = await _stateService.GetStateById(id);
                return state.Name;
            }
            catch (Exception ex)
            {
                return "";
                //return ex.Message;
            }
        }

        public async Task<string> GetCityById(int? id)
        {
            try
            {
                if (id == null)
                    return string.Empty;
                var city = await _cityService.GetCityById(id);
                return city.Name;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<List<SelectListItem>> GetCitiesByCountryId(int id)
        {
            try
            {
                var cities = await _cityService.GetCitiesByCountryId(id);
                return cities.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<SelectListItem>> GetCitiesByStateId(int id)
        {
            try
            {
                var cities = await _cityService.GetCitiesByStateId(id);
                return cities.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<SelectListItem>> GetStates(int id)
        {
            try
            {
                var states = await _stateService.GetStates(id);
                return states.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected void Dispose()
        {
            if (_countryService != null)
            {
                _countryService.Dispose();
            }
            if (_cityService != null)
            {
                _cityService.Dispose();
            }
            if (_stateService != null)
            {
                _stateService.Dispose();
            }
        }
    }
}