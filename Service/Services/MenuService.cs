using Entities;
using Extensions;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Service.Services.DomainServices;
using Entities.Search;
using AppDbContext.Migrations;

namespace Service.Services
{
    public class MenuService : DomainService<tbl_Menu, MenuSearch>, IMenuService
    {
        private readonly IFoodItemService foodItemService;
        public MenuService(IAppUnitOfWork unitOfWork, IMapper mapper, IFoodItemService foodItemService) : base(unitOfWork, mapper)
        {
            this.foodItemService = foodItemService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_MenuDayBoard";
        }

        public override async Task Validate(tbl_Menu model)
        {
            if (model.gradeId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Grade>().Validate(model.gradeId.Value) ?? throw new AppException(MessageContants.nf_grade);
            }
            if (model.nutritionGroupId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_NutritionGroup>().Validate(model.nutritionGroupId.Value) ?? throw new AppException(MessageContants.nf_nutritionGroup);
            }

            //validate food id
            if (model.foods != null && model.foods.Count > 0)
            {

                var foodIds = model.foods.Select(food => food.foodId).Distinct().ToList();
                var foodCount = await this.unitOfWork.Repository<tbl_Food>().GetQueryable()
                    .CountAsync(x => foodIds.Contains(x.id) && x.deleted == false);

                if (foodCount != foodIds.Count)
                    throw new AppException(MessageContants.nf_food);
            }
            else
            {
                throw new AppException(MessageContants.req_foods);
            }
            //validate item id
            if (model.items != null && model.items.Count > 0)
            {

                var itemIds = model.items.Select(food => food.itemId).Distinct().ToList();
                var foodCount = await this.unitOfWork.Repository<tbl_Item>().GetQueryable()
                    .CountAsync(x => itemIds.Contains(x.id) && x.deleted == false);

                if (foodCount != itemIds.Count)
                    throw new AppException(MessageContants.nf_item);
            }
            else
            {
                throw new AppException(MessageContants.req_details);
            }
        }

        public override async Task AddItem(tbl_Menu model)
        {
            await this.Validate(model);

            //add menu
            await this.unitOfWork.Repository<tbl_Menu>().CreateAsync(model);

            //add menu items 
            List<tbl_MenuItem> items = model.items.ToList();
            items.ForEach(x => { x.menuId = model.id; x.amt = x.qty * x.price; });

            //tính lại các thông số dinh dưỡng
            model = await NutritionCalc(model);

            await this.unitOfWork.Repository<tbl_MenuItem>().CreateAsync(items);

            //add menu food
            List<tbl_MenuFood> foods = model.foods.ToList();
            foods.ForEach(x => x.menuId = model.id);

            await this.unitOfWork.Repository<tbl_MenuFood>().CreateAsync(foods);
            await this.unitOfWork.SaveAsync();
        }

        //function nay code hoi lo nhung van chay duoc, khi nao ranh se sua lai
        public override async Task UpdateItem(tbl_Menu model)
        {
            await this.Validate(model);

            //tính lại các thông số dinh dưỡng
            model = await NutritionCalc(model);

            //update menu 
            await this.UpdateAsync(model);

            //update menu food
            List<tbl_MenuItem> items = model.items.ToList();
            items.ForEach(x => { x.menuId = model.id; x.amt = x.qty * x.price; });

            var currentItems = await this.unitOfWork.Repository<tbl_MenuItem>().GetQueryable().Where(x => x.deleted == false && x.menuId == model.id).ToListAsync();

            //get deleted Ids to delete
            var deletedItems = currentItems.Where(x => !items.Select(d => d.id).Contains(x.id)).ToList();

            //new items
            var newItems = items.Where(x => !currentItems.Select(d => d.id).Contains(x.id)).ToList();

            //update items
            var updatedItems = items.Where(x => currentItems.Select(d => d.id).Contains(x.id)).ToList();

            //submit value
            await this.unitOfWork.Repository<tbl_MenuItem>().CreateAsync(newItems);
            this.unitOfWork.Repository<tbl_MenuItem>().UpdateRange(updatedItems);
            this.unitOfWork.Repository<tbl_MenuItem>().Delete(deletedItems);

            List<tbl_MenuFood> foods = model.foods.ToList();
            foods.ForEach(x => x.menuId = model.id);
            var currentFoods = await this.unitOfWork.Repository<tbl_MenuFood>().GetQueryable().Where(x => x.deleted == false && x.menuId == model.id).ToListAsync();
            //get deleted Ids to delete
            var deletedFoods = currentFoods.Where(x => !foods.Select(d => d.id).Contains(x.id)).ToList();

            //new items
            var newFoods = foods.Where(x => !currentFoods.Select(d => d.id).Contains(x.id)).ToList();

            //update items
            var updatedFoods = foods.Where(x => currentFoods.Select(d => d.id).Contains(x.id)).ToList();

            //submit value
            await this.unitOfWork.Repository<tbl_MenuFood>().CreateAsync(newFoods);
            this.unitOfWork.Repository<tbl_MenuFood>().UpdateRange(updatedFoods);
            this.unitOfWork.Repository<tbl_MenuFood>().Delete(deletedFoods);


            await this.unitOfWork.SaveAsync();
        }

        private async Task<tbl_Menu> NutritionCalc(tbl_Menu item)
        {
            //Lấy lượng calo tổng (từ danh sách chi tiết trả ra, sum cột calo)
            var foodIds = item.foods.Select(x => x.foodId);
            FoodItemMenuRequest request = new FoodItemMenuRequest { foodIds = string.Join(",", foodIds) };
            var foodItems = await this.foodItemService.GetFoodItemMenu(request);

            //Tính số calo, lipit, gluxit, protein trên mỗi bé (Khối lượng món ăn trong thực đơn luôn mặc định luôn là 10 bé)
            double calo = foodItems.Sum(x => x.calo ?? 0) / 10; //chia 10 tre
            double protein = foodItems.Sum(x => x.protein ?? 0) / 10;
            double lipit = foodItems.Sum(x => x.lipit ?? 0) / 10;
            double gluxit = foodItems.Sum(x => x.gluxit ?? 0) / 10;

            //Tính tỷ lệ tại đây
            item.proteinPercent = (protein / 10) * 400 / calo;
            item.lipitPercent = (lipit / 10) * 900 / calo;
            item.gluxitPercent = 100 - item.proteinPercent - item.lipitPercent;
            if (item.gluxitPercent < 0)
                item.gluxitPercent = 0;
            item.calo = calo;
            item.protein = protein;
            item.lipit = lipit;
            item.gluxit = gluxit;

            return item;
        }

        public override async Task<tbl_Menu> GetByIdAsync(Guid id)
        {
            //thông tin cơ bản
            tbl_Menu menu = await this.unitOfWork.Repository<tbl_Menu>().Validate(id) ?? throw new AppException(MessageContants.nf_menu);

            //map thông tin món ăn
            menu.foods = await this.unitOfWork.Repository<tbl_MenuFood>()
                .GetDataExport("Get_All_MenuFood", new Microsoft.Data.SqlClient.SqlParameter[] { new Microsoft.Data.SqlClient.SqlParameter("menuId", menu.id) });

            //danh sách item 
            menu.items = await this.unitOfWork.Repository<tbl_MenuItem>()
                .GetDataExport("Get_All_MenuItem", new Microsoft.Data.SqlClient.SqlParameter[] { new Microsoft.Data.SqlClient.SqlParameter("menuId", menu.id) });

            menu.gradeName = this.unitOfWork.Repository<tbl_Grade>().Validate(menu.gradeId ?? Guid.Empty)?.Result?.name;
            return menu;
        }

        public async Task<PagedList<tbl_Menu>> GetMenuWithFoods (MenuSearch search)
        {
            var data = await this.GetPagedListData(search);

            if (data.items.Any())
            {
                //mapp foods to each item
                var menuIds = data.items.Select(x => x.id).ToList();

                var menuFood = await this.unitOfWork.Repository<tbl_MenuFood>().GetQueryable()
                    .Where(x => menuIds.Contains((Guid)x.menuId) && x.deleted == false && x.foodId.HasValue)      //database call
                    .ToListAsync();

                var foodIds = menuFood.Select(x => x.foodId.Value).ToList();

                var foods = await this.unitOfWork.Repository<tbl_Food>().GetQueryable().Where(x => foodIds.Contains(x.id)).ToListAsync();  //database call

                var menuDict = menuIds.ToDictionary(menuId => menuId, menuId => foods
                                                        .Where(food => menuFood.Where(week => week.menuId == menuId).Select(x => x.foodId).Contains(food.id))
                                                        .Select(x => new FoodItem 
                                                        { 
                                                            id = x.id, 
                                                            name = x.name,
                                                            /*mapping more field type  = menuFood.type*/
                                                            type = menuFood.Where(week => week.menuId == menuId && week.foodId == x.id)
                                                                    .Select(week => week.type)
                                                                    .FirstOrDefault()
                                                        })
                                                        .ToList());

                //mapping 
                foreach (var item in data.items)
                {
                    if (menuDict.TryGetValue(item.id, out var foodOut))
                    {
                        item.foodItems = foodOut;
                    }
                }
            }
            return data;
        }
    }
}
