using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class ProfileUpdate 
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public Guid? studentId { get; set; }
        [Required(ErrorMessage = "Không tìm thấy thông tin hồ sơ")]
        public Guid? profileTemplateId { get; set; }
        public bool? option { get; set; }
        public string text { get; set; }
    }
    public class ProfileUpdates
    {
        [Required(ErrorMessage = "Không tìm thấy dữ liệu")]
        public List<ProfileUpdate> Items { get; set; }
    }
}
