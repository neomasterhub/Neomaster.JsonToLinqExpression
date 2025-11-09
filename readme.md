<div align="center">
  <img src="Docs/img/title_path.svg">
</div>

<br>

<div align="center">

  [![License](https://img.shields.io/badge/🧾License-MIT-green?style=flat)](https://opensource.org/licenses/MIT)
  [![Telegram Channel](https://img.shields.io/badge/Telegram-Neomaster-2CA5E0?style=flat&logo=telegram)](https://t.me/neomaster_dev)
  [![.NET Vesion](https://img.shields.io/badge/.NET_Standard-2.1-blueviolet?style=flat&logo=dotnet)](#)  
  [![NuGet](https://img.shields.io/nuget/v/JsonToLinq.svg?label=NuGet&logo=nuget&logoColor=white&labelColor=gray&color=blue)](https://www.nuget.org/packages/JsonToLinq)

</div>

# JsonToLinq
**JsonToLinq** - lightweight C# library that converts JSON-based query definitions into LINQ expressions. Ideal for building dynamic filters, predicates, and queries.

## 🚀 Use Cases
**JsonToLinq** can be applied in various scenarios where dynamic, runtime-defined queries are needed. Examples include:
- **Server-side filtering:** Apply JSON-defined filters received from front-end applications to collections or database queries.
- **Dynamic reporting:** Build complex filters and predicates for reports without hardcoding logic.
- **Custom dashboards:** Let users define queries for dashboards dynamically and translate them to LINQ expressions.
- **EF Core / Entity Framework queries:** Map JSON filters directly to LINQ queries executed on the database.
- **Audit & logging filters:** Dynamically select subsets of data based on JSON rules for auditing or logging purposes.

## 📌 Default Operator Mapping
| JSON   | LINQ Expression                 | Description           |
|-----   |---------------------------------|-----------------------|
| `&`    | `Expression.And`                | Bitwise AND           |
| `&&`   | `Expression.AndAlso`            | Logical AND           |
| `\|`   | `Expression.Or`                 | Bitwise OR            |
| `\|\|` | `Expression.OrElse`             | Logical OR            |
| `=`    | `Expression.Equal`              | Equal                 |
| `!=`   | `Expression.NotEqual`           | Not equal             |
| `>`    | `Expression.GreaterThan`        | Greater than          |
| `>=`   | `Expression.GreaterThanOrEqual` | Greater than or equal |
| `<`    | `Expression.LessThan`           | Less than             |
| `<=`   | `Expression.LessThanOrEqual`    | Less than or equal    |

## 🧪 Demos
### Filtering Users
```csharp
using System.Text.Json;
using Neomaster.JsonToLinq;
using Neomaster.JsonToLinq.UnitTests;

// 1. Source data.
var users = new List<User>
{
  new() { Id = 1, Balance = 0, LastVisitAt = null },
  new() { Id = 2, Balance = 0, LastVisitAt = DateTime.UtcNow },
  new() { Id = 3, Balance = 0, LastVisitAt = DateTime.UtcNow.AddYears(-10) },
  new() { Id = 4, Balance = 100, LastVisitAt = null },
  new() { Id = 5, Balance = 100, LastVisitAt = DateTime.UtcNow },
  new() { Id = 6, Balance = 100, LastVisitAt = DateTime.UtcNow.AddYears(-10) },
};

// 2. JSON filter definition (simulates front-end request).
var filterJson = JsonDocument.Parse(
  """
  {
    "Logic": "&&",
    "Rules": [
      {
        "Field": "balance",
        "Operator": "=",
        "Value": 0
      },
      {
        "Logic": "||",
        "Rules": [
          {
            "Field": "lastVisitAt",
            "Operator": "=",
            "Value": null
          },
          {
            "Field": "lastVisitAt",
            "Operator": "<=",
            "Value": "2025-01-01T00:00:00Z"
          }
        ]
      }
    ]
  }
  """);

// 3. Parse JSON to LINQ expression and compile.
var filterExpr = JsonLinq.ParseToFilterExpression<User>(filterJson);
var filterLambda = filterExpr.Compile();

// 4. Apply filter.
var filteredUsers = users.Where(filterLambda);

// 5. Output results.
foreach (var fu in filteredUsers)
{
  Console.WriteLine($"Id: {fu.Id}");
}

// Id: 1
// Id: 3
```
