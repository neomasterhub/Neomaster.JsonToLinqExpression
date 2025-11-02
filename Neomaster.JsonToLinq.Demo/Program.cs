using System.Linq.Expressions;
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
            "Field": "last-visit-at",
            "Operator": "=",
            "Value": null
          },
          {
            "Field": "last-visit-at",
            "Operator": "<=",
            "Value": "2025-01-01T00:00:00Z"
          }
        ]
      }
    ]
  }
  """);

// 3. Map JSON field names to expression field definitions.
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

// 4. Parse JSON to LINQ expression and compile.
var filterExpr = JsonLinq.ParseToFilterExpression<User>(filterJson, fieldMapper);
var filterLambda = filterExpr.Compile();

// 5. Apply filter.
var filteredUsers = users.Where(filterLambda);

// 6. Output results.
foreach (var fu in filteredUsers)
{
  Console.WriteLine($"Id: {fu.Id}");
}

// Id: 1
// Id: 3
