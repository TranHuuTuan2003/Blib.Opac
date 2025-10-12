using Microsoft.Extensions.Caching.Memory;

using KMS.Api.Helpers;
using KMS.Api.Infrastructure.DbContext.master;
using KMS.Api.Services.Search.Logic;

using UC.Core.Interfaces;

namespace KMS.Api.Services
{
    public class ServiceWrapper : IServiceWrapper
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUserProvider _userProvider;
        private readonly IConfiguration _configuration;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ApiHelper _apiHelper;
        private readonly IIntermediateSearchLogic _intermediateSearchLogic;
        private readonly ILogger<ServiceWrapper> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;

        public ServiceWrapper(
            UnitOfWork unitOfWork,
            IDateTimeProvider dateTimeProvider,
            IUserProvider userProvider,
            IConfiguration configuration,
            AppConfigHelper appConfigHelper,
            ApiHelper apiHelper,
            IIntermediateSearchLogic intermediateSearchLogic,
            IHttpContextAccessor contextAccessor,
            ILogger<ServiceWrapper> logger,
            IMemoryCache memoryCache
            )
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
            _userProvider = userProvider;
            _configuration = configuration;
            _appConfigHelper = appConfigHelper;
            _apiHelper = apiHelper;
            _intermediateSearchLogic = intermediateSearchLogic;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        private UCExample.IService _uc_sample = null;
        public UCExample.IService uc_sample => _uc_sample ?? (_uc_sample = new UCExample.Service(_unitOfWork, _dateTimeProvider, _userProvider, _appConfigHelper));

        private Search.IService _search_service = null;
        public Search.IService search_service => _search_service ?? (_search_service = new Search.Service(_unitOfWork, _appConfigHelper, _intermediateSearchLogic, _memoryCache, _logger));

        private Document.IService _document;
        public Document.IService document => _document ??= new Document.Service(_unitOfWork, _apiHelper, _appConfigHelper, _logger, _memoryCache);

        private Collection.IService _collection;
        public Collection.IService collection => _collection ??= new Collection.Service(_unitOfWork, _apiHelper, _appConfigHelper, _logger, _memoryCache,_intermediateSearchLogic);

    }
}
