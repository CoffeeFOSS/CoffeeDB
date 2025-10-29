using Backend.Data;
using Microsoft.EntityFrameworkCore;

public abstract class BaseRepository<TEntity>(DataContext context)
  where TEntity : class
{
  protected readonly DataContext Context = context;

  public async Task<bool> SaveAllAsync()
  {
    return await Context.SaveChangesAsync() > 0; // SaveChangesAsync returns # of changes saved in our DB 
  }

  public void Update(TEntity entity)
  {
    Context.Entry(entity).State = EntityState.Modified;
  }
}
