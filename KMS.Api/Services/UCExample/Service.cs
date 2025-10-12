using System.Text;

using KMS.Api.Common;
using KMS.Api.Dtos;
using KMS.Api.Entities;
using KMS.Api.Helpers;
using KMS.Api.Infrastructure.DbContext.master;
using KMS.Shared.Helpers;

using UC.Core.Helpers;
using UC.Core.Interfaces;

namespace KMS.Api.Services.UCExample
{
	public class Service : Repository<string, uc_sample>, IService
	{
		private readonly UnitOfWork _unitOfWork;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IUserProvider _userProvider;
		private readonly AppConfigHelper _appConfigHelper;

		public Service(UnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider, IUserProvider userProvider, AppConfigHelper appConfigHelper) : base(unitOfWork, dateTimeProvider, userProvider, appConfigHelper)
		{
			_unitOfWork = unitOfWork;
			_dateTimeProvider = dateTimeProvider;
			_userProvider = userProvider;
			_appConfigHelper = appConfigHelper;
		}

		public async Task<UcExample> InsertItemAsync(UcExample example)
		{
			example.id = StringHelpers.TimestampId();
			var exampleEntity = new uc_sample();
			ModelMapper.MapProperties(example, exampleEntity);
			var insertSql = GenerateSqlQuery.GenerateInsertQuery<UcExample>("uc_example");
			return await TransactionHelper.ExecuteAsync(_unitOfWork, async () =>
			{
				await _unitOfWork.Repository.ExecuteAsync(insertSql, exampleEntity);
				return example;
			});
		}

		public async Task<UcExample> UpdateItemAsync(UcExample example)
		{
			var exampleEntity = new uc_sample();
			ModelMapper.MapProperties(example, exampleEntity);
			var updateSql = GenerateSqlQuery.GenerateUpdateQuery<UcExample>("uc_example");
			return await TransactionHelper.ExecuteAsync(_unitOfWork, async () =>
			{
				await _unitOfWork.Repository.ExecuteAsync(updateSql, exampleEntity);
				return example;
			});
		}

		public async Task<List<UcExample>> GetItemsAsync()
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT * FROM uc_example");
			return await _unitOfWork.Repository.QueryListAsync<UcExample>(sql.ToString(), new { });
		}

		public async Task<UcExample?> GetItemByIdAsync(string id)
		{
			var sql = new StringBuilder();
			sql.AppendLine("SELECT * FROM uc_example");
			sql.AppendLine("WHERE id = @id");
			return await _unitOfWork.Repository.QueryFirstAsync<UcExample>(sql.ToString(), new { id });
		}

		public async Task DeleteItemAsync(string id)
		{
			var sql = new StringBuilder();
			sql.AppendLine("DELETE FROM uc_example");
			sql.AppendLine("WHERE id = @id");
			await TransactionHelper.ExecuteAsync(_unitOfWork, async () =>
			{
				await _unitOfWork.Repository.ExecuteAsync(sql.ToString(), new { id });
			});
		}
	}
}
