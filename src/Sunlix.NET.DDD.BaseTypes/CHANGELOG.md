# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.4] - 2025-08-24
### Changed
- `Entity<TId>.IsTransient` converted from method to property.
- Added support for .NET 6.0 and .NET 8.0.

### Fixed
- Fixed XML documentation.

## [1.0.3] - 2025-08-21
### Added
- Initial public release of **Sunlix.NET.DDD.BaseTypes**.
- Introduced core base types for Domain-Driven Design (DDD):
  - **Entity\<TId>** with proxy-safe `IdEqualityComparer`.
  - **ValueObject** with structural equality based on `GetEqualityComponents()`.
  - **Enumeration\<T>** with lazy caching, duplicate detection, and support for adding behavior.
  - **Error** type for representing domain/business errors (instead of exceptions).
  - **Unit** semantic type as an alternative to `void` in functional flows.
- Added XML documentation for public APIs.
- Packaged with README and license for NuGet distribution.



[1.0.3]: https://github.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes/tree/1.0.3
[1.0.4]: https://github.com/Sunlix-Software/Sunlix.NET.DDD.BaseTypes/tree/1.0.4