using BlazorSPA.Server.Repositories;
using BlazorSPA.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BlazorSPA.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IOrderRepository orderRepository;

		public OrderController(IOrderRepository orderRepository)
		{
			this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
		}

		[HttpGet("")]
		[ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
		public ActionResult<List<Order>> GetAll(string query)
		{
			return orderRepository.GetAll(query);
		}

		[HttpGet("{id}", Name = nameof(GetById))]
		[ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<Order> GetById(int id)
		{
			Order order = orderRepository.GetById(id);
			if (order != null)
			{
				return order;
			}
			else
			{
				return NotFound();
			}
		}

		[HttpPost("")]
		[ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Order> Create([FromBody] Order order)
		{
			order.Id = 0; // Just in case
			orderRepository.Save(order);
			return CreatedAtRoute(nameof(GetById), new {id = order.Id}, order);
		}

		[HttpPut("{id}")]
		[ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Order> Update(int id, [FromBody] Order order)
		{
			if (id < 1)
			{
				ModelState.AddModelError("Id", "Invalid id");
				return BadRequest(ModelState);
			}
			order.Id = id;
			orderRepository.Save(order);
			return order;
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult Delete(int id)
		{
			orderRepository.Delete(id);
			return Ok();
		}

	}
}
