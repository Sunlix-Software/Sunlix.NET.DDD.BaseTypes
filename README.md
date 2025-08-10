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

