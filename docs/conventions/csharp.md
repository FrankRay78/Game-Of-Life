# C# .NET 10 Conventions

## Naming
- PascalCase: classes, methods, public properties, events, enums, **constants** (not ALL_CAPS)
- camelCase: local variables, method parameters, private/internal fields
- _camelCase: private instance fields (underscore prefix)
- s_camelCase: private/internal static fields
- t_camelCase: thread-static fields
- IPrefix: interfaces (e.g. IWorkerQueue)
- No Hungarian notation, no abbreviations

## Primary Constructor Parameters
- class/struct types: camelCase (consistent with method parameters)
- record types: PascalCase (they become public properties)

## var / Implicit Typing
- Use var when type is obvious from the right-hand side: new, explicit cast, or literal
- Do NOT use var when type comes from a method name alone (e.g. `var x = GetValue()`)
- Do NOT use var in foreach loops — state the element type explicitly
- DO use var in LINQ queries (anonymous/nested generic types)

## Namespaces & Files
- File-scoped namespaces: `namespace Company.Product.Module;`
- Place using directives OUTSIDE the namespace declaration
- One class per file; filename matches class name

## Strings
- Use string interpolation for short strings: $"{first}, {last}"
- Use StringBuilder for string concatenation in loops
- Prefer raw string literals over escape sequences

## Collections
- Use collection expressions: `string[] x = ["a", "b"];`

## Async
- Async methods must have the Async suffix
- Never use .Result or .Wait() — always await
- Include CancellationToken on all public async methods
- Return Task, not void (except event handlers)
- Use ConfigureAwait(false) in library code

## Error Handling
- Only catch exceptions you can meaningfully handle
- Catch specific exception types, not System.Exception
- Use `using` declarations (not blocks): `using var conn = ...;`
- Throw ArgumentNullException.ThrowIfNull() for null argument guards

## Code Structure
- Allman braces: opening brace on its own line
- Always use braces for if/else — never omit even for single-line bodies
- One statement per line, one declaration per line
- 4-space indentation (no tabs)
- using directives outside namespace declarations
- Member order: static fields → instance fields → constructors → properties → methods

## Dependency Injection
- Inject via constructor, never new() internally
- Depend on interfaces, not concrete types

## LINQ
- Use meaningful query variable names
- Use where clauses early to filter before other operations
- Prefer method syntax for simple chains; query syntax for complex multi-clause queries

## Testing (MSTest)
- Name tests: MethodName_Scenario_ExpectedResult
- Follow Arrange / Act / Assert
- Cover happy paths and edge cases

## Sources
- https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
- https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names
- https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md

Last reviewed: March 2026
