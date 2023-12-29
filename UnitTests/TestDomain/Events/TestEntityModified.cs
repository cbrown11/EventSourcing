﻿namespace EventSourcing.UnitTests.TestDomain.Events
{
    using DomainDrivenDesign.DDD;
    using DomainDrivenDesign.DDD.Interfaces;
    using DomainDrivenDesign.Shared;


    public class TestEntityModified : DomainEvent, IDomainEvent
    {
        public string TestProperty { get; set; }

        // Need for deserialize 
        public TestEntityModified() { }

        public TestEntityModified(string id, AuditInfo auditInfo, string testProperty) : base(auditInfo, id)
        {
            TestProperty = testProperty;
        }
    }
}
