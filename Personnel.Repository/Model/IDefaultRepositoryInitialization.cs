using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Repository.Model
{
    /// <summary>
    /// Initialize default for repository model
    /// </summary>
    public interface IDefaultRepositoryInitialization
    {
        /// <summary>
        /// Initialize defaults with <paramref name="context">context</paramref>
        /// </summary>
        /// <param name="context">Context for initialize</param>
        void InitializeDefault(RepositoryContext context);
    }
}
