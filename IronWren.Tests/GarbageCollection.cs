using IronWren.AutoMapper;

namespace IronWren.Tests
{
    [TestClass]
    public class GarbageCollection
    {
        [TestMethod]
        public void Basic()
        {
            var vm = new WeakReference<WrenVM>(new WrenVM());

            GC.Collect();

            WrenVM _;
            Assert.IsFalse(vm.TryGetTarget(out _));
        }

        [TestMethod]
        public void WithAutoMapper()
        {
            var vm = new WrenVM();
            vm.AutoMap<Test>();

            var wrVM = new WeakReference<WrenVM>(vm);
            vm = null;

            GC.Collect();

            WrenVM _;
            Assert.IsFalse(wrVM.TryGetTarget(out _));
        }

        private class Test
        {
            [WrenMethod("test")]
            private void test(WrenVM vm)
            { }
        }
    }
}