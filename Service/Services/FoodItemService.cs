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
    public class FoodItemService : DomainService<tbl_FoodItem, FoodItemSearch>, IFoodItemService
    {
        public FoodItemService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_FoodItem";
        }

        public async Task<List<tbl_FoodItem>> GetFoodItemMenu(FoodItemMenuRequest request)
        {
            if (string.IsNullOrEmpty(request.foodIds))
                return new List<tbl_FoodItem>();

            var foodDict = request.foodIds.Split(",").GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            foodDict = foodDict.Where(x=>x.Value > 1).ToDictionary(x=>x.Key, x => x.Value);

            var foodItems = await this.unitOfWork.Repository<tbl_FoodItem>().GetDataExport("Get_FoodItemMenu", GetSqlParameters(request));

            foreach(var food in foodDict)
            {
                var items = foodItems.Where(x=>x.foodId.ToString() == food.Key).ToList();
                if(items != null && items.Count > 0)
                {
                    items.ForEach(items => 
                    { 
                        items.qty *= food.Value; 
                        items.calo *= food.Value; 
                        items.protein *= food.Value; 
                        items.lipit *= food.Value; 
                        items.gluxit *= food.Value; 
                    });
                }
            }
            foodItems = foodItems.GroupBy(x => new { x.itemId, x.itemName, x.unitOfMeasureName }).Select(x => new tbl_FoodItem
            {
                itemId = x.Key.itemId,
                itemName = x.Key.itemName,
                unitOfMeasureName = x.Key.unitOfMeasureName,
                qty = x.Sum(d => d.qty),
                calo = x.Sum(d => d.calo),
                protein = x.Sum(d => d.protein),
                lipit = x.Sum(d => d.lipit),
                gluxit = x.Sum(d => d.gluxit),
            }).ToList();
            return foodItems;
        }
    }
}
