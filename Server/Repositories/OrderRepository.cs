using System.Collections.Generic;
using System.Linq;
using BlazorSPA.Shared;
using BlazorSPA.Server.Data;

namespace BlazorSPA.Server.Repositories
{
	public interface IOrderRepository
	{
		void Save(Order order);
		List<Order> GetAll(string description);
		Order GetById(int orderId);
		void Delete(int orderId);
	}

	public class OrderRepository : IOrderRepository
	{
		private readonly IBlazorSPAContext db;

		public OrderRepository(IBlazorSPAContext blazorSPAContext)
		{
			this.db = blazorSPAContext ?? throw new System.ArgumentNullException(nameof(blazorSPAContext));
		}

		public void Save(Order order)
		{
			if (order.Id < 1)
			{
				db.Orders.Add(order);
			}
			else
			{
				db.Orders.Update(order);
			}
			db.SaveChanges();
		}

		public List<Order> GetAll(string description)
		{
			return (
				from o in db.Orders
				where o.Description.Contains(description) || string.IsNullOrEmpty(description)
				orderby o.Id
				select o
			).ToList();
		}

		public Order GetById(int orderId)
		{
			return (
				from o in db.Orders
				where o.Id == orderId
				select o
			).SingleOrDefault();
		}

		public void Delete(int orderId)
		{
			Order order = (
				from o in db.Orders
				where o.Id == orderId
				select o
			).SingleOrDefault();
			if (order != null)
			{
				db.Orders.Remove(order);
				db.SaveChanges();
			}
		}

	}
}
