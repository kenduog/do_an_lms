using Interface.Repository;
using Interface.Services.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Service.Repository;
using Interface.UnitOfWork;
using Service;
using Interface.Services;
using Service.Services;
using Interface.DbContext;
using Service.Services.Auth;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.CodeAnalysis;
using Extensions;
using System.Net;
using Service.Auth;
using Microsoft.AspNetCore.Http;
using Interface.Services.Background;
using Service.Services.Background;

namespace BaseAPI
{
    public static class ServiceExtensions
    {
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IAppDbContext, AppDbContext.AppDbContext>();
            services.AddScoped(typeof(IDomainRepository<>), typeof(DomainRepository<>));
            services.AddScoped(typeof(IAppRepository<>), typeof(AppRepository<>));
            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
        }

        public static void ConfigureService(this IServiceCollection services)
        {
            services.AddLocalization(o => { o.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                CultureInfo[] supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("he")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddTransient<ITokenManagerService, TokenManagerService>();

            services.AddScoped<ITuitionConfigCategoryService, TuitionConfigCategoryService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IReportTemplateService, ReportTemplateService>();
            services.AddScoped<IStudySessionService, StudySessionService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<IPaymentBankService, PaymentBankService>();
            services.AddScoped<ICollectionSessionHeaderService, CollectionSessionHeaderService>();
            services.AddScoped<ICollectionSessionService, CollectionSessionService>();
            services.AddScoped<ICollectionPlanDetailService, CollectionPlanDetailService>();
            services.AddScoped<ICollectionPlanService, CollectionPlanService>();
            services.AddScoped<IFeeReductionService, FeeReductionService>();
            services.AddScoped<IFeeReductionConfigService, FeeReductionConfigService>();
            services.AddScoped<IFeeInGradeService, FeeInGradeService>();
            services.AddScoped<IFeeService, FeeService>();
            services.AddScoped<IPurchaseOrderLineService, PurchaseOrderLineService>();
            services.AddScoped<IPurchaseOrderHeaderService, PurchaseOrderHeaderService>();
            services.AddScoped<INutritionGroupService, NutritionGroupService>();
            services.AddScoped<IMenuWeekService, MenuWeekService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddScoped<IMenuFoodService, MenuFoodService>();
            services.AddScoped<IFoodItemService, FoodItemService>();
            services.AddScoped<IFoodService, FoodService>();
            services.AddScoped<IValueEntryService, ValueEntryService>();
            services.AddScoped<IItemInventoryService, ItemInventoryService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<IDeliveryOrderHeaderService, DeliveryOrderHeaderService>();
            services.AddScoped<IDeliveryOrderLineService, DeliveryOrderLineService>();
            services.AddScoped<IReceiveOrderLineService, ReceiveOrderLineService>();
            services.AddScoped<IReceiveOrderHeaderService, ReceiveOrderHeaderService>();
            services.AddScoped<IFeedbackGroupService, FeedbackGroupService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IFeedbackReplyService, FeedbackReplyService>();
            services.AddScoped<IFeedbackPermissionService, FeedbackPermissionService>();
            services.AddScoped<IItemOfSKUService, ItemOfSKUService>();
            services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
            services.AddScoped<IItemGroupService, ItemGroupService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IClassShiftService, ClassShiftService>();
            services.AddScoped<ITimeTableDetailService, TimeTableDetailService>();
            services.AddScoped<ISubjectInGradeService, SubjectInGradeService>();
            services.AddScoped<ISubjectGroupService, SubjectGroupService>();
            services.AddScoped<IStudentLeaveRequestService, StudentLeaveRequestService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IPaymentSessionService, PaymentSessionService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IBillDetailService, BillDetailService>();
            services.AddScoped<IBillService, BillService>();
            services.AddScoped<IArrangeClassService, ArrangeClassService>();
            services.AddScoped<IStudyShiftService, StudyShiftService>();
            services.AddScoped<IParentService, ParentService>();
            services.AddScoped<ITuitionConfigService, TuitionConfigService>();
            services.AddScoped<IExcelExportService, ExcelExportService>();
            services.AddScoped<IHighestLevelOfEducationService, HighestLevelOfEducationService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<ITeachingFrameService, TeachingFrameService>();
            services.AddScoped<IDropdownService, DropdownService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IAutoGenCodeConfigService, AutoGenCodeConfigService>();
            services.AddScoped<IClassInTimeTableService, ClassInTimeTableService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ITimeTableService, TimeTableService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IUserPermissionSerivce, UserPermissionService>();
            services.AddScoped<IGroupPermissionService, GroupPermissionService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IMultipleMessageService, MultipleMessageService>();
            services.AddScoped<IContentTypeService, ContentTypeService>();
            services.AddScoped<IStudentInClassService, StudentInClassService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IProfileTemplateService, ProfileTemplateService>();
            services.AddScoped<IHolidayService, HolidayService>();
            services.AddScoped<IDayOfWeekService, DayOfWeekService>();
            services.AddScoped<ISchoolYearService, SchoolYearService>();
            services.AddScoped<ISemesterService, SemesterService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<ICriteriaService, CriteriaService>();
            services.AddScoped<ICriteriaDetailService, CriteriaDetailService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<INecessaryService, NecessaryService>();
            services.AddScoped<IWardsService, WardsService>();
            services.AddScoped<IDistrictsService, DistrictsService>();
            services.AddScoped<ICitiesService, CitiesService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDomainHub, DomainHub>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<ILikeInNewsService, LikeInNewsService>();
            services.AddScoped<ICommentInNewsService, CommentInNewsService>();
            services.AddScoped<ITypeDocumentNewsService, TypeDocumentNewsService>();
            services.AddScoped<IGroupNewsService, GroupNewsService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IUserJoinGroupNewsService, UserJoinGroupNewsService>();
            services.AddScoped<IDocumentNewsService, DocumentNewsService>();
            services.AddScoped<ICommentDefaultService, CommentDefaultService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ITeachingAssignmentService, TeachingAssignmentService>();
            services.AddScoped<IWeekService, WeekService>();
            services.AddScoped<IGoodBehaviorCertificateService, GoodBehaviorCertificateService>();
            services.AddScoped<IChildAssessmentTopicService, ChildAssessmentTopicService>();
            services.AddScoped<IChildAssessmentDetailService, ChildAssessmentDetailService>();
            services.AddScoped<IStudentInAssessmentService, StudentInAssessmentService>();
            services.AddScoped<IScaleMeasureService, ScaleMeasureService>();
            services.AddScoped<IScaleMeasureDetailService, ScaleMeasureDetailService>();
            services.AddScoped<ISendNotificationService, SendNotificationService>();
            services.AddScoped<INotificationConfigService, NotificationConfigService>();
            services.AddScoped<IDateTimeConfigService, DateTimeConfigService>();
            services.AddScoped<ITranscriptService, TranscriptService>();
            services.AddScoped<IStatisticalService, StatisticalService>();

            //background service
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<RuntimeBackgroundService>();

        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "NextGen API V1", Version = "v1.0" });
                c.SwaggerDoc("v2", new OpenApiInfo() { Title = "NextGen API V2", Version = "v2.0" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
                var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
                foreach (var fi in dir.EnumerateFiles("*.xml"))
                {
                    c.IncludeXmlComments(fi.FullName);
                }
                c.EnableAnnotations();
            });
        }

    }
}
