using System.IO;
using Musoq.Service.Client.Core;

namespace Musoq.Console.Client.Evaluator
{
    public abstract class EvaluatorBase
    {
        protected readonly ApplicationArguments Args;

        public EvaluatorBase(ApplicationArguments args)
        {
            Args = args;
        }

        public abstract ResultTable Evaluate();

        protected string GetQuery()
        {
            return string.IsNullOrEmpty(Args.QuerySourceFile)
                ? Args.Query
                : File.ReadAllText(Args.QuerySourceFile);
        }
    }
}
