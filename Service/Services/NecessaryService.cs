using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Service.Services.DomainServices;
using Entities.Search;
using Newtonsoft.Json;
using Entities.DomainEntities;
using System.Net.Mail;
using Request.DomainRequests;
using Microsoft.Extensions.Configuration;
using static Utilities.CoreContants;
using Entities.AuthEntities;

namespace Service.Services
{
    public class NecessaryService : DomainService<tbl_Necessary, BaseSearch>, INecessaryService
    {
        private IConfiguration configuration;
        public NecessaryService(IAppUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper) : base(unitOfWork, mapper)
        {
            this.configuration = configuration;
        }
        public async Task SendMail(SendMailModel model)
        {
            try
            {
                await Task.Run(() =>
                {
                    string fromAddress = configuration.GetSection("MySettings:Email").Value.ToString();
                    string mailPassword = configuration.GetSection("MySettings:PasswordMail").Value.ToString(); 
                    SmtpClient client = new SmtpClient();
                    client.Port = 587;//outgoing port for the mail.
                    client.Host = "smtp.gmail.com";
                    client.EnableSsl = true;
                    client.Timeout = 1000000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(fromAddress, mailPassword);

                    // Fill the mail form.
                    var send_mail = new MailMessage();
                    send_mail.IsBodyHtml = true;
                    //address from where mail will be sent.
                    send_mail.From = new MailAddress(fromAddress);
                    //address to which mail will be sent.           
                    send_mail.To.Add(new MailAddress(model.to));
                    //subject of the mail.
                    send_mail.Subject = model.title;
                    send_mail.Body = model.content;
                    client.Send(send_mail);
                });
            }
            catch {}
        }
        /// <summary>
        /// City options
        /// </summary>
        /// <returns></returns>
        public async Task<List<DomainOption>> CityOption()
        {
            var data = await unitOfWork.Repository<tbl_Cities>().GetQueryable()
                .Where(x => x.deleted == false)
                .Select(x => new DomainOption { id = x.id, name = x.name })
                .OrderBy(x=>x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// District options
        /// </summary>
        /// <returns></returns>
        public async Task<List<DomainOption>> DistrictOption(Guid? cityId)
        {
            var data = await unitOfWork.Repository<tbl_Districts>().GetQueryable()
                .Where(x => x.deleted == false && x.cityId == cityId)
                .Select(x => new DomainOption { id = x.id, name = x.name }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// Ward options
        /// </summary>
        /// <returns></returns>
        public async Task<List<DomainOption>> WardOption(Guid? districtId)
        {
            var data = await unitOfWork.Repository<tbl_Wards>().GetQueryable()
                .Where(x => x.deleted == false && x.districtId == districtId)
                .Select(x => new DomainOption { id = x.id, name = x.name }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// Group options
        /// </summary>
        /// <returns></returns>
        public async Task<List<DomainOption>> GroupOption()
        {
            var data = await unitOfWork.Repository<tbl_Group>().GetQueryable()
                .Where(x => x.deleted == false && x.active == true)
                .Select(x => new DomainOption { id = x.id, name = x.name }).OrderBy(x => x.name).ToListAsync();
            return data;
        }
    }
}
