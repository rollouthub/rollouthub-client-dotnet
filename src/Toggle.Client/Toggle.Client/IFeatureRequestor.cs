using System.Threading;
using System.Threading.Tasks;

namespace Toggle.Client
{
    public interface IFeatureRequestor
    {
        Task<TogglesResult> GetAll(CancellationToken cancellationToken);
    }
}