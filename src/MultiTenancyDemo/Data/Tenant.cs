﻿using System;

namespace MultiTenancyDemo.Data
{
    public class Tenant:ISoftDelete,IHasCreateTime,IHasUpdateTime
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string HostName{get;set;}
        
        public TenantType TenantType { get; set; }
        
        public string Connection { get; set; }
        
        public TenantDbType TenantDbType { get; set; }

        public bool IsActive{get;set;}

        public bool IsDeleted{get;set;}

        public DateTime CreateTime{get;set;}

        public DateTime DeleteTime{get;set;}
        
        public DateTime UpdateTime { get ;set; }
    }
}