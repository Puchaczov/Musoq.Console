using Musoq.Console.Client.Helpers;
using Musoq.Service.Client.Core;
using Musoq.Service.Client.Core.Helpers;

namespace Musoq.Console.Client.Evaluator
{
    public class RemoteEvaluator : EvaluatorBase
    {
        public RemoteEvaluator(ApplicationArguments args) 
            : base(args)
        { }

        public override ResultTable Evaluate()
        {
            var api = new ApplicationFlowApi(string.IsNullOrEmpty(Args.Address)
                ? Configuration.Address
                : Args.Address);

            return api.RunQueryAsync(QueryContext.FromQueryText(GetQuery())).Result;
        }
    }
}