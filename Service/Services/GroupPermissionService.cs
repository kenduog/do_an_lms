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
using Entities.DomainEntities;
using Entities.AuthEntities;
using Request.DomainRequests;

namespace Service.Services
{
    public class GroupPermissionService : DomainService<tbl_GroupPermission, BaseSearch>, IGroupPermissionService
    {
        public GroupPermissionService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task AddContentTypeParentToPermission(tbl_GroupPermission model)
        {
            var permission = await this.unitOfWork.Repository<tbl_Permission>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.permissionId) ?? throw new AppException(MessageContants.nf_permission);

            //lấy content parent 
            var contentTypeParent = await this.unitOfWork.Repository<tbl_ContentType>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.id == permission.contentTypeId);

            //nếu menu paarent không phải root thì đệ quy để thêm menu root vào permission
            if (!contentTypeParent.isRoot.HasValue || !contentTypeParent.isRoot.Value)
            {
                tbl_Permission rootPermission = await this.unitOfWork.Repository<tbl_Permission>().GetQueryable().FirstOrDefaultAsync(x => x.contentTypeId == contentTypeParent.parentId) ?? throw new AppException(MessageContants.nf_permission);
                tbl_GroupPermission newGP = new tbl_GroupPermission { groupId = model.groupId, permissionId = rootPermission.id };
                await this.unitOfWork.Repository<tbl_GroupPermission>().CreateAsync(newGP);

                await this.AddContentTypeParentToPermission(newGP);
            }
        }

        public override async Task AddItem(tbl_GroupPermission model)
        {
            await this.unitOfWork.Repository<tbl_GroupPermission>().CreateAsync(model);
            await this.AddContentTypeParentToPermission(model);
            await this.unitOfWork.SaveAsync();
        }

        public async Task GrantAllPermissionToGroup(DomainUpdate item)
        {
            var group = await this.unitOfWork.Repository<tbl_Group>().GetQueryable().FirstOrDefaultAsync(x => x.id == item.id) ?? throw new AppException(MessageContants.nf_group);
            
            //xóa hết quyền hiện tại
            var groupPermissions = await this.unitOfWork.Repository<tbl_GroupPermission>().GetQueryable().Where(x => x.deleted == false && x.groupId == group.id).ToListAsync();
            this.unitOfWork.Repository<tbl_GroupPermission>().Delete(groupPermissions);

            //lấy bộ quyền mới để  thêm
            var permissions = await this.unitOfWork.Repository<tbl_Permission>().GetQueryable().Where(x => x.deleted == false && x.active == true && x.code != "MenuConfig").ToListAsync();
            var grpPermissions = permissions.Select(x => new tbl_GroupPermission { groupId = group.id, permissionId = x.id }).ToList();
            await this.unitOfWork.Repository<tbl_GroupPermission>().CreateAsync(grpPermissions);

            await this.unitOfWork.SaveAsync();
        }

        public async Task RemoveAllPermissionToGroup(DomainUpdate item)
        {
            var group = await this.unitOfWork.Repository<tbl_Group>().GetQueryable().FirstOrDefaultAsync(x => x.id == item.id) ?? throw new AppException(MessageContants.nf_group);

            var groupPermissions = await this.unitOfWork.Repository<tbl_GroupPermission>().GetQueryable().Where(x => x.deleted == false && x.groupId == group.id).ToListAsync();
                
            this.unitOfWork.Repository<tbl_GroupPermission>().Delete(groupPermissions);
            await this.unitOfWork.SaveAsync();
        }
    }
}
