namespace KMS.Shared.DTOs.Document
{
    public class CollectionDto
    {
        public string CollectionId { get; set; }
        public string CollectionTitle { get; set; }
        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
    }
}