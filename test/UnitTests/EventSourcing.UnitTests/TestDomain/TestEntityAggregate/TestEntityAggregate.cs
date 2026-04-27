using DomainDrivenDesign;
using DomainDrivenDesign.Interfaces;
using Common.Models.AuditInfo;
using EventSourcing.UnitTests.TestDomain.Events;


﻿namespace EventSourcing.UnitTests.TestDomain.TestEntityAggregate
{

    public class TestEntityAggregate : AggregateRoot, IAggregate
    {
        public string Id { get; set; }
        public override string AggregateId
        {
            get { return Id; }
        }

        public string TestProperty { get; set; }

        public override string Name => nameof(TestEntityAggregate);

        public TestEntityAggregate()
        {
            RegisterTransition<TestEntityCreated>(Apply);
            RegisterTransition<TestEntityModified>(Apply);
        }

        private void Apply(TestEntityCreated obj)
        {
            Id = obj.Id;
        }

        private void Apply(TestEntityModified obj)
        {
            TestProperty = obj.TestProperty;
        }

        public void Create(AuditInfo auditInfo, string id)
        {
            RaiseEvent(new TestEntityCreated(id, auditInfo));
        }

        public void Update(AuditInfo auditInfo, string id, string testProperty)
        {
            RaiseEvent(new TestEntityModified(id, auditInfo, testProperty));
        }

    }
}
