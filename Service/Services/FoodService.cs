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

namespace Service.Services
{
    public class FoodService : DomainService<tbl_Food, FoodSearch>, IFoodService
    {
        private readonly IFoodItemService foodItemService;
        public FoodService(IAppUnitOfWork unitOfWork, IMapper mapper, IFoodItemService foodItemService) : base(unitOfWork, mapper)
        {
            this.foodItemService = foodItemService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Food";
        }

        public override async Task Validate(tbl_Food model)
        {
            if (model.items != null && model.items.Count > 0)
            {
                var itemCount = await this.unitOfWork.Repository<tbl_Item>().GetQueryable()
                    .CountAsync(x => model.items.Select(d => d.itemId.Value).Contains(x.id) && x.deleted == false);

                if (itemCount != model.items.Count)
                    throw new AppException(MessageContants.nf_item);
            }
            else
            {
                throw new AppException(MessageContants.req_details);
            }
        }

        public override async Task AddItem(tbl_Food model)
        {
            await this.Validate(model);

            List<tbl_FoodItem> details = model.items.ToList();
            model.itemCount = details.Count;

            //add food
            await this.unitOfWork.Repository<tbl_Food>().CreateAsync(model);

            //re-calc nutrition
            var items = await this.unitOfWork.Repository<tbl_Item>().GetQueryable().ToListAsync();

            foreach (var detail in details)
            {
                detail.foodId = model.id;
                var item = items.FirstOrDefault(x => x.id == detail.itemId);
                if(item!= null)
                {
                    detail.essenceRate = item.essenceRate ?? 0;
                    detail.actualQty = Math.Round((double)(detail.qty ?? 0 - (item.essenceRate * detail.qty ?? 0 / 100)), 2);
                    detail.protein = Math.Round((double)(item.protein ?? 0 * detail.actualQty  * ((item.weightPerUnit ?? 1)/100)), 2);
                    detail.calo = Math.Round((double)(item.calo ?? 0 * detail.actualQty  * ((item.weightPerUnit ?? 1)/100)), 2);
                    detail.gluxit = Math.Round((double)(item.gluxit ?? 0 * detail.actualQty  * ((item.weightPerUnit ?? 1)/100)), 2);
                    detail.lipit = Math.Round((double)(item.lipit ?? 0 * detail.actualQty  * ((item.weightPerUnit ?? 1)/100)), 2);
                }
            }

            await this.unitOfWork.Repository<tbl_FoodItem>().CreateAsync(details);
            await this.unitOfWork.SaveAsync();
        }

        public override async Task UpdateItem(tbl_Food model)
        {
            await this.Validate(model);

            List<tbl_FoodItem> details = model.items.ToList();
            //re-calc nutrition
            var items = await this.unitOfWork.Repository<tbl_Item>().GetQueryable().ToListAsync();
            foreach (var detail in details)
            {
                detail.foodId = model.id;
                var item = items.FirstOrDefault(x => x.id == detail.itemId);
                if (item != null)
                {
                    detail.essenceRate = item.essenceRate ?? 0;
                    detail.actualQty = Math.Round((double)(detail.qty ?? 0 - (item.essenceRate * detail.qty ?? 0 / 100)), 2);
                    detail.calo = Math.Round((double)(item.calo ?? 0 * detail.actualQty * ((item.weightPerUnit ?? 1)/100)), 2);
                    detail.gluxit = Math.Round((double)(item.gluxit ?? 0 * detail.actualQty  * ((item.weightPerUnit ?? 1)/100)), 2);
                    detail.lipit = Math.Round((double)(item.lipit ?? 0 * detail.actualQty  * ((item.weightPerUnit ?? 1)/100)), 2);
                    detail.protein = Math.Round((double)(item.protein ?? 0 * detail.actualQty  * ((item.weightPerUnit ?? 1)/100)), 2);
                }
            }

            model.itemCount = details.Count;

            //update food
            await this.UpdateAsync(model);

            //update food-item
            var currentItems = await this.unitOfWork.Repository<tbl_FoodItem>().GetQueryable().Where(x => x.deleted == false && x.foodId== model.id).ToListAsync();
            //get deleted Ids to delete
            var deletedItems = currentItems.Where(x => !details.Select(d => d.id).Contains(x.id)).ToList();
            deletedItems.ForEach(x => x.deleted = true);

            //new items
            var newItems = details.Where(x => !currentItems.Select(d => d.id).Contains(x.id)).ToList();

            //update items
            var updatedItems = details.Where(x => currentItems.Select(d => d.id).Contains(x.id)).ToList();

            //submit value
            await this.unitOfWork.Repository<tbl_FoodItem>().CreateAsync(newItems);
            await this.foodItemService.UpdateAsync(updatedItems);
            await this.foodItemService.UpdateAsync(deletedItems);

            await this.unitOfWork.SaveAsync();
        }

        public override async Task<tbl_Food> GetByIdAsync(Guid id)
        {
            //base baseinformation
            var food = await this.unitOfWork.Repository<tbl_Food>().Validate(id) ?? throw new AppException(MessageContants.nf_food);

            //mapping item
            food.items = await this.unitOfWork.Repository<tbl_FoodItem>().GetDataExport("Get_FoodItem", new SqlParameter[] { new SqlParameter("foodId", food.id) });

            return food;
        }
    }
}
