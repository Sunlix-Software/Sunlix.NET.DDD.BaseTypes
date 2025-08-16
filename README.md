# Sunlix.NET.DDD.BaseTypes

**Sunlix.NET.DDD.BaseTypes** is a lightweight library providing a set of base types for domain modeling in C#. It includes `Entity`, `ValueObject`, `Enumeration`, and `Error`, which are particularly useful in domain-driven design (DDD).

## Table of contents

* [Overview](#overview)
  * [Entity\<TId>](#entitytid)
  * [ValueObject](#valueobject)
  * [Enumeration\<T>](#enumerationt)
* [Usage examples](#usage-examples)
  * [Implementing a value object (Money)](#implementing-a-value-object-money)
  * [Modeling entities (DB-generated vs app-generated IDs)](#modeling-entities-db-generated-vs-app-generated-ids)
  * [Smart enums (OrderStatus)](#smart-enums-orderstatus)
* [Equality & hashing rules](#equality--hashing-rules)
* [Validation & exceptions](#validation--exceptions)
* [EF Core notes](#ef-core-notes)
* [Customization points](#customization-points)
* [FAQ](#faq)
* [License](#license)

## Overview
### Entity\<TId>
An `Entity<T>` is a domain object defined primarily by its identity, not by its attributes. It represents a distinct, trackable thing in the domain— e.g., an Order, Customer, or Account — whose history and continuity over time matter.

**General concepts:**

* **Domain role:** Entities model real domain concepts that must be individually tracked, constrained by invariants, and often participate in aggregates and transactions.
* **Identity:** An identifier that uniquely distinguishes one instance from all others and enables references between objects.
* **Lifespan:** The entity persists across multiple operations and can change over time (creation → updates → archival/deletion).
* **Mutability:** Unlike value objects, entities are mutable — their attributes can change while the identity remains the same.
* **Equality:** Whether two entity instances are “the same” depends on context. Two objects can refer to the same business entity yet carry different snapshots of state (staleness, out-of-date caches). We don’t override `Equals` on `Entity<TId>`. Instead we provide a domain service `Entity<TId>.IdEqualityComparer` which compares entities by `UnproxiedType` and `Id`. Use it when you explicitly mean “same conceptual entity + same identifier” (e.g., for reconciliation of entities).

**Rule of thumb:** use an `Entity<T>` when the business cares about which specific thing it is over time (its continuity and audit trail), not just what values it holds at a moment.

### ValueObject
A `ValueObject` models a descriptive aspect of the domain rather than a distinct, trackable thing. Typical examples include money amounts, dates and ranges, measurements, addresses, and email addresses.

**General concepts:**

* **Domain role:** A value object captures a concept defined entirely by its attributes (amount + currency, start + end date, latitude + longitude, etc.). It can include behavior that depends only on those attributes (e.g., normalization, arithmetic, comparison, validation), but not operations that depend on identity or lifecycle.
* **Identity:** Value objects have no identity. Two instances with the same values are interchangeable. You don’t “look them up by ID” or track them over time.
* **Lifespan:** Value objects are ephemeral. They’re created where needed, passed around, and discarded. The domain doesn’t care about their history, only about the value they represent at a given moment.
* **Mutability:** Value objects should be immutable. Any change produces a new instance (e.g., money.Add(tax) returns a new Money). Immutability simplifies reasoning, enables safe sharing, and avoids unintended side effects.
* **Equality:** Equality is structural — two value objects are equal if and only if they are of the same conceptual type (see `UnproxiedType`) and all of their significant components are equal (see `GetEqualityComponents`). Hash codes are derived from the same components so equal values behave correctly in sets and dictionaries.
* **Validation & normalization:** Value objects should be valid at creation (fail fast). They commonly normalize internal state (e.g., upper-case currency codes, trimmed strings) and enforce invariants (ranges, formats).
* **Composition:** Value objects can be composed of other value objects (e.g., Address composed of Street, City, PostalCode). Treat them as atomic inside aggregates.

**Rule of thumb:** Use a `ValueObject` when the business cares about what the value is, not which specific instance it is. If you need to reference it across the model, track its lifecycle, or audit its history, that’s a sign you need an entity instead.

### Enumeration\<T>
An `Enumeration<T>` models a closed set of named constants with domain meaning and optional behavior—richer than a plain enum. Typical examples: OrderStatus, DocumentState.

**General concepts:**

* **Domain role:** A fixed set of named values meaningful to the domain. Each `Enumeration<T>` is a first-class object that can expose behavior in addition to data.
* **Lifespan:** Enumerations are declared as `public static readonly` fields on the derived type. They behave like singletons for the lifetime of the process; you don’t create them dynamically in normal code.
* **Mutability:** `Enumeration<T>` instances are immutable. `Value` and `Name` are set in the constructor and never change.
* **Value and Name:** Each `Enumeration<T>` has an integer `Value` (the identifier) and a string `Name` (the human-readable label).
* **Equality:** Two `Enumeration<T>` instances are equal when they’re of the same conceptual type (see `UnproxiedType`) and have the same `Value`.

**Rule of thumb:** Choose an `Enumeration<T>` when you need a closed, named set with optional behavior and invariants, more expressive than a basic enum.

### Error
An `Error` is a lightweight `ValueObject` that carries an error code and a human-readable message for domain/application failures.

**General concepts:**

* **Domain role:** Represents failures in a structured form (validation errors, business rule violations). Useful for returning from domain services or mapping to API/problem-details.
* **Mutability:** `Error` instances are immutable. `Code` and `Message` are set in the constructor and never change.
* **Equality:** Two `Error` instances are equal when they’re of the same conceptual type (see `UnproxiedType`) and have the same `Value`.

**Rule of thumb:** Use `Error` when you need a consistent error payload (store/log by `Code`, `Message`, and keep equality stable across layers).

## Usage
This section contains small, self-contained examples that demonstrate how to use the types from **Sunlix.NET.DDD.BaseTypes**. The sample domain classes are deliberately simplified: they do not model a real domain, are not related to each other, and exist solely to illustrate the API surface. For clarity, the snippets omit nonessential infrastructure (e.g., error handling, logging, full EF Core setup) unless explicitly relevant. The examples below use **Entity Framework Core** as the ORM.

### Implement an entity with DB-generated Id
Use this approach when the database assigns the primary key (IDENTITY/SEQUENCE). Create the entity without an Id (parameterless constructor), let EF Core persist it, and the Id will be populated on `SaveChanges()`. Until then the entity is transient (`Id == default`). Configure EF with `ValueGeneratedOnAdd()` to indicate the Id is database-generated.

```csharp
public sealed class Book : Entity<int>
{
    public string Title { get; private set; }

    // Parameterless constructor for ORM
    private Book() { }

    public Book(string title)
    {
        if (string.IsNullOrEmpty(title))
            throw new ArgumentOutOfRangeException(nameof(title));
        Title = title;
    }
}

// EF Core DbContext 
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Book>(bookBuilder =>
    {
        bookBuilder.ToTable("Books").HasKey(b => b.Id);
        bookBuilder.Property(b => b.Id)
            .ValueGeneratedOnAdd();
            /*.UseIdentityColumn();             // use the IDENTITY feature to generate entity Id*/
    });
}

// Usage
var book = new Book("Domain-Driven Design");   // Id == default (transient entity)
context.Books.Add(book);
await context.SaveChangesAsync();              // Id populated by EF Core             
```

### Implement an entity with app-assigned Id
Use this approach when your application (or an upstream system) provides the identifier (e.g., Guid, ULID, Snowflake, or a typed ID value object). Construct the entity with the Id. Configure EF with `ValueGeneratedNever()`. The entity is non-transient immediately, so Id-based comparisons via `IdEqualityComparer` are safe before persistence.
```csharp
public sealed class User : Entity<Guid>
{
    // Parameterless constructor for ORM
    private User() { }

    public User(Guid id) : base(id) { }
}

// EF Core DbContext 
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>(userBuilder =>
    {
        bookBuilder.ToTable("Users").HasKey(b => b.Id);
        bookBuilder.Property(b => b.Id)
            .ValueGeneratedNever();
    });
}

// Usage
var user = new User(Guid.NewGuid());
context.Users.Add(user);
await context.SaveChangesAsync();              // Id is assigned by application           
```

### Implement an entity with strongly typed Id
Using strongly typed Ids prevents accidental mix-ups (e.g., passing a CustomerId where an OrderId is expected), makes APIs self-documenting, allows validation in one place, and still maps cleanly in EF Core via a value converter.

```csharp
public readonly record struct PersonId(Guid Value)
{
    public static PersonId Empty { get; } = default;
    public static PersonId CreateNew() => new(Guid.NewGuid());
}

public sealed class Person
{
    // Strongly typed Id
    public PersonId Id { get; private set; } = PersonId.Empty;

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    // Parameterless constructor for ORM
    private Person() { }

    public static Person CreateNew(string firstName, string lastName) => new()
    {
        FirstName = firstName,
        LastName = lastName,
        Id = PersonId.CreateNew()
    };
}

// EF Core DbContext 
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>(personBuilder =>
    {
        personBuilder.ToTable("People").HasKey(p => p.Id);
        personBuilder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new PersonId(value));
    });
}

// Usage
var person = Person.CreateNew("John", "Smith");
context.People.Add(person);
await context.SaveChangesAsync();           
```

### Transience check

`IsTransient()` tells you whether the entity has a default ID.

```csharp
if (order.IsTransient()) { /* not persisted yet */ }           
```

### Comparing entities

Entity equality is context-dependent, so the base type does not override `Equals`. When you explicitly mean *same entity by Id*, use the provided comparer:

```csharp
var set = new HashSet<Entity<Guid>>(Entity<Guid>.IdEqualityComparer);

var entityFromEfProxy = /* entity loaded via EF (proxy) */;
var entityFromRepository = /* same entity loaded elsewhere */;

set.Add(fromEfProxy);
bool same = set.Contains(fromRepository); // true if UnproxiedType matches and Ids are equal           
```
You can use the comparer in dictionaries, sets, etc.
```csharp
var dict = new Dictionary<Entity<Guid>, string>(Entity<Guid>.IdEqualityComparer);           
```

### Overriding `Entity<TId>.UnproxiedType` (optional)

Override `UnproxiedType` to compare a hierarchy as one conceptual type (e.g., treat all Payment subclasses as the same concept). Ensure Ids are unique across the hierarchy before unifying types like this.

```csharp
public abstract class Payment : Entity<Guid>
{
    // Treat all payments as the same conceptual type in Id-based comparisons
    protected override Type UnproxiedType => typeof(Payment);
}

public sealed class CardPayment : Payment { }
public sealed class BankTransfer : Payment { }           
```

#### Proxy note (EF Core & NHibernate).
ORMs create lazy-loading proxies for loaded objects. The entity base class default implementation `UnproxiedType => GetType()` will return the proxy type, not the domain type in this scenario. Since `IdEqualityComparer` compares entities by `UnproxiedType + Id`, a proxy and a non-proxy instance of the same entity may compare as different if you keep the default. You can override `UnproxiedType` to overcome this issue.

To avoid this, override `UnproxiedType` in the derived class to return the real domain type:
```csharp
public sealed class Order : Entity<int>
{
    protected override Type UnproxiedType => typeof(Order);
}
```

### Implement a value object

Define the value’s data and return its significant parts from `GetEqualityComponents()`.  
Equality is structural: `a.Equals(b)` returns true only if both conditions hold:
* `a.UnproxiedType == b.UnproxiedType`
* `a.GetEqualityComponents()` and `b.GetEqualityComponents()` are sequence-equal — same length, same order, and pairwise-equal components.

```csharp
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Required.", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant(); // normalize
    }

    // Return components used for equality check (order matters)
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}

// Structural equality
var money1 = new Money(10m, "usd");
var money2 = new Money(10m, "USD");
Console.WriteLine(money1 == money2); // true           
```

### Fail-fast validation & normalization

Put invariants and normalization in the constructor so every instance is valid by design.  
***Examples:*** *trimming strings, upper-casing codes, range checks, format checks.*

```csharp
public sealed class Email : ValueObject
{
    public string Value { get; }
    public Email(string value)
    {
        if (!IsValidEmail(value))
            throw new ArgumentException("Email is invalid.", nameof(value));
        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}         
```

### Using value objects in collections (hashing)

`GetHashCode()` is derived from the same components as equality (plus the unproxied type). The hash is cached for performance.

```csharp
var set = new HashSet<ValueObject> { new Money(10, "USD") };
set.Contains(new Money(10, "USD")); // true   
```
### Overriding `ValueObject.UnproxiedType` (optional)

ORMs create lazy-loading proxies for loaded objects. The value object base class default implementation `UnproxiedType => GetType()` will return the proxy type, not the domain type in this scenario. Since `ValuObject` compares objects by `UnproxiedType + equality components`, a proxy and a non-proxy instance of the same value object may compare as different if you keep the default. You can override `UnproxiedType` to overcome this issue.

```csharp
public class Money : ValueObject
{
    // as above…
    protected override Type UnproxiedType => typeof(Money);
}           
```

