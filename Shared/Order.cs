using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorSPA.Shared
{
	public class Order
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(30)]
		public string Description { get; set; }

		[Range(1,1000)]
		public int Quantity { get; set; }
	}
}
