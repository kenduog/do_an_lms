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
using Entities.DataTransferObject;
using OfficeOpenXml.ConditionalFormatting;
using System.Runtime;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Microsoft.CodeAnalysis.Operations;
using System.Drawing.Printing;

namespace Service.Services
{
    public class DropdownService : IDropdownService
    {
        private readonly IAppUnitOfWork unitOfWork;
        private readonly IAppDbContext appDbContext;
        public DropdownService(IAppUnitOfWork unitOfWork, IAppDbContext appDbContext)
        {
            this.unitOfWork = unitOfWork;
            this.appDbContext = appDbContext;
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
                .OrderBy(x => x.name).ToListAsync();
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
        public async Task<List<GroupOption>> GroupOption(GroupOptionSearch search)
        {
            var data = await unitOfWork.Repository<tbl_Group>().GetQueryable()
                .Where(x => x.deleted == false && x.active == true /* && x.code != "QTV"*/)
                .Select(x => new GroupOption
                {
                    id = x.id,
                    code = x.code,
                    name = x.name,
                    isStaff = x.code.Contains("PH") ? false : true
                }).OrderBy(x => x.name).ToListAsync();
            if (search.isStaff != null)
                data = data.Where(x => x.isStaff == search.isStaff).ToList();
            return data;
        }

        /// <summary>
        /// Group staff options
        /// </summary>
        /// <returns></returns>
        public async Task<List<GroupOption>> GroupStaffOption()
        {
            var data = await unitOfWork.Repository<tbl_Group>().GetQueryable()
                .Where(x => x.deleted == false && x.active == true && x.code != "QTV" && x.code != "PH" && x.code != "GV")
                .Select(x => new GroupOption { id = x.id, name = x.name, code = x.code }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// Feedback group staff options
        /// </summary>
        /// <returns></returns>
        public async Task<List<GroupOption>> FeedbackGroupStaffOption()
        {
            var data = await unitOfWork.Repository<tbl_Group>().GetQueryable()
                .Where(x => x.deleted == false && x.active == true && x.code != "QTV" && x.code != "PH")
                .Select(x => new GroupOption { id = x.id, name = x.name, code = x.code }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// School year options
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectedOption>> SchoolYearOption()
        {
            var now = Timestamp.Now();
            var data = await unitOfWork.Repository<tbl_SchoolYear>().GetQueryable()
                .Where(x => x.deleted == false && x.active == true)
                .Select(x => new SelectedOption
                {
                    id = x.id,
                    name = x.name,
                    selected = (x.sTime <= now && now <= x.eTime)
                }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// Grade options
        /// </summary>
        /// <returns></returns>
        public async Task<List<DomainOption>> GradeOption(Guid branchId)
        {
            var data = await unitOfWork.Repository<tbl_Grade>().GetQueryable()
                .Where(x => x.deleted == false && x.active == true && x.branchId == branchId)
                .Select(x => new DomainOption { id = x.id, name = x.name }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// Class options
        /// </summary>
        /// <returns></returns>
        public async Task<List<DomainOption>> ClassOption(Guid gradeId)
        {
            var data = await unitOfWork.Repository<tbl_Class>().GetQueryable()
                .Where(x => x.deleted == false && x.active == true && x.gradeId == gradeId)
                .Select(x => new DomainOption { id = x.id, name = x.name }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// Branch options
        /// </summary>
        /// <returns></returns>
        public async Task<List<BranchOption>> BranchOption()
        {
            var userId = LoginContext.Instance.CurrentUser.userId;
            var user = await unitOfWork.Repository<tbl_Users>()
                .GetQueryable().FirstOrDefaultAsync(x => x.id == userId);
            bool isAdmin = false;

            if (user.isSuperUser == true)
                isAdmin = true;
            else
            {
                var adminGroup = await unitOfWork.Repository<tbl_Group>()
                    .GetQueryable().FirstOrDefaultAsync(x => x.code == "QTV");
                if (adminGroup == null)
                    isAdmin = false;
                isAdmin = await unitOfWork.Repository<tbl_UserGroup>()
                    .GetQueryable().AnyAsync(x => x.userId == userId && x.groupId == adminGroup.id && x.deleted == false);
            }
            var data = await unitOfWork.Repository<tbl_Branch>().GetQueryable()
                .Where(x => x.deleted == false && x.active == true && (user.branchIds.Contains(x.id.ToString()) || isAdmin))
                .Select(x => new BranchOption { id = x.id, name = x.name, logo = x.logo, description = x.description, type = x.type }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// Semester options
        /// </summary>
        /// <returns></returns>
        public async Task<List<SemesterOption>> SemesterOption(Guid? schoolYearIdId)
        {
            var schoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().Validate(schoolYearIdId ?? Guid.Empty) ?? throw new AppException(MessageContants.nf_schoolYear);
            var now = Timestamp.Now();
            var data = await unitOfWork.Repository<tbl_Semester>().GetQueryable()
                .Where(x => x.deleted == false && x.schoolYearId == schoolYearIdId)
                .Select(x => new SemesterOption
                {
                    id = x.id,
                    name = x.name,
                    selected = (x.sTime <= now && now <= x.eTime),
                    sTime = schoolYear.sTime,
                    eTime = schoolYear.eTime
                }).OrderBy(x => x.name).ToListAsync();

            return data;
        }
        public async Task<List<WeekOption>> WeekOption(Guid? semesterId)
        {
            var now = Timestamp.Now();
            var data = await unitOfWork.Repository<tbl_Week>().GetQueryable()
                .Where(x => x.deleted == false && x.semesterId == semesterId)
                .Select(x => new WeekOption { id = x.id, sTime = x.sTime, eTime = x.eTime, name = x.name, selected = (x.sTime < now && now < x.eTime) })
                .OrderBy(x => x.sTime).ToListAsync();
            return data;
        }
        public async Task<List<DomainOption>> DepartmentOption()
        {
            var data = await unitOfWork.Repository<tbl_Departments>().GetQueryable()
                .Where(x => x.deleted == false)
                .Select(x => new DomainOption { id = x.id, name = x.name }).OrderBy(x => x.name).ToListAsync();
            return data;
        }
        public async Task<List<DomainOption>> HighestLevelOfEducationOption()
        {
            var data = await unitOfWork.Repository<tbl_HighestLevelOfEducation>().GetQueryable()
                .Where(x => x.deleted == false)
                .Select(x => new DomainOption { id = x.id, name = x.name }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// Study shift filter option
        /// </summary>
        /// <returns></returns>
        public async Task<List<double>> StudyShiftFilterOption()
        {
            var sqlParameters = new SqlParameter[] { };
            var data = await unitOfWork.Repository<tbl_StudyShift>().GetQueryable()
                .Where(x => x.deleted == false && x.sTime.HasValue && x.eTime.HasValue)
                .Select(x => (x.eTime.Value - x.sTime.Value) / 60000)
                .Distinct()
                .ToListAsync();
            if (data != null && data.Count > 0) data.Sort();
            return data;
        }
        /// <summary>
        /// thêm Lọc theo chi nhánh nữa
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<List<UserOption>> StaffOptionByCode(StaffOptionSearch search)
        {
            var group = await unitOfWork.Repository<tbl_Group>()
                .GetQueryable().FirstOrDefaultAsync(x => x.code.ToUpper() == search.code.ToUpper() && x.deleted == false);
            var branch = await this.unitOfWork.Repository<tbl_Branch>().Validate(search.branchId ?? Guid.Empty) ?? throw new AppException(MessageContants.nf_branch);
            if (group == null)
                return new List<UserOption>();
            var item = await Task.Run(() =>
            {
                var result = new List<UserOption>();
                DataTable dataTable = new DataTable();
                Microsoft.Data.SqlClient.SqlConnection connection = null;
                Microsoft.Data.SqlClient.SqlCommand command = null;
                try
                {
                    connection = (Microsoft.Data.SqlClient.SqlConnection)appDbContext.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = $"Get_StaffOption";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@groupId", group.id);
                    command.Parameters.AddWithValue("@branchId", branch.id);
                    Microsoft.Data.SqlClient.SqlDataAdapter sqlDataAdapter = new Microsoft.Data.SqlClient.SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    result = ConvertToList(dataTable);
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
                return result;
            });
            //var listStaff = new List<UserOption>();
            //foreach (var i in item)
            //{
                
            //    if (i.code.StartsWith(search.code))
            //        listStaff.Add(i);
            //}

            return item;
        }
        public static List<UserOption> ConvertToList(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(UserOption).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<UserOption>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch
                        {
                        }
                    }
                }
                return objT;
            }).ToList();
        }
        public static List<TeacherOption> ConvertToListTeacher(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(TeacherOption).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<TeacherOption>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch
                        {
                        }
                    }
                }
                return objT;
            }).ToList();
        }
        public async Task<List<DiscountOption>> DiscountOption(Guid tuitionConfigId)
        {
            double now = Timestamp.Now();
            var data = await unitOfWork.Repository<tbl_Discount>().GetQueryable()
                .Where(x => x.deleted == false && x.tuitionConfigId == tuitionConfigId)
                .Select(x => new DiscountOption
                {
                    id = x.id,
                    name = x.name,
                    description = x.description,
                    type = x.type,
                    typeName = x.typeName,
                    value = x.value,
                    maximumValue = x.maximumValue
                }).OrderBy(x => x.name).ToListAsync();
            return data;
        }

        /// <summary>
        /// Thêm học học viên nữa
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserOption>> StudentOption(StudentOptionSearch search)
        {
            var data = await unitOfWork.Repository<tbl_Student>().GetDataExport("Get_StudentOption", new SqlParameter[]
            {
                new SqlParameter("branchId", search.branchId),
                new SqlParameter("gradeId", search.gradeId),
                new SqlParameter("classId", search.classId),
            });

            return data.Select(x => new UserOption
            {
                id = x.id,
                code = x.code,
                name = x.fullName
            }).ToList();
        }

        public async Task<List<DomainOption>> SubjectGroupOption(Guid branchId)
            => await unitOfWork.Repository<tbl_SubjectGroup>().GetQueryable()
            .Where(x => x.deleted == false && x.branchId == branchId)
            .Select(x => new DomainOption { id = x.id, name = x.name })
            .ToListAsync();

        public async Task<List<SubjectOption>> SubjectOption(OptionSearch search)
        {
            var result = new List<SubjectOption>();
            var data = await unitOfWork.Repository<tbl_Subject>().GetDataExport("Get_SubjectDropdown", new SqlParameter[]
               {
                new SqlParameter("schoolYearId", search.schoolYearId),
                new SqlParameter("branchId", search.branchId),
               });
            if (data.Any())
            {
                result = data.Select(x => new SubjectOption { id = x.id, acronym = x.acronym, name = x.name }).ToList();
            }
            return result;
        }

        public async Task<List<DomainOption>> CriteriaOption(Guid gradeId)
            => await unitOfWork.Repository<tbl_Criteria>().GetQueryable()
            .Where(x => x.deleted == false && x.gradeId == gradeId)
            .Select(x => new DomainOption { id = x.id, name = x.name })
            .ToListAsync();

        public async Task<List<DomainOption>> RoomOption(Guid branchId)
         => await unitOfWork.Repository<tbl_Room>().GetQueryable()
         .Where(x => x.deleted == false && x.branchId == branchId)
         .Select(x => new DomainOption { id = x.id, name = x.name })
         .ToListAsync();

        public async Task<List<DomainOption>> UnitOfMeasureOption()
          => await unitOfWork.Repository<tbl_UnitOfMeasure>().GetQueryable()
          .Where(x => x.deleted == false)
          .Select(x => new DomainOption { id = x.id, name = x.name })
          .ToListAsync();

        public async Task<List<DomainOption>> ItemGroupOption()
         => await unitOfWork.Repository<tbl_ItemGroup>().GetQueryable()
         .Where(x => x.deleted == false)
         .Select(x => new DomainOption { id = x.id, name = x.name })
         .ToListAsync();

        public async Task<List<DomainOption>> FeedbackGroupOption(Guid branchId)
         => await unitOfWork.Repository<tbl_FeedbackGroup>().GetQueryable()
         .Where(x => x.deleted == false && x.branchId == branchId)
         .Select(x => new DomainOption { id = x.id, name = x.name_vi })
         .ToListAsync();


        public async Task<List<LookupOption>> LookupOption(string typeCode)
        {
            var data = await appDbContext.Set<tbl_Lookup>()
                .Where(x => x.lookupTypeCode == typeCode && x.deleted == false)
                .Select(x => new LookupOption { id = x.id, code = x.code, name = x.name })
                .ToListAsync();
            return data;
        }

        public async Task<List<DomainOption>> VendorOption()
         => await unitOfWork.Repository<tbl_Vendor>().GetQueryable()
         .Where(x => x.deleted == false)
         .Select(x => new DomainOption { id = x.id, name = x.name })
         .ToListAsync();

        public async Task<List<ItemOption>> ItemSKUOption(Guid branchId)
        {
            var items = await this.unitOfWork.Repository<tbl_Item>().GetQueryable().Where(x => x.deleted == false && x.branchId == branchId)
                .Select(x => x.id).ToListAsync();

            var unitOfMeasure = await this.unitOfWork.Repository<tbl_UnitOfMeasure>().GetQueryable().ToDictionaryAsync(x => x.id, x => x.name);

            var data = await unitOfWork.Repository<tbl_ItemOfSKU>().GetQueryable()
                .Where(x => x.deleted == false && x.itemId.HasValue && items.Contains(x.itemId.Value))
                .Select(x => new ItemOption
                {
                    itemSkuId = x.id,
                    itemId = x.itemId.Value,
                    code = x.code,
                    name = x.name,
                    convertQty = x.convertQty,
                    unitOfMeasureId = x.unitOfMearsureId,
                    unitPrice = x.unitPrice
                })
                .OrderBy(x => x.name)
                .ToListAsync();

            foreach (var item in data)
            {
                if (unitOfMeasure.TryGetValue(item.unitOfMeasureId ?? Guid.Empty, out string name))
                {
                    item.unitOfMeasureName = name;
                }
            }
            return data;
        }

        public async Task<List<ItemOption>> ItemOption(Guid branchId)
        {
            var items = await this.unitOfWork.Repository<tbl_Item>().GetDataExport("Get_ItemOption", new SqlParameter[] { new SqlParameter("branchId", branchId) });
            return items.Select(x => new Entities.DomainEntities.ItemOption
            {
                itemId = x.id,
                code = x.code,
                name = x.name,
                unitPrice = x.unitPrice,
                essenceRate = x.essenceRate,
                weightPerUnit = x.weightPerUnit,
                unitOfMeasureName = x.unitOfMeasureName,
                calo = x.calo,
                lipit = x.lipit,
                gluxit = x.gluxit,
                protein = x.protein
            }).ToList();
        }

        public async Task<List<ParentOption>> ParentOption(Guid branchId)
        {
            List<ParentOption> result = new List<ParentOption>();

            var items = await this.unitOfWork.Repository<tbl_Parent>().GetDataExport("Get_ParentOption", new SqlParameter[] { new SqlParameter("branchId", branchId) });

            if (items.Any())
            {
                result = items.Select(x => new Entities.DomainEntities.ParentOption
                {
                    id = x.id,
                    code = x.code,
                    name = x.fullName,
                    phone = x.phone
                }).OrderBy(x => x.name).ToList();
            }
            return result;
        }

        public async Task<List<FoodOption>> FoodOption(Guid branchId)
        {
            var items = await this.unitOfWork.Repository<tbl_Food>().GetDataExport("Get_FoodOption", new SqlParameter[] { new SqlParameter("branchId", branchId) });
            return items.Select(x => new Entities.DomainEntities.FoodOption
            {
                foodId = x.id,
                name = x.name,
                description = x.description,
                calo = x.calo,
                protein = x.protein,
                lipit = x.lipit,
                gluxit = x.gluxit
            }).ToList();
        }

        public async Task<List<DomainOption>> NutritionGroupOption(NutritionGroupOptionSearch request)
         => await unitOfWork.Repository<tbl_NutritionGroup>().GetQueryable()
         .Where(x => x.deleted == false && x.branchId == request.branchId && x.gradeIds.Contains(request.gradeId.ToString()))
         .Select(x => new DomainOption { id = x.id, name = x.name })
         .ToListAsync();

        public async Task<List<MenuOption>> MenuOption(Guid branchId)
        {
            List<MenuOption> result = new List<MenuOption>();

            var items = await this.unitOfWork.Repository<tbl_Menu>().GetDataExport("Get_MenuOption", new SqlParameter[] { new SqlParameter("branchId", branchId) });

            if (items.Any())
            {
                result = items.Select(x => new Entities.DomainEntities.MenuOption
                {
                    id = x.id,
                    name = x.name,
                    gradeName = x.gradeName,
                    nutritionGroupName = x.nutritionGroupName
                }).OrderBy(x => x.name).ToList();
            }
            return result;

        }

        public async Task<List<PurchaseOrderOption>> PurchaseOrderOption(Guid branchId)
        {
            List<PurchaseOrderOption> result = new List<PurchaseOrderOption>();

            var items = await this.unitOfWork.Repository<tbl_PurchaseOrderHeader>().GetQueryable()
                .Where(x => x.deleted == false && x.statusCode == LookupConstant.TrangThai_DaDuyet && x.branchId == branchId)
                .ToListAsync();

            if (items.Any())
            {
                result = items.Select(x => new PurchaseOrderOption
                {
                    id = x.id,
                    date = x.date,
                    code = x.code,
                    description = x.description
                }).OrderByDescending(x => x.date).ToList();
            }
            return result;
        }

        public async Task<List<FeeOption>> FeeByGradeOption(FeeOptionSearch search)
        {
            List<FeeOption> result = new List<FeeOption>();

            var items = await this.unitOfWork.Repository<tbl_FeeInGrade>().GetDataExport("Get_FeeOption", new SqlParameter[]
            {
                new SqlParameter("branchId", search.branchId),
                new SqlParameter("gradeId", search.gradeId)
            });

            if (items.Any())
            {
                result = items.Select(x => new Entities.DomainEntities.FeeOption
                {
                    id = x.id,
                    name = x.name,
                    description = x.description,
                    collectionType = x.collectionType,
                    gradeName = x.gradeName,
                    feeId = x.feeId,
                    price = x.price
                }).OrderBy(x => x.name).ToList();
            }
            return result;
        }

        public async Task<List<DomainOption>> FeeOption(Guid branchId)
        => await unitOfWork.Repository<tbl_Fee>().GetQueryable()
        .Where(x => x.deleted == false && x.branchId == branchId)
        .Select(x => new DomainOption { id = x.id, name = x.name })
        .ToListAsync();

        public async Task<List<DomainOption>> FeeReductionOption(Guid branchId)
        => await unitOfWork.Repository<tbl_FeeReduction>().GetQueryable()
        .Where(x => x.deleted == false && x.branchId == branchId)
        .Select(x => new DomainOption { id = x.id, name = x.name })
        .ToListAsync();

        public async Task<List<PaymentBankOption>> PaymentBankOption()
       => await unitOfWork.Repository<tbl_PaymentBank>().GetQueryable()
       .Where(x => x.deleted == false)
       .Select(x => new PaymentBankOption { id = x.id, name = x.name, icon = x.icon })
       .ToListAsync();

        public async Task<List<PaymentMethodOption>> PaymentMethodOption(Guid branchId)
       => await unitOfWork.Repository<tbl_PaymentMethod>().GetQueryable()
       .Where(x => x.deleted == false && x.branchId == branchId)
       .Select(x => new PaymentMethodOption { id = x.id, name = x.name, location = x.location, accountName = x.accountName, accountNumber = x.accountNumber })
       .ToListAsync();

        public async Task<List<DomainOption>> CollectionPlanOption(Guid branchId)
       => await unitOfWork.Repository<tbl_CollectionPlan>().GetQueryable()
       .Where(x => x.deleted == false && x.branchId == branchId)
       .Select(x => new DomainOption { id = x.id, name = x.name })
       .ToListAsync();
        public async Task<List<StudyShiftOption>> StudyShiftOption()
            => await unitOfWork.Repository<tbl_StudyShift>().GetQueryable()
            .Where(x => x.deleted == false)
            .Select(x => new StudyShiftOption
            {
                id = x.id,
                eTime = x.eTime,
                sTime = x.sTime
            }).ToListAsync();
        public async Task<List<StudySessionOption>> StudySessionOption(Guid branchId)
            => await unitOfWork.Repository<tbl_StudySession>().GetQueryable()
            .Where(x => x.branchId == branchId && x.deleted == false)
            .Select(x => new StudySessionOption
            {
                id = x.id,
                branchId = x.branchId,
                eTime = x.eTime,
                sTime = x.sTime,
                index = x.index
            }).ToListAsync();
        public async Task<List<DomainOption>> SubjectInClassOption(Guid classId)
        {
            var subjectInClass = await unitOfWork.Repository<tbl_TimeTableDetail>().GetQueryable()
             .Where(x => x.classId == classId && x.deleted == false).Select(x => x.subjectId).ToListAsync();
            var result = new List<DomainOption>();
            foreach(var s in subjectInClass.Distinct())
            {
                var subject = await unitOfWork.Repository<tbl_Subject>().GetQueryable()
             .FirstOrDefaultAsync(x => x.id == s && x.deleted == false);
                if (subject != null)
                    result.Add(new DomainOption
                    {
                        id = subject.id,
                        name = subject.name,
                    });
            }
            return result;
        }
        public async Task<List<TeacherOption>> TeacherNotAssignmentOption(TeacherNotAssignmentOptionSearch search)
        {
            var group = await unitOfWork.Repository<tbl_Group>()
                .GetQueryable().FirstOrDefaultAsync(x => x.code.ToUpper() == "GV" && x.deleted == false);
            var branch = await this.unitOfWork.Repository<tbl_Branch>().Validate(search.branchId) ?? throw new AppException(MessageContants.nf_branch);
            var schoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().Validate(search.schoolYearId) ?? throw new AppException(MessageContants.nf_schoolYear);
            if (group == null)
                return new List<TeacherOption>();
            return await Task.Run(() =>
            {
                var result = new List<TeacherOption>();
                DataTable dataTable = new DataTable();
                Microsoft.Data.SqlClient.SqlConnection connection = null;
                Microsoft.Data.SqlClient.SqlCommand command = null;
                try
                {
                    connection = (Microsoft.Data.SqlClient.SqlConnection)appDbContext.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = $"Get_TeacherNoAssignmentOption";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@groupId", group.id);
                    command.Parameters.AddWithValue("@branchId", branch.id);
                    command.Parameters.AddWithValue("@schoolYearId", schoolYear.id);
                    Microsoft.Data.SqlClient.SqlDataAdapter sqlDataAdapter = new Microsoft.Data.SqlClient.SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    result = ConvertToListTeacher(dataTable);
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
                return result;
            });
        }

        public async Task<List<DomainOption>> TuitionConfigCategoryOption()
        {
            return await unitOfWork.Repository<tbl_TuitionConfigCategory>()
                .GetQueryable()
                .Where(x => x.deleted == false)
                .Select(x => new DomainOption
                {
                    id = x.id,
                    name= x.name
                }).ToListAsync();
        }
        public async Task<List<TuitionConfigOption>> TuitionConfigOption()
        {
            var categorys = await TuitionConfigCategoryOption();
            var data = await unitOfWork.Repository<tbl_TuitionConfig>()
                .GetQueryable()
                .Where(x => x.deleted == false)
                .Select(x => new TuitionConfigOption
                {
                    id = x.id,
                    description = x.description,
                    name = x.name,
                    price = x.price,
                    thumbnail = x.thumbnail,
                    unit = x.unit,
                    tuitionConfigCategoryId = x.tuitionConfigCategoryId,
                }).ToListAsync();
            return (from i in data
                    join c in categorys on i.tuitionConfigCategoryId equals c.id
                    select new TuitionConfigOption
                    {
                        id = i.id,
                        description = i.description,
                        name = i.name,
                        price = i.price,
                        thumbnail = i.thumbnail,
                        unit = i.unit,
                        tuitionConfigCategoryId = i.tuitionConfigCategoryId,
                        tuitionConfigCategoryName = c.name
                    }).ToList();
        }
    }
}
