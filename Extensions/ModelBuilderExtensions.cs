using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using static Utilities.CoreContants;
using Utilities;
using Entities.AuthEntities;

namespace Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var userId = Guid.NewGuid();
            modelBuilder.Entity<tbl_Users>().HasData(
                    new tbl_Users()
                    {
                        id = userId,
                        username = "admin",
                        fullName = "Mona Media",
                        phone = "1900 636 648",
                        email = "info@mona-media.com",
                        address = "373/226 Lý Thường Kiệt, P8, Q. Tân Bình, HCM",
                        status = ((int)UserStatus.Active),
                        statusName = GetUserStatusName(((int)UserStatus.Active)),
                        birthday = 0,
                        password = SecurityUtilities.HashSHA1("lmsteam@"),
                        gender = 0,
                        isSuperUser = true,
                        code = $"{GetGroupaAcronym(Group.Admin.ToString())}001",
                        created = Timestamp.Now(),
                        updated = Timestamp.Now(),
                        createdBy = userId,
                        updatedBy = userId,
                        deleted = false,
                        active = true
                    }
                );
            modelBuilder.Entity<tbl_Group>().HasData(
                    new tbl_Group() { id = Guid.NewGuid(), name = GetGroupName(Group.Admin.ToString()), code = Group.Admin.ToString(),created = Timestamp.Now(), updated = Timestamp.Now(), createdBy = userId, updatedBy = userId, deleted = false, active = true },
                    new tbl_Group() { id = Guid.NewGuid(), name = GetGroupName(Group.Teacher.ToString()), code = Group.Teacher.ToString(),created = Timestamp.Now(), updated = Timestamp.Now(), createdBy = userId, updatedBy = userId, deleted = false, active = true },
                    new tbl_Group() { id = Guid.NewGuid(), name = GetGroupName(Group.Parents.ToString()), code = Group.Parents.ToString(), created = Timestamp.Now(), updated = Timestamp.Now(), createdBy = userId, updatedBy = userId, deleted = false, active = true },
                    new tbl_Group() { id = Guid.NewGuid(), name = GetGroupName(Group.Manager.ToString()), code = Group.Manager.ToString(), created = Timestamp.Now(), updated = Timestamp.Now(), createdBy = userId, updatedBy = userId, deleted = false, active = true }
                );
        }

        public static int lastDayOfMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }
    }
}
