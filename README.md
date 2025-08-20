# Sunlix.NET.DDD.BaseTypes
[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Sunlix.NET.DDD.BaseTypes.svg)](https://www.nuget.org/packages/Sunlix.NET.DDD.BaseTypes/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sunlix.NET.DDD.BaseTypes.svg)](https://www.nuget.org/packages/Sunlix.NET.DDD.BaseTypes/)
[![GitHub license](https://img.shields.io/github/license/Sunlix-Software/Sunlix.NET.DDD.BaseTypes.svg)](https://github.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes/blob/main/LICENSE)

<p>
  <img src="https://raw.githubusercontent.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes/main/src/Sunlix.NET.DDD.BaseTypes/assets/sunlix.png" width="130" align="left" style="margin-right: 15px;"/>
  <strong>Sunlix.NET.DDD.BaseTypes</strong> is a lightweight and extensible library designed for building robust domain models in C#.  
  It provides a clean foundation for implementing Domain-Driven Design (DDD) patterns by introducing essential base types:  
  <code>Entity</code>, <code>ValueObject</code>, <code>Enumeration</code>, and <code>Error</code>. These primitives help developers write more expressive, consistent, and maintainable domain logic while reducing boilerplate code. The library is framework-agnostic, making it suitable for use in microservices, monoliths, and modular applications
</p>

## Table of contents
## Table of Contents
- [Installation](#installation)  
- [Usage](#usage)  
  - [Entity\<TId>](#entitytid)  
  - [ValueObject](#valueobject)  
  - [Enumeration\<T>](#enumerationt)  
  - [Error](#error)
  - [Unit](#unit) 
- [License](#license)  
- [Contributing](#contributing)  

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

#### Example: Entity with app-assigned id

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
var user = new User(Guid.NewGuid());
context.Users.Add(user);
await context.SaveChangesAsync();              // Id is assigned by application   
```

#### Example: Entity with DB-generated id

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
            /*.UseIdentityColumn();             // use the IDENTITY feature to generate entity Id*/
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
public sealed class Order : Entity<int>
{
    protected override Type UnproxiedType => typeof(Order);
}
```

### ValueObject
A value object models a descriptive aspect of the domain rather than a distinct, trackable thing. Typical examples include money amounts, dates and ranges, measurements, addresses, and email addresses. Unlike entities, value objects have **no identity**: two instances with the same values are fully interchangeable. They are **immutable** — any modification produces a new instance, which simplifies reasoning, avoids side effects, and ensures safe sharing. Value objects are **ephemeral**: they are created where needed, passed around, and discarded. The domain doesn’t care about their history, only the value they hold at the moment.

**Equality is structural**: two value objects are equal if they share the same `UnproxiedType` and all components returned by `GetEqualityComponents() `are equal in length, order, and value. Hash codes are derived from these components, enabling correct use in sets and dictionaries.

Value objects should also be **validated and normalized at creation (fail fast)**, often enforcing invariants like ranges, formats, or canonical representations (e.g., uppercased currency codes). They can also be composed into larger value objects (e.g., an `Address` built from `Street`, `City`, and `PostalCode`).

#### Example: Simple value object

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

#### Example: Enumeration with behaviour

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

#### Example: Validation error

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

#### Example: Unit in Result record

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

## License
Sunlix.NET.DDD.BaseTypes is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contributing
Contributions are welcome! Feel free to open an issue or submit a pull request.
