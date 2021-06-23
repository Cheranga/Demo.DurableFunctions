using System.Threading;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core.Domain;

namespace Demo.DurableFunctions.Core.Application.DataAccess
{
    public interface ICommand
    {
    }
    
    public interface ICommandHandler<in TCommand> where TCommand: ICommand
    {
        Task<Result> ExecuteAsync(TCommand command, CancellationToken cancellationToken);
    }
}