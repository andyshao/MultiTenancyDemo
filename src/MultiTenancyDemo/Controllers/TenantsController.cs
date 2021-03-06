﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancyDemo.Data;
using MultiTenancyDemo.Repository;
using System.Linq;
using System.Threading.Tasks;
using MultiTenancyDemo.Uow;
using StackExchange.Redis;

namespace MultiTenancyDemo.Controllers
{
    public class TenantsController : Controller
    {
        private readonly IMultiTenantRepositoryBase<Tenant> _repository;
        private readonly IMultiTenancyDemoUnitOfWork _unitOfWork;

        public TenantsController(IMultiTenantRepositoryBase<Tenant> repository,
                                IMultiTenancyDemoUnitOfWork unitOfWork)
        {
            _repository = repository;
            this._unitOfWork=unitOfWork;
        }

        // GET: Tenants
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAll().ToListAsync());
        }

        // GET: Tenants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _repository.GetAll()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        // GET: Tenants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tenants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,HostName,TenantType,Connection,TenantDbType,IsActive,IsDeleted,CreateTime,DeleteTime")] Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                
                await _repository.CreateAsync(tenant);
                await _unitOfWork.SaveChangesAsync();
                if(tenant.TenantType== TenantType.有钱租户)
                {
                    System.Console.WriteLine("开始创建数据库");
                    using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379"))
                    {
                        System.Console.WriteLine("发布消息");
                        ISubscriber sub = redis.GetSubscriber();
                        sub.Publish("createtenant", tenant.Connection);
                        System.Console.WriteLine("消息发布成功");
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View(tenant);
        }

        // GET: Tenants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _repository.GetAll().FirstAsync(r=>r.Id==id);
            if (tenant == null)
            {
                return NotFound();
            }
            return View(tenant);
        }

        // POST: Tenants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,HostName,TenantType,Connection,TenantDbType,IsActive,IsDeleted,CreateTime,DeleteTime")] Tenant tenant)
        {
            if (id != tenant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   await _repository.UpdateAsync(tenant);
                   await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TenantExists(tenant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tenant);
        }

        // GET: Tenants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _repository.GetAll()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        // POST: Tenants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tenant = await _repository.GetAll().FirstAsync(r=>r.Id==id);
            //_context.Tenant.Remove(tenant);
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TenantExists(int id)
        {
            return _repository.GetAll().Any(e => e.Id == id);
        }
    }
}
