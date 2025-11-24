using Backend.Interfaces.Repository;

namespace Backend.Interfaces;

public interface IUnitOfWork
{
  IUserRepository UserRepository { get; }
  IRoasterRepository RoasterRepository { get; }
  IRevisionMetadataRepository RevisionMetadataRepository { get; }

  Task<bool> SaveAllAsync();
  bool HasChanges();
}
