# Sunlix.NET.DDD.BaseTypes
[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Sunlix.NET.DDD.BaseTypes.svg)](https://www.nuget.org/packages/Sunlix.NET.DDD.BaseTypes/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sunlix.NET.DDD.BaseTypes.svg)](https://www.nuget.org/packages/Sunlix.NET.DDD.BaseTypes/)
[![GitHub license](https://img.shields.io/github/license/Sunlix-Software/Sunlix.NET.DDD.BaseTypes.svg)](https://github.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes/blob/main/LICENSE)

<p>
  <img src="https://raw.githubusercontent.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes/main/src/Sunlix.NET.DDD.BaseTypes/assets/sunlix.png" width="130" align="left" style="margin-right: 15px;"/>
  <strong>Sunlix.NET.DDD.BaseTypes</strong> is a lightweight and extensible library designed for building robust domain models in C#.  
  It provides a clean foundation for implementing Domain-Driven Design (DDD) patterns by introducing essential base types:  
  <code>Entity</code>, <code>ValueObject</code>, <code>Enumeration</code>, <code>Error</code> and <code>Unit</code>. These primitives help developers write more expressive, consistent, and maintainable domain logic while reducing boilerplate code. The library is framework-agnostic, making it suitable for use in microservices, monoliths, and modular applications
</p>

## Table of contents
- [Why this library](#why-this-library)
- [Installation](#installation)  
- [Usage](#usage)  
  - [Entity\<TId>](#entitytid)  
  - [ValueObject](#valueobject)  
  - [Enumeration\<T>](#enumerationt)  
  - [Error](#error)
  - [Unit](#unit)
- [FAQ](#faq)
- [License](#license)  
- [Contributing](#contributing)

## Why this library

* **Less boilerplate** — concise base types for entities and value objects.
* **Clear equality semantics** — identity-based for entities, structural for value objects.
* **Extensible enumerations** — strongly typed, with both data and behavior.
* **Consistent error model** — an `Error` type for representing validation failures and business rule violations instead of relying on exceptions.  
* **ORM-friendly** — supports ORMs such as Entity Framework Core and NHibernate; includes safe patterns for lazy-loading proxies and DB-generated identifiers.
* **Addresses common pitfalls** — the implementation handles issues developers often hit when rolling their own (e.g., equality traps, proxy type mismatches, inconsistent hash codes).

## Installation
You can install the package via NuGet:
```sh
dotnet add package Sunlix.NET.DDD.BaseTypes
```

## Usage
This section contains small, self-contained examples that demonstrate how to use the types from **Sunlix.NET.DDD.BaseTypes**. The sample domain classes are deliberately simplified: they do not model a real domain, are not related to each other, and exist solely to illustrate the API surface. For clarity, the snippets omit nonessential infrastructure (e.g., error handling, logging, ORM setup, etc.).

### Entity\<TId>
Entities represent domain concepts that must be individually tracked over time. Each entity has a **unique identity** that distinguishes it from all others and enables references across the system (e.g. `Order`, `Customer` or `Account`). Unlike value objects, entities are **mutable**: their attributes can change, but their identity remains the same. Entities typically **have a lifespan** (*creation → updates → archival/deletion*) and often participate in aggregates and transactions, constrained by invariants. 

Whether two entity instances **are “the same” depends on context**. Two objects can refer to the same business entity yet carry different snapshots of state (staleness, out-of-date caches). We don’t override `Equals` on `Entity<TId>`. Instead we provide a domain service `Entity<TId>.IdEqualityComparer` which compares entities by `UnproxiedType` and `Id`.

**Example: Entity with app-assigned id**

```csharp
public sealed class Book : Entity<Guid>
{
    public string Title { get; private set; } = string.Empty;

    // Parameterless constructor for ORM
    private Book() { }

    public Book(Guid id, string title) : base(id)
    {
        if (string.IsNullOrEmpty(title))
            throw new ArgumentOutOfRangeException(nameof(title));
        Title = title;
    }
}

// Usage
var book = new Book(Guid.NewGuid());
context.Books.Add(book);
await context.SaveChangesAsync();              // Id is assigned by application   
```

**Example: Entity with DB-generated id**

```csharp
public sealed class Book : Entity<int>
{
    public string Title { get; private set; } = string.Empty;

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
            /*.UseIdentityColumn();             // use the IDENTITY feature to generate entity Id
    });
}

// Usage
var book = new Book("Domain-Driven Design");   // Id == default (transient entity)
context.Books.Add(book);
await context.SaveChangesAsync();              // Id populated by EF Core
```

#### Proxy note (EF Core & NHibernate).
ORMs create lazy-loading proxies for loaded objects. The entity base class default implementation `UnproxiedType => GetType()` will return the proxy type, not the domain type in this scenario. Since `IdEqualityComparer` compares entities by `UnproxiedType + Id`, a proxy and a non-proxy instance of the same entity may compare as different if you keep the default. You can override `UnproxiedType` to overcome this issue.

To avoid this, override `UnproxiedType` in the derived class to return the real domain type:

```csharp
public sealed class Book : Entity<int>
{
    protected override Type UnproxiedType => typeof(Book);
}
```

### ValueObject
A value object models a descriptive aspect of the domain rather than a distinct, trackable thing. Typical examples include money amounts, dates and ranges, measurements, addresses, and email addresses. Unlike entities, value objects have **no identity**: two instances with the same values are fully interchangeable. They are **immutable** — any modification produces a new instance, which simplifies reasoning, avoids side effects, and ensures safe sharing. Value objects are **ephemeral**: they are created where needed, passed around, and discarded. The domain doesn’t care about their history, only the value they hold at the moment.

**Equality is structural**: two value objects are equal if they share the same `UnproxiedType` and all components returned by `GetEqualityComponents() `are equal in length, order, and value. Hash codes are derived from these components, enabling correct use in sets and dictionaries.

Value objects should also be **validated and normalized at creation (fail fast)**, often enforcing invariants like ranges, formats, or canonical representations (e.g., uppercased currency codes). They can also be composed into larger value objects (e.g., an `Address` built from `Street`, `City`, and `PostalCode`).

**Example:**

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

### Enumeration\<T>
An `Enumeration<T>` represents a closed set of named values that carry domain meaning and may also include behavior. Unlike plain enums, enumerations are modeled as objects with **both data and optional logic**. Each instance has an integer `Value` that serves as its identifier and a string `Name` as its label. Enumerations are immutable and declared as static fields on the derived type, effectively acting as **singletons for the lifetime of the process**. 

Equality is based on the `UnproxiedType` and the numeric `Value`. Use an `Enumeration<T>` when the domain requires a fixed, named set of options that can also enforce invariants or provide behavior, making it more expressive than a basic enum.

**Example:**

```csharp
public sealed class TaxCategory : Enumeration<TaxCategory>
{
    public decimal Rate { get; } // e.g., 0.20m = 20%

    private TaxCategory(int value, string name, decimal rate)
        : base(value, name) => Rate = rate;

    public decimal ApplyTax(decimal net) => Math.Round(net * (1 + Rate), 2);

    public static readonly TaxCategory Standard = new(0, "Standard", 0.20m);
    public static readonly TaxCategory Reduced = new(1, "Reduced", 0.10m);
    public static readonly TaxCategory Zero = new(2, "Zero", 0.00m);
}

// Usage
var gross = TaxCategory.Reduced.ApplyTax(100m); // 110.00        
```

### Error
An `Error` is a lightweight value object that encapsulates a code and a descriptive message to represent failures in a structured form. Unlike exceptions, which signal unexpected and exceptional situations, errors model expected outcomes such as **validation failures or business rule violations**. Error instances are **immutable**: both `Code` and `Message` are set at creation and never change. 

Equality is based on the `UnproxiedType` and `Code`, making them stable for logging, comparison, or transport across layers. Use `Error` when you need a consistent and predictable way to propagate domain or application failures, especially when they are part of normal business logic rather than unexpected runtime faults.

**Example:**

```csharp
public readonly record struct Result<T>(T? Value, Error? Error)
{
    public bool IsSuccess => Error is null;
    public static Result<T> Ok(T value) => new(value, null);
    public static Result<T> Fail(Error e) => new(default, e);
}

public static class Errors
{
    public static readonly Error InvalidEmail = new("user.invalid_email", "Email format is invalid.");

    public static Error NotFound(string entity, object id)
        => new("common.not_found", $"{entity} with id '{id}' was not found.");
}

public Result<User> Register(string email)
{
    if (!IsValidEmail(email)) return Result<User>.Fail(Errors.InvalidEmail);
    // ...
    return Result<User>.Ok(new User(/*...*/));
}       
```
### Unit

`Unit` is a lightweight structure that represents the **absence of a meaningful result**. Unlike void, it can be used as a proper type in generics and functional pipelines, which makes it especially **useful in functional programming**. All instances of `Unit` are **equal and interchangeable**, with a single canonical instance exposed as `Unit.value`. This allows APIs to return `Unit` when no data is needed but a return type is required, ensuring type safety and consistency across layers.

**Example:**

```csharp
public readonly record struct Result<T>(T? Value, Error? Error)
{
    public bool IsSuccess => Error is null;
    public static Result<T> Ok(T value) => new(value, null);
    public static Result<T> Fail(Error e) => new(default, e);
}

public Result<Unit> Save(User user)
{
    if (user.IsValid)
    {
        // perform save logic
        return Result<Unit>.Ok(Unit.value);
    }

    return Result<Unit>.Fail(new Error("invalid_user", "User validation failed"));
}
```
## FAQ
<details>
  <summary>Is the library compatible with ORMs (e.g., Entity Framework Core, NHibernate)?</summary><br/>
  
  Yes. The base types are designed to be **ORM-friendly**:

- **Parameterless constructors** are provided to support ORMs (e.g., EF Core, NHibernate) that rely on them for materialization.    
- **Database-generated identifiers** are naturally supported: `Entity<TId>` can be created with a default identifier and be populated by the ORM on save.  
</details>

<details>
  <summary>Why doesn’t <code>Entity&lt;TId&gt;</code> override <code>Equals</code>?</summary><br/>

Although entities are identified by their unique Id, entity equality and Id equality are not the same thing. Two entities may share the same Id but represent different snapshots of state — for example, due to database inconsistencies, stale data in caches, or concurrent updates. Whether such entities should be considered “equal” is **context-dependent**.  

For this reason, `Equals` is **not overridden** on `Entity<TId>`. Instead, the library provides a domain service `Entity<TId>.IdEqualityComparer`, which compares entities by their `Id` and `UnproxiedType` (to handle ORM proxies safely). This comparer can also be used in collections (e.g., `HashSet`, `Dictionary`) when equality by `Id` is required.  

**Example:**

```csharp
var same = Entity<Book>.IdEqualityComparer.Equals(book1, book2);
```

```csharp
var set = new HashSet<Book>(Entity<Book>.IdEqualityComparer)
{
    book1,
    book2
};
```
</details>

<details>
  <summary>What is <code>UnproxiedType</code> and when should I override it?</summary>

In lazy-loading scenarios, ORMs such as *Entity Framework Core* and *NHibernate* substitute the real domain object with a dynamically generated proxy. As a result, calling `GetType()` on such an object will return the proxy type instead of the actual domain type.  Since both `Entity<TId>.IdEqualityComparer` and `ValueObject.Equals` use the **type** as part of their equality checks, comparing a proxy instance to a real domain instance will lead to false negatives.  

To address this, `Entity<TId>` and `ValueObject` base classes rely on a virtual property `UnproxiedType` for equality checks, which by default simply calls `GetType()`. You can override it in a derived class to always return the real domain type, ensuring consistent equality behavior.

**Example:**

```csharp
public sealed class Book : Entity<int>
{
    public string Title { get; private set; } = string.Empty;

    // Parameterless constructor for ORM
    private Book() { }

    public Book(string title)
    {
        if (string.IsNullOrEmpty(title))
            throw new ArgumentOutOfRangeException(nameof(title));
        Title = title;
    }

    // Ensures equality checks are not broken by EF/NHibernate proxies
    protected override Type UnproxiedType => typeof(Order);
}
```

This mechanism is also useful in **inheritance hierarchies**. Sometimes, two entities of different CLR types should be considered conceptually the *same* domain type. By overriding `UnproxiedType`, you can unify them for equality checks:

**Example:**

```csharp
public abstract class Payment : Entity<Guid>
{
    protected Payment(Guid id) : base(id) { }

    // Treat all Payment subclasses as one conceptual type
    protected override Type UnproxiedType => typeof(Payment);
}

public sealed class CreditCardPayment : Payment
{
    public CreditCardPayment(Guid id) : base(id) { }
}

public sealed class BankTransferPayment : Payment
{
    public BankTransferPayment(Guid id) : base(id) { }
}
```
Here, `CreditCardPayment` and `BankTransferPayment` will compare equal if their Ids match, because equality is based on the shared conceptual type `Payment`.
</details>

## License
Sunlix.NET.DDD.BaseTypes is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contributing
Contributions are welcome! Feel free to open an issue or submit a pull request.
