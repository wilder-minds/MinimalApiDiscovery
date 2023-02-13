using Bogus;

namespace UsingMinimalApiDiscovery.Data;

public class CustomerRepository
{
  private Faker<Customer> _faker;

  public CustomerRepository()
	{
		_faker = new Faker<Customer>()
			.UseSeed(8675309)
			.RuleFor(c => c.Id, f => f.IndexFaker)
			.RuleFor(c => c.Name, f => f.Name.FullName())
			.RuleFor(c => c.Birthdate, (f, c) => f.Date.BetweenDateOnly(DateOnly.Parse("1950/01/01"), DateOnly.FromDateTime(DateTime.Today)));

	}

	public Task<List<Customer>> GetCustomers() 
		=> Task.FromResult(_faker.Generate(20));

  public Task<Customer> GetCustomer(int id) 
		=> Task.FromResult(_faker.Generate(20).Where(c => c.Id == id).First());

  public Task<Customer> SaveCustomer(Customer customer)
    => Task.FromResult(customer);

  public Task<Customer> UpdateCustomer(Customer customer)
    => Task.FromResult(customer);

  public Task<bool> DeleteCustomer(int id)
    => Task.FromResult(true);

}