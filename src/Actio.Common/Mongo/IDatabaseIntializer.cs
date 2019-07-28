using System.Threading.Tasks;

namespace Actio.Common.Mongo
{
    public interface IDatabaseIntializer
    {
        Task IntializeAsync();
    }
}