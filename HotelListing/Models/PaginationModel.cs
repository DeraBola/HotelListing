namespace HotelListing.Models
{
	public class PaginationModel<T>
	{
		public List<T> Items { get; set; }
		public int TotalItems { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
		public int PageSize { get; set; }

		public PaginationModel(List<T> items, int totalItems, int currentPage, int pageSize)
		{
			Items = items;
			TotalItems = totalItems;
			CurrentPage = currentPage;
			PageSize = pageSize;
		}
	}

}
