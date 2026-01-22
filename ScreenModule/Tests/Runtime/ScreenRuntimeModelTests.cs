using System;
using NUnit.Framework;
using UnityEngine;

namespace Strada.Modules.Screen.Tests
{
    [TestFixture]
    public class ScreenRuntimeModelTests
    {
        private ScreenRuntimeModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new ScreenRuntimeModel();
            // Manually call initialize since we're not using the full DI setup
            typeof(ScreenRuntimeModel)
                .GetMethod("OnInitialize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(_model, null);
        }

        [TearDown]
        public void TearDown()
        {
            typeof(ScreenRuntimeModel)
                .GetMethod("OnDispose", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(_model, null);
            _model = null;
        }

        #region Pool Tests

        [Test]
        public void AddToPassivePool_WhenScreenIsNull_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _model.AddToPassivePool(null));
        }

        [Test]
        public void TryGetFromPool_WhenPoolEmpty_ReturnsFalse()
        {
            var result = _model.TryGetFromPool<MockScreenBody>(out var screen);

            Assert.IsFalse(result);
            Assert.IsNull(screen);
        }

        [Test]
        public void TryGetFromPool_ByType_WhenPoolEmpty_ReturnsFalse()
        {
            var result = _model.TryGetFromPool(typeof(MockScreenBody), out var screen);

            Assert.IsFalse(result);
            Assert.IsNull(screen);
        }

        [Test]
        public void GetAllPooledScreens_WhenEmpty_ReturnsEmptyList()
        {
            var pooled = _model.GetAllPooledScreens();

            Assert.IsNotNull(pooled);
            Assert.AreEqual(0, pooled.Count);
        }

        #endregion

        #region Active Screen Tests

        [Test]
        public void AddToActive_WhenScreenIsNull_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _model.AddToActive(null));
        }

        [Test]
        public void RemoveFromActive_WhenScreenIsNull_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _model.RemoveFromActive(null));
        }

        [Test]
        public void IsScreenActive_WhenNotAdded_ReturnsFalse()
        {
            var result = _model.IsScreenActive(typeof(MockScreenBody), 1, out var screen);

            Assert.IsFalse(result);
            Assert.IsNull(screen);
        }

        [Test]
        public void IsScreenActive_Generic_WhenNotAdded_ReturnsFalse()
        {
            var result = _model.IsScreenActive<MockScreenBody>(1, out var screen);

            Assert.IsFalse(result);
            Assert.IsNull(screen);
        }

        [Test]
        public void GetAllActiveScreens_WhenEmpty_ReturnsEmptyList()
        {
            var active = _model.GetAllActiveScreens();

            Assert.IsNotNull(active);
            Assert.AreEqual(0, active.Count);
        }

        [Test]
        public void GetActiveScreensByManager_WhenEmpty_ReturnsEmptyList()
        {
            var active = _model.GetActiveScreensByManager(1);

            Assert.IsNotNull(active);
            Assert.AreEqual(0, active.Count);
        }

        #endregion

        #region Layer Tests

        [Test]
        public void IsLayerOccupied_WhenNoScreenOnLayer_ReturnsFalse()
        {
            var result = _model.IsLayerOccupied(0, 1, out var occupant);

            Assert.IsFalse(result);
            Assert.IsNull(occupant);
        }

        #endregion

        #region Tag Tests

        [Test]
        public void GetActiveScreensByTag_WhenEmpty_ReturnsEmptyList()
        {
            var screens = _model.GetActiveScreensByTag(ScreenTag.None, 1);

            Assert.IsNotNull(screens);
            Assert.AreEqual(0, screens.Count);
        }

        #endregion

        #region Clear Tests

        [Test]
        public void ClearAll_WhenEmpty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _model.ClearAll());
        }

        #endregion

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
