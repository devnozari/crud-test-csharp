﻿using Mc2.CrudTest.Domain.Contracts.Common;
using Mc2.CrudTest.Shared.Contracts;
using Mc2.CrudTest.Shared.Utility;
using Mc2.CrudTest.Storage.Database.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Logics.Logics
{
    public class CustomerLogic
    {
        private DatabaseContext DatabaseContext { get; set; }
        public CustomerLogic(DatabaseContext databaseContext)
        {
            DatabaseContext = databaseContext;
        }
        public async Task<MessageContract<long>> AddCustomer(CustomerContract customer)
        {
            try
            {
                var result = await DatabaseContext.Customers.AddAsync(customer.Convert());

                await DatabaseContext.SaveChangesAsync();

                return result.Entity.Id.ToContract();
            }
            catch (Exception ex)
            {
                return ex.ToErrorContract<long>();
            }
        }
        public async Task<MessageContract<long>> UpdateCustomer(CustomerContract customer)
        {
            try
            {
                var customerResult = Get(customer.Id.Value);

                if (customerResult.IsData())
                {
                    var result = DatabaseContext.Customers.Update(customer.Convert());

                    await DatabaseContext.SaveChangesAsync();

                    return result.Entity.Id.ToContract();
                }
                else
                {
                    return new MessageContract<long>() { IsSucess = false, ErrorMessage = "Not Find Data" };
                }
            }
            catch (Exception ex)
            {
                return ex.ToErrorContract<long>();
            }
        }
        public async Task<MessageContract<long>> Delete(long Id)
        {
            try
            {
                var customerResult = Get(Id);

                if (customerResult.IsData())
                {
                    var result = DatabaseContext.Customers.Remove(customerResult.Result.Convert());

                    await DatabaseContext.SaveChangesAsync();

                    return result.Entity.Id.ToContract();
                }
                else
                {
                    return new MessageContract<long>() { IsSucess = false, ErrorMessage = "Not Find Data" };
                }
            }
            catch (Exception ex)
            {
                return ex.ToErrorContract<long>();
            }
        }
        public MessageContract<CustomerContract> Get(long Id)
        {
            var result = DatabaseContext.Customers.FirstOrDefault(dr => dr.Id == Id);

            return CustomerContract.ConvertContract(result).ToContract();
        }
        public MessageContract<List<CustomerContract>> GetAll()
        {
            return DatabaseContext.Customers.Select(dr => CustomerContract.ConvertContract(dr)).ToList().ToContract();
        }
    }
}
