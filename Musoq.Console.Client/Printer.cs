using System.Data;

namespace Musoq.Console.Client
{
    public abstract class Printer
    {
        protected readonly DataTable Table;

        public Printer(DataTable table)
        {
            Table = table;
        }

        public abstract void Print();
    }
}