using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    public record ProcessOrderCommand
    {
        public ProcessOrderCommand(IReadOnlyCollection<UnvalidatedProducts> inputProducts)
        {
            InputProducts = inputProducts;
;
        }

        public IReadOnlyCollection<UnvalidatedProducts> InputProducts { get; }
    }
}
