using AutoMapper;
using Entities;
using Entities.DataTransferObject;
using Entities.Search;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Request.RequestCreate;
using Request.RequestUpdate;
using Service.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Utilities;

namespace Service.Services
{
    public class TimeTableService : DomainService<tbl_TimeTable, TimeTableSearch>, ITimeTableService
    {
        private readonly IAppDbContext appDbContext;
        public TimeTableService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext appDbContext) : base(unitOfWork, mapper)
        {
            this.appDbContext = appDbContext;
        }

        protected override string GetStoreProcName()
        {
            return "Get_TimeTable";
        }

        public async Task<TimeTableResponse> CreateTimeTable(TimeTableCreate model)
        {
            TimeTableResponse result = new TimeTableResponse() { schedules = new List<TimeTableItem>() };

            //validate 
            var schoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.schoolYearId)
                ?? throw new AppException(MessageContants.nf_schoolYear);
            var branch = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.branchId)
               ?? throw new AppException(MessageContants.nf_branch);
            var semester = await this.unitOfWork.Repository<tbl_Semester>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.semesterId && x.schoolYearId == schoolYear.id)
               ?? throw new AppException(MessageContants.nf_semester);
            var grade = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.gradeId)
              ?? throw new AppException(MessageContants.nf_grade);
            var week = await this.unitOfWork.Repository<tbl_Week>().GetQueryable().FirstOrDefaultAsync(x =>
                x.sTime <= model.startDay &&
                x.eTime >= model.startDay &&
                x.deleted == false &&
                x.schoolYearId == model.schoolYearId &&
                x.semesterId == model.semesterId
               ) ?? throw new AppException(MessageContants.day_not_exist_in_week_config);
            //lưu tkb 
            tbl_TimeTable timeTable = new tbl_TimeTable()
            {
                name = model.name,
                schoolYearId = schoolYear.id,
                branchId = branch.id,
                semesterId = semester.id,
                gradeId = grade.id,
                active = false,
                startDay = model.startDay
            };

            result.name = timeTable.name;
            result.schoolYearId = timeTable.schoolYearId;
            result.branchId = timeTable.branchId;
            result.semesterId = timeTable.semesterId;
            result.semesterName = timeTable.semesterName;
            result.gradeId = timeTable.gradeId;
            result.gradeName = timeTable.gradeName;
            result.startDay = model.startDay;

            await this.unitOfWork.Repository<tbl_TimeTable>().CreateAsync(timeTable);
            await this.unitOfWork.SaveAsync();

            var classes = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().Where(x => x.deleted == false && x.gradeId == grade.id).ToListAsync();
            if (classes.Any())
            {
                for (int day = 2; day <= 7; day++)
                {
                    for (int index = 1; index <= 10; index++)
                    {
                        result.schedules.Add(new TimeTableItem
                        {
                            id = Guid.NewGuid(),
                            day = day,
                            index = index,
                            items = classes.ToDictionary(x => x.id, x => new Entities.TimeTableSubItem())
                        });
                    }
                }
            }
            return result;
        }

        public override async Task<bool> UpdateAsync(tbl_TimeTable item)
        {
            tbl_TimeTable timeTable = await this.unitOfWork.Repository<tbl_TimeTable>().GetQueryable().FirstOrDefaultAsync(x => x.id == item.id) ?? throw new AppException(MessageContants.nf_timeTable);
            // only check if item use not active
            if (item.active == true && timeTable.active == false)
            {
                var checkTimeTable = await this.unitOfWork.Repository<tbl_TimeTable>().GetQueryable().AnyAsync(x =>
                x.branchId == timeTable.branchId
                && x.gradeId == timeTable.gradeId
                && x.semesterId == timeTable.semesterId
                && x.active == true
                && x.deleted == false);
                if(checkTimeTable)
                    throw new AppException(MessageContants.can_not_update_only_active_timetable);
            }
            if(item.startDay == 0)
                item.startDay = timeTable.startDay;
            return await UpdateAsync(new List<tbl_TimeTable> { item });
        }

        public async Task<TimeTableResponse> GenerateTimetable(Guid id)
        {
            TimeTableResponse result = new TimeTableResponse() { schedules = new List<TimeTableItem>() };
            tbl_TimeTable timeTable = await this.unitOfWork.Repository<tbl_TimeTable>().GetQueryable().FirstOrDefaultAsync(x => x.id == id) ?? throw new AppException(MessageContants.nf_timeTable);
            result.name = timeTable.name;
            result.schoolYearId = timeTable.schoolYearId;
            result.branchId = timeTable.branchId;
            result.semesterId = timeTable.semesterId;
            result.semesterName = timeTable.semesterName;
            result.gradeId = timeTable.gradeId;
            result.gradeName = timeTable.gradeName;
            result.id = timeTable.id;
            result.active = timeTable.active;
            result.startDay = timeTable.startDay;

            List<TimeTableDetailResponse> details = await appDbContext.Set<TimeTableDetailResponse>().FromSqlRaw($"Get_TimeTableDetailNoPaging @timeTableId = '{timeTable.id}'").ToListAsync();

            var dictByClass = details.Where(x => x.classId.HasValue).GroupBy(x => new { x.day, x.index, x.classId }).ToDictionary(x => new GroupedDay
            {
                day = x.Key.day,
                index = x.Key.index,
                classId = x.Key.classId.Value
            }, x => new GroupedDayInfo
            {
                id = x.FirstOrDefault().id,
                subjectId = x.FirstOrDefault().subjectId.Value,
                teacherId = x.FirstOrDefault().teacherId.Value,
                sTime = x.FirstOrDefault().sTime,
                eTime = x.FirstOrDefault().eTime,
                roomId = x.FirstOrDefault().roomId,
                className = x.FirstOrDefault().className,
                subjectName = x.FirstOrDefault().subjectName,
                teacherName = x.FirstOrDefault().teacherName,
                roomName = x.FirstOrDefault().roomName,
                subjectType = x.FirstOrDefault().subjectType
            });

            var classIds = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().Where(x => x.deleted == false && x.gradeId == timeTable.gradeId).Select(x => x.id).ToListAsync();

            var studySessions = await this.unitOfWork.Repository<tbl_StudySession>().GetQueryable().Where(x => x.deleted == false && x.branchId == timeTable.branchId)
                .ToListAsync();

            for (int day = 2; day <= 7; day++)
            {
                for (int index = 1; index <= 10; index++)
                {
                    var studySession = studySessions.FirstOrDefault(x => x.index == index);
                    TimeTableItem itemDay = new TimeTableItem()
                    {
                        id = Guid.NewGuid(),
                        day = day,
                        index = index,
                        items = new Dictionary<Guid, TimeTableSubItem>(),
                        sTime = studySession?.sTime ?? 0,
                        eTime = studySession?.eTime ?? 0,
                    };
                    foreach (var classId in classIds)
                    {
                        var subItem = new TimeTableSubItem();
                        GroupedDay key = new GroupedDay { day = day, index = index, classId = classId };
                        GroupedDayInfo info;

                        if (dictByClass.TryGetValue(key, out info))
                        {
                            subItem.id = info.id;
                            subItem.sTime = info.sTime;
                            subItem.eTime = info.eTime;
                            subItem.teacherId = info.teacherId;
                            subItem.subjectId = info.subjectId;
                            subItem.roomId = info.roomId;
                            subItem.className = info.className;
                            subItem.subjectName = info.subjectName;
                            subItem.teacherName = info.teacherName;
                            subItem.roomName = info.roomName;
                            subItem.subjectType = info.subjectType;
                        }
                        itemDay.items[classId] = subItem;
                    }
                    result.schedules.Add(itemDay);
                }
            }
            return result;
        }

        public class GroupedDay
        {
            public int? day { get; set; }
            public int? index { get; set; }
            public Guid classId { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is GroupedDay other)
                {
                    return this.day == other.day && this.index == other.index && this.classId == other.classId;
                }
                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + day.GetHashCode();
                    hash = hash * 23 + index.GetHashCode();
                    hash = hash * 23 + classId.GetHashCode();
                    return hash;
                }
            }
        }

        public class GroupedDayInfo
        {
            public Guid id { get; set; }
            public Guid subjectId { get; set; }
            public Guid? roomId { get; set; }
            public Guid? teacherId { get; set; }
            public double? sTime { get; set; }
            public double? eTime { get; set; }
            public int? subjectType { get; set; }

            public string subjectName { get; set; }
            public string className { get; set; }
            public string teacherName { get; set; }
            public string roomName { get; set; }
        }

        public class TeacherWithShift
        {
            public Guid id { get; set; }
            public Guid? teacherId { get; set; }
            public double? sTime { get; set; }
            public double? eTime { get; set; }
        }

        #region Code cua a Hung
        ///// <summary>
        ///// Kiểm tra lịch có đạt điều kiện không
        ///// Không trùng lịch giáo viên
        ///// Không trùng lịch phòng - nếu có chọn phòng
        ///// Có ngày, giờ, giáo viên đầy đủ
        ///// Hàm kiểm tra này dùng để kiểm tra trước khi insert buổi học
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public override async Task Validate(tbl_TimeTable model)
        //{
        //    var checkTeacher = await isTeacherScheduleAvailable(model);
        //    if (checkTeacher != null)
        //        throw new AppException(MessageContants.exs_timetable_teacher);

        //    if (model.roomId.HasValue)
        //    {
        //        var checkClassRoom = await isClassRoomScheduleAvailable(model);
        //        if (checkClassRoom != null)
        //            throw new AppException(MessageContants.exs_timetable_classroom);
        //    }
        //}

        ///// <summary>
        ///// Điều kiện check trùng giáo viên
        ///// Không được trùng giờ bắt đầu và giờ kết thúc
        ///// Không được chồng chéo thời gian ví dụ: 15 => 16 vs 15:30 => 16:30
        ///// Không được bao bọc hoặc bị bao bọc thời gian, ví dụ: 15 => 16 vs 14 => 16:30
        ///// true là hợp lệ
        ///// false là trùng lịch, không được sắp lịch
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        ///// <exception cref="AppException"></exception>
        //public async Task<tbl_TimeTable> isTeacherScheduleAvailable(tbl_TimeTable model)
        //{
        //    var item = await this.unitOfWork.Repository<tbl_TimeTable>().GetQueryable()
        //            .FirstOrDefaultAsync(x => x.deleted == false && x.active == true
        //            && (x.startTime == model.startTime
        //            || x.endTime == model.endTime
        //            || (x.startTime <= model.startTime && model.startTime < x.endTime)
        //            || (x.startTime < model.endTime && model.endTime <= x.endTime)
        //            || (x.startTime < model.startTime && model.endTime < x.endTime)
        //            || (x.startTime > model.startTime && model.endTime > x.endTime)));

        //    return item;
        //}

        ///// <summary>
        ///// Điều kiện check trùng phòng học
        ///// Không được trùng giờ bắt đầu và giờ kết thúc
        ///// Không được chồng chéo thời gian ví dụ: 15 => 16 vs 15:30 => 16:30
        ///// Không được bao bọc hoặc bị bao bọc thời gian, ví dụ: 15 => 16 vs 14 => 16:30
        ///// true là hợp lệ
        ///// false là trùng lịch, không được sắp lịch
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        ///// <exception cref="AppException"></exception>
        //public async Task<tbl_TimeTable> isClassRoomScheduleAvailable(tbl_TimeTable model)
        //{
        //    var item = await this.unitOfWork.Repository<tbl_TimeTable>().GetQueryable()
        //            .FirstOrDefaultAsync(x => x.roomId == model.roomId && x.deleted == false && x.active == true
        //            && (x.startTime == model.startTime
        //            || x.endTime == model.endTime
        //            || (x.startTime <= model.startTime && model.startTime < x.endTime)
        //            || (x.startTime < model.endTime && model.endTime <= x.endTime)
        //            || (x.startTime < model.startTime && model.endTime < x.endTime)
        //            || (x.startTime > model.startTime && model.endTime > x.endTime)));

        //    return item;
        //}

        ///// <summary>
        ///// Kiểm tra lịch có đạt điều kiện không
        ///// Không trùng lịch giáo viên
        ///// Không trùng lịch phòng - nếu có chọn phòng
        ///// Có ngày, giờ, giáo viên đầy đủ
        ///// Hàm kiểm tra này dùng để kiểm tra trước khi update buổi học
        ///// dùng riêng để không 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public async Task ValidateForUpdate(tbl_TimeTable model)
        //{
        //    var checkTeacher = await isTeacherScheduleAvailableForUpdate(model);
        //    if (checkTeacher != null)
        //        throw new AppException(MessageContants.exs_timetable_teacher);

        //    if (model.roomId.HasValue)
        //    {
        //        var checkClassRoom = await isClassRoomScheduleAvailableForUpdate(model);
        //        if (checkClassRoom != null)
        //            throw new AppException(MessageContants.exs_timetable_classroom);
        //    }
        //}
        ///// <summary>
        ///// Điều kiện check trùng giáo viên
        ///// Không được trùng giờ bắt đầu và giờ kết thúc
        ///// Không được chồng chéo thời gian ví dụ: 15 => 16 vs 15:30 => 16:30
        ///// Không được bao bọc hoặc bị bao bọc thời gian, ví dụ: 15 => 16 vs 14 => 16:30
        ///// true là hợp lệ
        ///// false là trùng lịch, không được sắp lịch
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        ///// <exception cref="AppException"></exception>
        //public async Task<tbl_TimeTable> isTeacherScheduleAvailableForUpdate(tbl_TimeTable model)
        //{
        //    var item = await this.unitOfWork.Repository<tbl_TimeTable>().GetQueryable()
        //            .FirstOrDefaultAsync(x => x.deleted == false && x.active == true
        //            && (x.startTime == model.startTime
        //            || x.endTime == model.endTime
        //            || (x.startTime <= model.startTime && model.startTime < x.endTime)
        //            || (x.startTime < model.endTime && model.endTime <= x.endTime)
        //            || (x.startTime < model.startTime && model.endTime < x.endTime)
        //            || (x.startTime > model.startTime && model.endTime > x.endTime))
        //            && x.id != model.id);

        //    return item;
        //}

        ///// <summary>
        ///// Điều kiện check trùng phòng học
        ///// Không được trùng giờ bắt đầu và giờ kết thúc
        ///// Không được chồng chéo thời gian ví dụ: 15 => 16 vs 15:30 => 16:30
        ///// Không được bao bọc hoặc bị bao bọc thời gian, ví dụ: 15 => 16 vs 14 => 16:30
        ///// true là hợp lệ
        ///// false là trùng lịch, không được sắp lịch
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        ///// <exception cref="AppException"></exception>
        //public async Task<tbl_TimeTable> isClassRoomScheduleAvailableForUpdate(tbl_TimeTable model)
        //{
        //    var item = await this.unitOfWork.Repository<tbl_TimeTable>().GetQueryable()
        //            .FirstOrDefaultAsync(x => x.roomId == model.roomId && x.deleted == false && x.active == true
        //            && (x.startTime == model.startTime
        //            || x.endTime == model.endTime
        //            || (x.startTime <= model.startTime && model.startTime < x.endTime)
        //            || (x.startTime < model.endTime && model.endTime <= x.endTime)
        //            || (x.startTime < model.startTime && model.endTime < x.endTime)
        //            || (x.startTime > model.startTime && model.endTime > x.endTime))
        //            && x.id != model.id);

        //    return item;
        //}

        ///// <summary>
        ///// Hàm tạo thời khóa biểu lớp học
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public async Task<List<GenerateTimeTableDTO>> GenerateTimeTable(GenerateTimeTableCreate model)
        //{
        //    var data = new List<GenerateTimeTableDTO>();
        //    double unixTimestamp = model.startTime;

        //    var studyShifts = await this.unitOfWork.Repository<tbl_TimeTable>().GetQueryable().
        //        Where(x => x.deleted == false && x.active == true).ToListAsync();

        //    DateTime currentDate = Timestamp.ToDatetime((long)unixTimestamp).Date;
        //    while (unixTimestamp <= model.endTime)
        //    {
        //        //Check xem ngày này là thứ mấy để lấy danh sách buổi học mẫu ra
        //        var keyDayofCurrent = CoreContants.ConvertEnumDefaultToKeyDayofWeek((int)currentDate.DayOfWeek);
        //        var scheduleSample = model.schedules.Where(x => x.keyDayOfWeek == keyDayofCurrent).ToList();
        //        foreach (var s in scheduleSample)
        //        {
        //            var studyShift = studyShifts.Where(x => x.id == s.studyShiftId).First();
        //            var startTime = Timestamp.ToDatetime((long)studyShift.startTime);
        //            var endTime = Timestamp.ToDatetime((long)studyShift.endTime);
        //            var item = new tbl_TimeTable()
        //            {
        //                startTime = Timestamp.TimestampDateTime(currentDate.Add(startTime.TimeOfDay)),
        //                endTime = Timestamp.TimestampDateTime(currentDate.Add(endTime.TimeOfDay)),
        //                roomId = s.roomId
        //            };
        //            var jtem = mapper.Map<GenerateTimeTableDTO>(item);

        //            try
        //            {
        //                await Validate(item);
        //                jtem.isAvailable = true;
        //                data.Add(jtem);
        //            }
        //            catch(Exception e)
        //            {
        //                jtem.isAvailable = false;
        //                jtem.reason = e.Message;
        //                data.Add(jtem);
        //            }
        //        }
        //        currentDate.AddDays(1);
        //    }

        //    return data;
        //}

        #endregion
    }
}
