# EventSourcing

CQRS can be used without event sourcing or DDD, just as these concepts work without CQRS. However there is no denying that three concepts complement each other.

So this library gives a template to implement a DDD pattern using the Event Sourcing pattern, which can be implemented with a CQRS pattern .With this pattern, application state is stored as sequence of events. Each event represents a set of changes to the data. The current state is constructed by replaying the events

![alt text](https://www.codeproject.com/KB/architecture/555855/CQRS.jpg)


Reference: This is a re-implementation of Greg Young's [SimpleCQRS](https://github.com/gregoryyoung/m-r) project, arguably the de-facto sample application for CQRS, DDD & Event Sourcing.

## Event Store (IDomainRepository)
These should be written in their own package or project libabry and injected in based on IDomainRepository. Not added to this library.

The package does have a DomainRepositoryBase that can help with that.

### Memory Event Store Provided
The memory event store can be used but not sure how useful that would be in production. 

It is there for test purposes and along with demostrating how to write other event stores in different persitance. 

## Domain Event Publisher (IDomainEventPublisher)

This is optional to provide and allows domain events to published after they have successfully saved in the IDomainRepository

These should be written in their own package or project libabry and injected in based on IEventPublisher. Not added to this library.

NOTE: Suggest NOT exposing the domain events and convert them to external events if not within the domain bounded context. This abstraction allows your domain not to be consumed and therefore not impact changes inside your bounded context. see CQRS_Example.pdf


