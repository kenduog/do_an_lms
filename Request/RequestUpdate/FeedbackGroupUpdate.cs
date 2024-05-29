using Request.DomainRequests;
using System;
using System.ComponentModel.DataAnnotations;

namespace Request.RequestCreate
{
    public class FeedbackGroupUpdate : DomainUpdate
    {
        public string name_vi { get; set; }
    }
}
