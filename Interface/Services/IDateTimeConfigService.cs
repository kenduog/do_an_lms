using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Interface.Services
{
    public interface IDateTimeConfigService
    {
        Task<DateTime?> DoubleToDateTime(double value);
        Task<string> DoubleToString(double value, string type);
        Task<string> DoubleToStringExcel(string value, string type);
        Task<double?> StringToDouble(string value, string type);
        Task<double?> DateTimeToMilliseconds(DateTime value);
    }
}
