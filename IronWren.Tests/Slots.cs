using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IronWren.Tests
{
    [TestClass]
    public class Slots
    {
        private readonly WrenVM vm;

        public Slots()
        {
            vm = new WrenVM();
        }

        [TestMethod]
        public void Bool()
        {
            vm.EnsureSlots(1);
            vm.SetSlotNull(0);

            vm.SetSlotBool(0, true);

            Assert.IsTrue(vm.GetSlotBool(0));
        }

        [TestMethod]
        public void Bytes()
        {
            vm.EnsureSlots(1);
            vm.SetSlotNull(0);

            var bytes = new byte[] { 1, 9, 0, 200 };
            vm.SetSlotBytes(0, bytes);

            var wrenBytes = vm.GetSlotBytes(0);

            Assert.AreEqual(bytes.Length, wrenBytes.Length);
            for (var i = 0; i < wrenBytes.Length; ++i)
                Assert.AreEqual(bytes[i], wrenBytes[i]);

            Assert.AreEqual(WrenType.String, vm.GetSlotType(0));
        }

        [TestCleanup]
        public void Cleanup()
        {
            vm.CollectGarbage();
            vm.Dispose();
        }

        [TestMethod]
        public void Count()
        {
            vm.EnsureSlots(10);
            Assert.IsTrue(vm.GetSlotCount() >= 10);
        }

        [TestMethod]
        public void Double()
        {
            vm.EnsureSlots(1);
            vm.SetSlotNull(0);

            vm.SetSlotDouble(0, 200001);

            Assert.AreEqual(200001, vm.GetSlotDouble(0));

            Assert.AreEqual(WrenType.Number, vm.GetSlotType(0));
        }

        [TestMethod]
        public void Foreign()
        {
            vm.EnsureSlots(1);
            vm.SetSlotNull(0);

            vm.SetSlotNewForeign(0, this);

            Assert.AreEqual(this, vm.GetSlotForeign<Slots>(0));

            Assert.AreEqual(WrenType.Foreign, vm.GetSlotType(0));
        }

        [TestMethod]
        public void List()
        {
            vm.EnsureSlots(2);
            vm.SetSlotNull(0, 1);

            vm.SetSlotNewList(0);
            vm.SetSlotDouble(1, 9);

            vm.InsertInList(0, -1, 1);

            Assert.AreEqual(WrenType.List, vm.GetSlotType(0));
        }

        [TestMethod]
        public void String()
        {
            vm.EnsureSlots(1);
            vm.SetSlotNull(0);

            vm.SetSlotString(0, "wren");

            Assert.AreEqual("wren", vm.GetSlotString(0));

            Assert.AreEqual(WrenType.String, vm.GetSlotType(0));
        }

        [TestMethod]
        public void Value()
        {
            vm.EnsureSlots(2);
            vm.SetSlotNull(0, 1);

            Assert.AreEqual(WrenType.Null, vm.GetSlotType(0));

            vm.SetSlotDouble(0, 9);

            var handle = vm.GetSlotHandle(0);
            vm.SetSlotHandle(1, handle);

            Assert.AreEqual(9, vm.GetSlotDouble(1));

            vm.ReleaseHandle(handle);
        }
    }
}