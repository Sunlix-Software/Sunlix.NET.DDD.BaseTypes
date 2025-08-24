# Sunlix.NET.DDD.BaseTypes

[![.NET](https://img.shields.io/badge/.NET-6.0_|_8.0_|_9.0-blue)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Sunlix.NET.DDD.BaseTypes.svg)](https://www.nuget.org/packages/Sunlix.NET.DDD.BaseTypes/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sunlix.NET.DDD.BaseTypes.svg)](https://www.nuget.org/packages/Sunlix.NET.DDD.BaseTypes/)
[![GitHub license](https://img.shields.io/github/license/Sunlix-Software/Sunlix.NET.DDD.BaseTypes.svg)](https://github.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes/blob/main/LICENSE)

**Sunlix.NET.DDD.BaseTypes** is a lightweight and extensible library designed for building robust domain models in C#.
It provides a clean foundation for implementing Domain-Driven Design (DDD) patterns by introducing essential base types:
`Entity`, `ValueObject`, `Enumeration`, `Error`, and `Unit`.

These primitives help developers write more expressive, consistent, and maintainable domain logic while reducing boilerplate code.
The library is framework-agnostic, making it suitable for microservices, monoliths, and modular applications.

## Why this Library

* **Less boilerplate** — concise base types for entities and value objects.
* **Clear equality semantics** — identity-based for entities, structural for value objects.
* **Extensible enumerations** — strongly typed, with both data and behavior.
* **Consistent error model** — represent validation failures and business rule violations without relying on exceptions.
* **ORM-friendly** — supports Entity Framework Core, NHibernate; safe for proxies and DB-generated identifiers.
* **Addresses common pitfalls** — correct equality, consistent hash codes, proxy-safe type resolution.

*Full documentation & FAQ: see the [GitHub README](https://github.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes).*

---

## Installation

```bash
dotnet add package Sunlix.NET.DDD.BaseTypes
```

## Usage

### Entity\<TId>

Entities represent domain concepts that must be individually tracked over time. Each entity has a unique identity that distinguishes it from all others and enables references across the system (e.g. `Order`, `Customer` or `Account`).

**Example**:

```csharp
public sealed class Book : Entity<int>
{
    private Book() { }   // for ORM
    public Book(string title) => Title = title;

    public string Title { get; private set; } = string.Empty;

    protected override Type UnproxiedType => typeof(Book);
}
```

### ValueObject

A value object models a descriptive aspect of the domain rather than a distinct, trackable thing. Typical examples include money amounts, dates and ranges, measurements, addresses, and email addresses.

**Example**:

```csharp
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency) =>
        (Amount, Currency) = (amount, currency.ToUpperInvariant());

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

### Enumeration\<T>

An `Enumeration<T>` represents a closed set of named values that carry domain meaning and may also include behavior.

**Example**:

```csharp
public sealed class TaxCategory : Enumeration<TaxCategory>
{
    private TaxCategory(int value, string name, decimal rate) : base(value, name) =>
        Rate = rate;

    public decimal Rate { get; }
    public decimal ApplyTax(decimal net) => Math.Round(net * (1 + Rate), 2);

    public static readonly TaxCategory Standard = new(0, "Standard", 0.20m);
    public static readonly TaxCategory Reduced  = new(1, "Reduced",  0.10m);
    public static readonly TaxCategory Zero     = new(2, "Zero",     0.00m);
}
```

### Error

An `Error` is a lightweight value object that encapsulates a code and a descriptive message to represent failures in a structured form.

**Example**:

```csharp
public sealed class Error : ValueObject
{
    public string Code { get; }
    public string Message { get; }

    public Error(string code, string message) =>
        (Code, Message) = (code, message);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
```

### Unit

`Unit` is a lightweight structure that represents the absence of a meaningful result — essentially a semantic alternative to `void`.

**Example**:

```csharp
public readonly record struct Result<T>(T? Value, Error? Error)
{
    public bool IsSuccess => Error is null;
    public static Result<T> Ok(T value) => new(value, null);
    public static Result<T> Fail(Error e) => new(default, e);
}

public Result<Unit> Save(User user) =>
    user.IsValid ? Result<Unit>.Ok(Unit.value)
                 : Result<Unit>.Fail(new Error("invalid_user", "User validation failed"));
```

## Links

* **Source & docs:** [GitHub](https://github.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes)
* **License:** [MIT License](https://github.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes/blob/main/LICENSE)
