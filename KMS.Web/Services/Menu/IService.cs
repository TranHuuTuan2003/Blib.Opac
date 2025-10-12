namespace KMS.Web.Services.Menu
{
    public interface IService
    {
        Task<List<Shared.DTOs.Menu.Menu>> GetMenuAsync();
    }
}