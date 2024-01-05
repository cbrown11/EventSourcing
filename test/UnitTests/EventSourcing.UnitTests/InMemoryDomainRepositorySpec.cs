
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using DomainDrivenDesign.Interfaces;
using EventSourcing.EventSourcing.Interfaces;
using DomainDrivenDesign.Shared;
using EventSourcing.UnitTests.TestDomain.TestEntityAggregate;
using EventSourcing.InMemory;


namespace EventSourcing.UnitTests
{

    public abstract class InMemoryDomainRepositorySpec
    {
        protected static string Id;
        protected static DomainRepository SUT;
        protected static TestEntityAggregate TestEntityAggregate;
        protected static Mock<IDomainEventPublisher> TransientDomainEventPublisherMock;
        protected static Exception _exception;
        Establish context = () =>
        {
            TransientDomainEventPublisherMock = new Mock<IDomainEventPublisher>();
            SUT = new DomainRepository("TestCategory", TransientDomainEventPublisherMock.Object);
        };
    }


    [Subject(typeof(DomainRepository))]
    public class when_saving_a_aggregate_with_a_domain_event : InMemoryDomainRepositorySpec
    {
        Establish context = () =>
        {
            TestEntityAggregate = new TestEntityAggregate();
            Id = string.Format("CreateId-{0}", Guid.NewGuid());
            TestEntityAggregate.Create(new AuditInfo(), Id);
            SUT = new DomainRepository("TestCategory");
        };

        Because of = () => _exception = Catch.Exception(() => SUT.SaveAsync(TestEntityAggregate));

        It should_have_not_raised_any_errors = () => _exception.ShouldBeNull();
        It should_saved_successfully = () => SUT.GetByIdAsync<TestEntityAggregate>(Id).Result.ShouldNotBeNull();
        It should_saved_the_id_successfully = () => SUT.GetByIdAsync<TestEntityAggregate>(Id).Result.Id.ShouldNotBeNull();
    }

    [Subject(typeof(DomainRepository))]
    public class when_saving_a_aggregate_with_a_domain_event_and_publish_saved_domain_events : InMemoryDomainRepositorySpec
    {
        Establish context = () =>
        {
            TestEntityAggregate = new TestEntityAggregate();
            Id = string.Format("CreateId-{0}", Guid.NewGuid());
            TestEntityAggregate.Create(new AuditInfo(), Id);
        };

        Because of = () => _exception = Catch.Exception(() => SUT.SaveAsync(TestEntityAggregate));

        It should_have_not_raised_any_errors = () => _exception.ShouldBeNull();
        It should_saved_successfully = () => SUT.GetByIdAsync<TestEntityAggregate>(Id).Result.ShouldNotBeNull();
        It should_saved_the_id_successfully = () => SUT.GetByIdAsync<TestEntityAggregate>(Id).Result.Id.ShouldNotBeNull();
        It should_publish_the_event = () => TransientDomainEventPublisherMock.Verify(foo => foo.PublishAsync(Moq.It.IsAny<IDomainEvent>(), default), Times.AtLeastOnce);
    }


    [Subject(typeof(DomainRepository))]
    public class when_saving_aggregate_thats_has_10_uncommited_domain_events : InMemoryDomainRepositorySpec
    {
        protected static int CountLength = 10;

        private Establish context = () =>
        {
            var auditInfo = new AuditInfo();
            TestEntityAggregate = new TestEntityAggregate();
            Id = string.Format("MultipleTest10Id-{0}", Guid.NewGuid());
            TestEntityAggregate.Create(auditInfo, Id);
            for (int i = 1; i <= CountLength; i++)
            {
                TestEntityAggregate.Update(auditInfo, Id, string.Format("Test string value {0}", i));
            }
        };
        Because of = () => _exception = Catch.Exception(() => SUT.SaveAsync(TestEntityAggregate));
        It should_have_not_raised_any_errors = () => _exception.ShouldBeNull();
        It should_saved_successfully = () => SUT.GetByIdAsync<TestEntityAggregate>(Id).Result.ShouldNotBeNull();
        It should_saved_the_id_successfully = () => SUT.GetByIdAsync<TestEntityAggregate>(Id).Result.Id.ShouldNotBeNull();
        It should_publish_all_the_events = () => TransientDomainEventPublisherMock.Verify(foo => foo.PublishAsync(Moq.It.IsAny<IDomainEvent>(), default), Times.Exactly(CountLength + 1));
    }


    [Subject(typeof(DomainRepository))]
    public class when_saving_aggregate_thats_has_200_uncommited_domain_events : InMemoryDomainRepositorySpec
    {
        protected static int CountLength = 200;

        private Establish context = () =>
        {
            var auditInfo = new AuditInfo();
            TestEntityAggregate = new TestEntityAggregate();
            Id = string.Format("MultipleTest200PlusId-{0}", Guid.NewGuid());
            TestEntityAggregate.Create(auditInfo, Id);
            for (int i = 1; i <= CountLength; i++)
            {
                TestEntityAggregate.Update(auditInfo, Id, string.Format("Test string value {0}", i));
            }
        };
        Because of = () => _exception = Catch.Exception(() => SUT.SaveAsync(TestEntityAggregate));
        It should_have_not_raised_any_errors = () => _exception.ShouldBeNull();
        It should_saved_successfully = () => SUT.GetByIdAsync<TestEntityAggregate>(Id).Result.ShouldNotBeNull();
        It should_publish_all_the_events = () => TransientDomainEventPublisherMock.Verify(foo => foo.PublishAsync(Moq.It.IsAny<IDomainEvent>(), default), Times.Exactly(CountLength + 1));
    }

    [Subject(typeof(DomainRepository))]
    public class when_handling_multiple_aggregates : InMemoryDomainRepositorySpec
    {
        protected static TestEntityAggregate Result;
        protected static int CountLength = 4;

        private Establish context = () =>
        {
            for (int i = 1; i <= CountLength; i++)
            {
                TestEntityAggregate = new TestEntityAggregate();
                Id = i.ToString();
                TestEntityAggregate.Create(new AuditInfo(), Id);
                SUT.SaveAsync(TestEntityAggregate).Wait();
            }
        };

        Because of = () => Result = SUT.GetByIdAsync<TestEntityAggregate>(Id).Result;

        It should_have_last_stream = () => Result.Id.ShouldEqual(CountLength.ToString());
        It should_have_first_stream = () => SUT.GetByIdAsync<TestEntityAggregate>("1").Result.Id.ShouldEqual("1");
    }


    [Subject(typeof(DomainRepository))]
    public class when_saving_aggregate_thats_has_5000_uncommited_domain_events : InMemoryDomainRepositorySpec
    {
        protected static TestEntityAggregate TestEntityAggregate;
        protected static string Id;
        protected static int CountLength = 5000;

        private Establish context = () =>
        {
            var auditInfo = new AuditInfo();
            TestEntityAggregate = new TestEntityAggregate();
            Id = string.Format("OverEventReadLimitCountId-{0}", Guid.NewGuid());
            TestEntityAggregate.Create(auditInfo, Id);
            for (int i = 1; i <= CountLength; i++)
            {
                TestEntityAggregate.Update(auditInfo, Id, string.Format("Test string value {0}", i));
            }
        };
        Because of = () => _exception = Catch.Exception(() => SUT.SaveAsync(TestEntityAggregate));
        It should_have_not_raised_any_errors = () => _exception.ShouldBeNull();
        It should_saved_successfully = () => SUT.GetByIdAsync<TestEntityAggregate>(Id).Result.ShouldNotBeNull();
        It should_publish_all_the_events = () => TransientDomainEventPublisherMock.Verify(foo => foo.PublishAsync(Moq.It.IsAny<IDomainEvent>(), default), Times.Exactly(CountLength + 1));
    }
}
