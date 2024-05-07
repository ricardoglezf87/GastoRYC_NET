using Dommel;
using GARCA.Models;
using System.Linq.Expressions;

namespace GARCA.wsData.Repositories
{
    public interface IRepositoryBase<Q>
        where Q : ModelBase, new()
    {
        public Task<IEnumerable<Q>?> GetAll();

        public Task<Q?> GetById(int id);

        public Task<IEnumerable<Q>?> Get(Expression<Func<Q, bool>> predicate);

        public Task<Q> Save(Q obj);

        public Task<bool> Update(Q obj);

        public Task<int> Insert(Q obj);

        public Task<bool> Delete(Q obj);

        public Task<bool> Delete(int id);
    }
}
