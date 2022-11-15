using ManageEmployee.Entities;
using ManageEmployee.Enum;
using ManageEmployee.Helpers;
using System.Linq.Expressions;

namespace ManageEmployee.Services
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetAll();
        IEnumerable<Customer> GetAll(int currentPage, int pageSize,string keyword);
        Customer GetById(int id);
        Task<string> Create(Customer param);
        Task<string> Update(Customer param);
        string Delete(int id);
    }
    public class CustomerService : ICustomerService
    {
        private ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers.Where(x => !x.IsDelete);
        }

        public IEnumerable<Customer> GetAll(int currentPage, int pageSize,string keyword)
        {

            var query = _context.Customers
                .Where(x => !x.IsDelete);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.CustomerCode.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
                                        x.CustomerName.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
                                        x.Address.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
                                        x.Phone.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
                                        x.Email.Trim().ToLower().Contains(keyword.Trim().ToLower())
                );
            }

            return query
                .Skip(pageSize * currentPage)
                .Take(pageSize);
        }

        public Customer GetById(int id)
        {
            return _context.Customers.Where(x => x.CUSTOMER_ID == id).FirstOrDefault();
        }

        public async Task<string> Create(Customer param)
        {
            {
                if (string.IsNullOrWhiteSpace(param.CustomerCode))
                    throw new AppException(ResultErrorEnum.MODEL_MISS);

                if (_context.Customers.Where(u => u.CustomerCode == param.CustomerCode).Any())
                {
                    throw new AppException(ResultErrorEnum.CODE_EXIST);
                }
                _context.Customers.Add(param);
                await _context.SaveChangesAsync();

                return string.Empty;
            }
        }

        public async Task<string> Update(Customer param)
        {
            var customer = await _context.Customers.FindAsync(param.CUSTOMER_ID);

            if (customer == null)
                throw new AppException(ResultErrorEnum.MODEL_NULL);

            customer.CustomerCode = param.CustomerCode;
            customer.CustomerName = param.CustomerName;
            customer.Address = param.Address;
            customer.Phone = param.Phone;
            customer.Email = param.Email;

            customer.UpdatedAt = DateTime.Now;
            customer.UpdatedBy = param.UpdatedBy;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return string.Empty;
        }

        public int Count(Expression<Func<Customer, bool>> where = null)
        {
            if (where != null)
            {
                return _context.Customers.Where(where).Count(x => !x.IsDelete);
            }
            return _context.Customers.Count(x => !x.IsDelete);

        }
        public string Delete(int id)
        {
            try
            {
                 _context.Database.BeginTransactionAsync();
                var Customer = _context.Customers.Find(id);
                if (Customer != null)
                {
                    _context.Customers.Remove(Customer);
                }
                 _context.SaveChanges();
                _context.Database.CommitTransaction();
                return string.Empty;
            }
            catch
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }
    }
}
