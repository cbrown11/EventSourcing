
﻿namespace EventSourcing.UnitTests.TestDomain.Events
{
    using Common.Models.AuditInfo;
    using DomainDrivenDesign;
    using DomainDrivenDesign.Interfaces;
    public class TestEntityCreated : DomainEvent, IDomainEvent
    {
        public string TestProperty { get; set; }

        // Need for deserialize 
        public TestEntityCreated() { }

        public TestEntityCreated(string id, AuditInfo auditInfo) : base(auditInfo, id)
        {
        }

    }
}
