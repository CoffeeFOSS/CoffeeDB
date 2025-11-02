namespace Backend.Interfaces.Repository;

public interface IBaseRepository<TEntity> where TEntity : class
{
  /// <summary>
  /// Saves all changes to the database.
  /// </summary>
  /// <returns>True if the changes were saved successfully; otherwise, false.</returns>
  Task<bool> SaveAllAsync();

  /// <summary>
  /// Lets EF know the entity has been modified explicitly.
  /// </summary>
  /// <param name="entity">The entity that has been modified</param>
  void Update(TEntity entity);
}
