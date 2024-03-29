
﻿namespace EventSourcing.UnitTests.TestDomain.Events
{
    using DomainDrivenDesign;
    using DomainDrivenDesign.Interfaces;
    using DomainDrivenDesign.Shared;
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
