using IronWren.AutoMapper;

namespace IronWren.Tests
{
    [TestClass]
    public class GarbageCollection
    {
        [TestMethod]
        public void Basic()
        {
            var vm = getVM();

            GC.Collect();

            Assert.IsFalse(vm.TryGetTarget(out _));
        }

        [TestMethod]
        public void WithAutoMapper()
        {
            var vm = getVM();
            applyAutoMapping(vm);

            GC.Collect();

            Assert.IsFalse(vm.TryGetTarget(out _));
        }

        private void applyAutoMapping(WeakReference<WrenVM> wrVM)
        {
            wrVM.TryGetTarget(out var vm);
            vm.AutoMap<Test>();
        }

        private WeakReference<WrenVM> getVM()
        {
            return new WeakReference<WrenVM>(new WrenVM());
        }

        private class Test
        {
            [WrenMethod("test")]
            private void test(WrenVM vm)
            { }
        }
    }
}