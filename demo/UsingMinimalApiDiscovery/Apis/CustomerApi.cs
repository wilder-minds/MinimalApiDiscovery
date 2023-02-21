using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using UsingMinimalApiDiscovery.Data;
using WilderMinds.MinimalApiDiscovery;

namespace UsingMinimalApiDiscovery.Apis;

public class CustomerApi : IApi
{
  public void Register(WebApplication app)
  {
    var grp = app.MapGroup("/api/customers");
    grp.MapGet("", GetCustomers);
    grp.MapGet("", GetCustomer);
    grp.MapPost("{id:int}", SaveCustomer);
    grp.MapPut("{id:int}", UpdateCustomer);
    grp.MapDelete("{id:int}", DeleteCustomer);
  }

  static async Task<IResult> GetCustomers(CustomerRepository repo)
  {
    return Results.Ok(await repo.GetCustomers());
  }

  static async Task<IResult> GetCustomer(CustomerRepository repo, int id)
  {
    return Results.Ok(await repo.GetCustomer(id));
  }

  static async Task<IResult> SaveCustomer(CustomerRepository repo, Customer model)
  {
    return Results.Created($"/api/customer/{model.Id}", await repo.SaveCustomer(model));
  }

  static private async Task<IResult> UpdateCustomer(CustomerRepository repo, Customer model)
  {
    return Results.Ok(await repo.UpdateCustomer(model));
  }

  static async Task<IResult> DeleteCustomer(CustomerRepository repo, int id)
  {
    var result = await repo.DeleteCustomer(id);
    if (result) return Results.Ok();
    return Results.BadRequest();
  }
}