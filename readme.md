<div align="center">
  <img src="Docs/img/title.svg">
</div>

<div align="center">

  [![NuGet Version](https://img.shields.io/nuget/v/JsonToLinq.svg?label=NuGet&logo=nuget&logoColor=white&labelColor=gray&color=blue)](https://www.nuget.org/packages/JsonToLinq)

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

## 🧪Demo: Filtering Users
```csharp
using System.Linq.Expressions;
using System.Text.Json;
using Neomaster.JsonToLinq;
using Neomaster.JsonToLinq.UnitTests;

var users = new List<User>
{
  new() { Id = 1, Balance = 0, LastVisitAt = null },
  new() { Id = 2, Balance = 0, LastVisitAt = DateTime.UtcNow },
  new() { Id = 3, Balance = 0, LastVisitAt = DateTime.UtcNow.AddYears(-10) },
  new() { Id = 4, Balance = 100, LastVisitAt = null },
  new() { Id = 5, Balance = 100, LastVisitAt = DateTime.UtcNow },
  new() { Id = 6, Balance = 100, LastVisitAt = DateTime.UtcNow.AddYears(-10) },
};

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
            "Field": "last-visit-at",
            "Operator": "=",
            "Value": null
          },
          {
            "Field": "last-visit-at",
            "Operator": "<=",
            "Value": "2020-01-01T00:00:00Z"
          }
        ]
      }
    ]
  }
  """);

// TODO: Simplify building.
var fieldMapper = new ExpressionFieldMapper()
  .Add(
    "balance",
    new ExpressionField()
    {
      Name = nameof(User.Balance),
      GetValue = jsonElement => Expression.Constant(jsonElement.Value.GetDecimal()),
    })
  .Add(
    "last-visit-at",
    new ExpressionField()
    {
      Name = nameof(User.LastVisitAt),
      GetValue = jsonElement => Expression.Constant(jsonElement?.Deserialize<DateTime?>(), typeof(DateTime?)),
    });

var filterExpr = JsonLinq.ParseToFilterExpression<User>(filterJson, fieldMapper);
var filterLambda = filterExpr.Compile();
var filteredUsers = users.Where(filterLambda);

foreach (var fu in filteredUsers)
{
  Console.WriteLine($"Id: {fu.Id}");
}

// Id: 1
// Id: 3

```
