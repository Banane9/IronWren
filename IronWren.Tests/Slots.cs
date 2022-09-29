namespace IronWren.Tests
{
    [TestClass]
    public sealed class Slots
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
        public void Handle()
        {
            vm.EnsureSlots(2);
            vm.SetSlotNull(0, 1);

            Assert.AreEqual(WrenType.Null, vm.GetSlotType(0));

            vm.SetSlotDouble(0, 9);

            var handle = vm.GetSlotHandle(0);
            vm.SetSlotHandle(1, handle);

            Assert.AreEqual(9, vm.GetSlotDouble(1));

            handle.Release();
        }

        [TestMethod]
        public void List()
        {
            vm.EnsureSlots(2);
            vm.SetSlotNull(0, 1);

            vm.SetSlotNewList(0);

            Assert.AreEqual(WrenType.List, vm.GetSlotType(0));

            Assert.AreEqual(0, vm.GetListCount(0));

            vm.SetSlotDouble(1, 9);

            vm.InsertInList(0, -1, 1);

            Assert.AreEqual(1, vm.GetListCount(0));

            vm.SetSlotNull(1);
            vm.GetListElement(0, 0, 1);
            Assert.AreEqual(WrenType.Number, vm.GetSlotType(1));
            Assert.AreEqual(9, vm.GetSlotDouble(1));

            vm.SetSlotDouble(1, 3);
            vm.SetListElement(0, 0, 1);
            vm.SetSlotNull(1);
            vm.GetListElement(0, 0, 1);
            Assert.AreEqual(WrenType.Number, vm.GetSlotType(1));
            Assert.AreEqual(3, vm.GetSlotDouble(1));
        }

        [TestMethod]
        public void Map()
        {
            vm.EnsureSlots(4);
            vm.SetSlotNull(0, 1, 2, 3);

            vm.SetSlotNewMap(0);
            Assert.AreEqual(WrenType.Map, vm.GetSlotType(0));

            vm.SetSlotString(1, "key");
            vm.SetSlotString(2, "value");
            vm.SetMapValue(0, 1, 2);

            Assert.AreEqual(1, vm.GetMapCount(0));
            Assert.IsTrue(vm.GetMapContainsKey(0, 1));

            vm.GetMapValue(0, 1, 3);
            Assert.AreEqual("value", vm.GetSlotString(3));

            vm.RemoveMapValue(0, 1, 2);
            Assert.AreEqual(0, vm.GetMapCount(0));
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
    }
}