using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public static class CheckConstraintExtensions
{
  public static string GetColumnName<TEntity, TProperty>(
      this EntityTypeBuilder<TEntity> builder,
      Expression<Func<TEntity, TProperty>> propertyExpression)
      where TEntity : class
  {
    // get C# property name from expression (ex: "GrindTime")
    if (propertyExpression.Body is not MemberExpression memberExpression)
    {
      throw new ArgumentException("Expression must be a property access.", nameof(propertyExpression));
    }
    var propertyName = memberExpression.Member.Name;

    // look up the property in the model's metadata
    var property = builder.Metadata.FindProperty(propertyName);
    if (property == null)
    {
      throw new InvalidOperationException($"Property '{propertyName}' not found on entity '{typeof(TEntity).Name}'.");
    }

    // get the actual mapped column name (e.g., "grind_time")
    var tableName = builder.Metadata.GetTableName();
    var storeObject = StoreObjectIdentifier.Table(tableName!, null);
    var columnName = property.GetColumnName(storeObject);

    // return the column name in double quotes for PostgreSQL case-sensitivity.
    return $"\"{columnName}\"";
  }
}