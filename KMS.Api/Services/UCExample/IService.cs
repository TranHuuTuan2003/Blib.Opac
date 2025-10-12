using UC.Core.Interfaces;

namespace KMS.Api.Services.UCExample
{
	public interface IService : IRepositoryBase<string, Entities.uc_sample>
	{
		Task<Dtos.UcExample> InsertItemAsync(Dtos.UcExample example);
		Task<Dtos.UcExample> UpdateItemAsync(Dtos.UcExample example);
		Task<List<Dtos.UcExample>> GetItemsAsync();
		Task<Dtos.UcExample?> GetItemByIdAsync(string id);
		Task DeleteItemAsync(string id);
	}
}
