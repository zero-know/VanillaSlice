using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{ProjectName}}.Common.Enums
{
    /// <summary>
    /// Byte type enum representing the status of a product for datatype optimization demonstration
    /// </summary>
    public enum ProductStatus : byte
    {
        Inactive = 0,
        Active = 1,
        Discontinued = 2,
        OutOfStock = 3
    }
}
