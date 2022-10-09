using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IronWren
{
    /// <summary>
    /// Represents the configuration used by the <see cref="WrenVM"/>.
    /// </summary>
    public class WrenConfig
    {
        /// <summary>
        /// Wren will grow (and shrink) the heap automatically as the number of bytes
        /// remaining in use after a collection changes. This number determines the
        /// amount of additional memory Wren will use after a collection, as a
        /// percentage of the current heap size.
        /// <para/>
        /// For example, say that this is 50. After a garbage collection, Wren there
        /// are 400 bytes of memory still in use. That means the next collection will
        /// be triggered after a total of 600 bytes are allocated (including the 400
        /// already in use.
        /// <para/>
        /// Setting this to a smaller number wastes less memory, but triggers more
        /// frequent garbage collections.
        /// <para/>
        /// If zero, defaults to 50%.
        /// <para/>
        /// Won't affect the settings of a <see cref="WrenVM"/> which was created with this <see cref="WrenConfig"/> when changed afterwards.
        /// </summary>
        public int HeapGrowthPercent { get; set; } = WrenUsedConfig.DefaultConfig.HeapGrowthPercent;

        /// <summary>
        /// The number of bytes Wren will allocate before triggering the first garbage collection.
        /// <para/>
        /// If zero, defaults to 10MiB.
        /// <para/>
        /// Won't affect the settings of a <see cref="WrenVM"/> which was created with this <see cref="WrenConfig"/> when changed afterwards.
        /// </summary>
        public nuint InitialHeapSize { get; set; } = WrenUsedConfig.DefaultConfig.InitialHeapSize;

        /// <summary>
        /// After a collection occurs, the threshold for the next collection is
        /// determined based on the number of bytes remaining in use. This allows Wren
        /// to shrink its memory usage automatically after reclaiming a large amount
        /// of memory.
        /// <para/>
        /// This can be used to ensure that the heap does not get too small, which can
        /// in turn lead to a large number of collections afterwards as the heap grows
        /// back to a usable size.
        /// <para/>
        /// If zero, defaults to 1MiB.
        /// <para/>
        /// Won't affect the settings of a <see cref="WrenVM"/> which was created with this <see cref="WrenConfig"/> when changed afterwards.
        /// </summary>
        public nuint MinHeapSize { get; set; } = WrenUsedConfig.DefaultConfig.MinHeapSize;

        /// <summary>
        /// Creates a new instance of the <see cref="WrenConfig"/> class with the default settings.
        /// </summary>
        public WrenConfig()
        { }
    }
}