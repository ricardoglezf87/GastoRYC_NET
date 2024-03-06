using Dommel;
using GARCA.Models;

namespace GARCA.wsData.Managers
{
    public interface IManagerBase<Q>
        where Q : ModelBase, new()
    {
        public Task<IEnumerable<Q>?> GetAll();

        public Task<Q?> GetById(int id);

        public Task<Q?> GetById(DateTime id);

        public Task<Q> Save(Q obj);

        public Task<bool> Update(Q obj);

        public Task<int> Insert(Q obj);

        public Task<bool> Delete(Q obj);

        public Task<bool> Delete(int id);
    }
}
