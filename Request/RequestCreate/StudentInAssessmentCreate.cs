using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class StudentInAssessmentCreate : DomainCreate
    {
        public List<StudentModel> studentModels { get; set; }
        public List<DetailTopicModel> detailTopicModels { get; set; }
    }
    public class StudentModel
    {
        public Guid? studentId { get; set; }
        public Guid? semesterId { get; set; }
        public Guid? assessmentTopicId { get; set; }
    }
    public class DetailTopicModel
    {
        public Guid? assessmentDetailId { get; set; }
        public bool? status { get; set; }
    }
}
