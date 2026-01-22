using System;
using NUnit.Framework;
using UnityEngine;

namespace Strada.Modules.Screen.Tests
{
    [TestFixture]
    public class ScreenPoolManagerTests
    {
        private ScreenPoolManager _poolManager;

        [SetUp]
        public void SetUp()
        {
            _poolManager = new ScreenPoolManager();
            _poolManager.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _poolManager.Dispose();
            _poolManager = null;
        }

        [Test]
        public void AddToPool_WhenScreenIsNull_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _poolManager.AddToPool(null));
        }

        [Test]
        public void RemoveFromPool_WhenScreenIsNull_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _poolManager.RemoveFromPool(null));
        }

        [Test]
        public void TryGetFromPool_WhenPoolEmpty_ReturnsFalse()
        {
            var result = _poolManager.TryGetFromPool<MockScreenBody>(out var screen);

            Assert.IsFalse(result);
            Assert.IsNull(screen);
        }

        [Test]
        public void TryGetFromPool_ByType_WhenPoolEmpty_ReturnsFalse()
        {
            var result = _poolManager.TryGetFromPool(typeof(MockScreenBody), out var screen);

            Assert.IsFalse(result);
            Assert.IsNull(screen);
        }

        [Test]
        public void GetAllPooledScreens_WhenEmpty_ReturnsEmptyList()
        {
            var pooled = _poolManager.GetAllPooledScreens();

            Assert.IsNotNull(pooled);
            Assert.AreEqual(0, pooled.Count);
        }

        [Test]
        public void Clear_WhenEmpty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _poolManager.Clear());
        }

        [Test]
        public void Dispose_WhenCalledMultipleTimes_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _poolManager.Dispose());
            Assert.DoesNotThrow(() => _poolManager.Dispose());
        }

        private class MockScreenBody : IScreenBody
        {
            public ScreenData Data { get; set; }
            public Action<IScreenBody> OnShowAnimationComplete { get; set; }
            public Action<IScreenBody> OnHideAnimationComplete { get; set; }
            public GameObject GameObject => null;
            public RectTransform RectTransform => null;

            public void Show() { }
            public void Hide() { }
            public void OnScreenHidden() { }
            public void BeforeSetup() { }
            public void AfterSetup() { }
            public void SetParameters(object[] parameters) { }
            public bool HasState(ScreenState state) => Data != null && (Data.State & state) == state;
            public void AddState(ScreenState state) { if (Data != null) Data.State |= state; }
            public void RemoveState(ScreenState state) { if (Data != null) Data.State &= ~state; }
        }
    }
}
