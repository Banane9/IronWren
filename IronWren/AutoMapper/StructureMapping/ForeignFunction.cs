using System;
using System.Collections.Generic;
using System.Linq;

namespace IronWren.AutoMapper.StructureMapping
{
    internal abstract class ForeignFunction
    {
        internal abstract string GetSource();

        internal WrenForeignMethod Bind()
        {
            throw new NotImplementedException();
        }
    }
}